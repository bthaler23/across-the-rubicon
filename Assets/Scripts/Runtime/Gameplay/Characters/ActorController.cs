using Game.Data;
using Game.Events;
using Game.Gameplay;
using Game.Grid;
using Game.Stats;
using GamePlugins.Events;
using GamePlugins.Utils;
using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game
{
	public class ActorController : MonoBehaviour, ITurnActor
	{
		[BoxGroup("Character")]
		[SerializeField]
		private GameObject aliveGO;
		[BoxGroup("Character")]
		[SerializeField]
		private GameObject deathGO;
		[BoxGroup("UI")]
		[SerializeField]
		private Transform uiPositionXform;
		[BoxGroup("Actions")]
		[SerializeField]
		private Transform actionParentXform;
		[ShowInInspector, ReadOnly]
		private TurnActionBase activeAction;
		[BoxGroup("Actions")]
		[ShowInInspector, ReadOnly]
		private SerializedDictionary<ActionInfo, TurnActionBase> actorActions;
		[ShowInInspector, ReadOnly]
		[BoxGroup("Stats")]
		private SerializedDictionary<StatType, IStatValue> statValues;
		[ShowInInspector, ReadOnly]
		[BoxGroup("Stats")]
		private HealthStats healthStatsCache;

		private PlayerInputController inputController;
		[ShowInInspector, ReadOnly]
		private ActorInfo info;
		[ShowInInspector, ReadOnly]
		private TeamInfo teamInfo;
		[ShowInInspector, ReadOnly]
		private Vector2Int currentPosition;
		[ShowInInspector, ReadOnly]
		private bool isTurnActive = false;
		[ShowInInspector, ReadOnly]
		private int turnMeter;

		public string ID => gameObject.name;
		public ActorInfo Info { get => info; }
		public Vector2Int CurrentPosition { get => currentPosition; }

		public event Action OnTurnCompleted;

		public void Initialize(ActorInfo info, PlayerInputController inputController, TeamInfo teamInfo)
		{
			this.info = info;
			this.teamInfo = teamInfo;
			this.inputController = inputController;
			isTurnActive = false;
			InitializeActions();
			InitializeStats();
		}

		private void InitializeStats()
		{
			statValues = new SerializedDictionary<StatType, IStatValue>();
			healthStatsCache = new HealthStats(info.Health);
			statValues.Add(StatType.Health, healthStatsCache);
			statValues.Add(StatType.MaxHealth, healthStatsCache);
			statValues.Add(StatType.MovementRange, new StatValue(info.MovementRange));
			statValues.Add(StatType.AttackMin, new StatValue(info.MinAttack));
			statValues.Add(StatType.AttackMax, new StatValue(info.MaxAttack));
			statValues.Add(StatType.AttackRange, new StatValue(info.AttackRange));
			statValues.Add(StatType.Speed, new StatValue(info.Speed));
			turnMeter = 0;
		}

		private void InitializeActions()
		{
			actorActions = new();

			foreach (var action in info.DefaultActions)
			{
				if (action == null) continue;
				ActivateAction(action);
			}

		}

		public void Move(Vector2Int positionIndex)
		{
			currentPosition = positionIndex;
			Vector3 worldPosition = HexGridManager.Instance.GridIndexToWordPosition(positionIndex);
			transform.position = worldPosition;
		}

		public void ApplyDamage(int damage)
		{
			healthStatsCache.ApplyDamage(damage);

			if (!healthStatsCache.IsAlive)
			{
				deathGO.SetActive(true);
				aliveGO.SetActive(false);
				EventBus.Publish<OnCharacterDiedEvent>(new OnCharacterDiedEvent(this));
			}
		}

		public bool HasAnyActions()
		{
			if (actorActions == null || actorActions.Count == 0 || !healthStatsCache.IsAlive) return false;

			foreach (var action in actorActions)
			{
				if (action.Value.IsAvailable())
				{
					return true;
				}
			}

			return false;
		}

		public void TurnStart()
		{
			isTurnActive = true;
			//TEMP
			SetActiveAction(GetFirstAction());
		}

		private TurnActionBase GetFirstAction()
		{
			return actorActions.Values.FirstOrDefault();
		}

		public void TurnEnd()
		{
			EventBus.Publish<ActiveActorRefreshEvent>(new ActiveActorRefreshEvent(null));
			DisabelCurrentAction(true);
			isTurnActive = false;
		}

		private void OnActionCompleted()
		{
			OnTurnCompleted?.Invoke();

		}

		private bool DisabelCurrentAction(bool forced = false)
		{
			if (activeAction != null)
			{
				bool canDisable = forced || !activeAction.IsActionStarted();
				if (canDisable)
				{
					activeAction.OnActionCompleted -= OnActionCompleted;
					activeAction.DisableAction();
				}
				else
				{
					return false;
				}
			}

			activeAction = null;
			return true;
		}

		public Sprite GetActorIcon()
		{
			return info.CharacterAvatar;
		}

		public Color GetTeamColor()
		{
			return teamInfo.TeamColor;
		}

		public IReadOnlyList<TurnActionBase> GetActions()
		{
			return actorActions.Values.ToList();
		}

		public TurnActionBase GetActiveAction()
		{
			return activeAction;
		}

		public void SetActiveAction(TurnActionBase action)
		{
			if (!action.IsAvailable()) return;

			if (DisabelCurrentAction())
			{
				activeAction = action;
				activeAction.ActivateAction();
				activeAction.OnActionCompleted += OnActionCompleted;
				EventBus.Publish<ActiveActorRefreshEvent>(new ActiveActorRefreshEvent(this));
			}
		}

		public IStatValue GetStat(StatType type)
		{
			if (statValues.TryGetValue(type, out var statValue))
			{
				return statValue;
			}
			return null;
		}

		public int GetStatValue(StatType type)
		{
			if (statValues.TryGetValue(type, out var statValue))
			{
				return Mathf.RoundToInt(statValue.GetValue());
			}
			return 0;
		}

		#region Event Registration/Unregistration
		public void RegisterHexCellClickEvent(Action<Vector2Int> onCellClicked)
		{
			inputController.CellClicked += onCellClicked;
		}

		public void UnRegisterHexCellClickEvent(Action<Vector2Int> onCellClicked)
		{
			inputController.CellClicked -= onCellClicked;
		}

		public int GetCharacterAttackDamage()
		{
			//TODO GYURI: fix this temporary implementation
			int minAttack = GetStatValue(StatType.AttackMin);
			int maxAttack = GetStatValue(StatType.AttackMin);

			return UnityEngine.Random.Range(minAttack, maxAttack);
		}

		public float GetTurnSpeed()
		{
			return GetStatValue(StatType.Speed);
		}

		public Transform GetUIXform()
		{
			return uiPositionXform;
		}

		public void ActivateAction(ActionInfo actionInfo)
		{
			if (actorActions.ContainsKey(actionInfo)) return;
			TurnActionBase actionInstance = Instantiate(actionInfo.ActionPrefab, actionParentXform);
			actionInstance.Initialize(actionInfo, this);
			actorActions.Add(actionInfo, actionInstance);
		}

		public void RemoveAction(ActionInfo actionInfo)
		{
			if (actorActions.ContainsKey(actionInfo))
			{
				var actionInstance = actorActions[actionInfo];
				actionInstance.DisableAction();
				Destroy(actionInstance.gameObject);
				actorActions.Remove(actionInfo);
			}
		}

		public bool IsAlive()
		{
			return healthStatsCache.IsAlive;
		}
		#endregion
	}
}
