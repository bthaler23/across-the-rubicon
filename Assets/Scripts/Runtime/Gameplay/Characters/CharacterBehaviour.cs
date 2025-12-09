using Game.Character;
using Game.Data;
using Game.Events;
using Game.Gameplay;
using Game.Grid;
using Game.Stats;
using Game.UI;
using GamePlugins.Events;
using GamePlugins.Utils;
using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.Rendering;

namespace Game.Character
{
	public class CharacterBehaviour : MonoBehaviour, ITurnActor
	{
		[BoxGroup("Character")]
		[SerializeField]
		private CharacterEquipmentController equipmentController;
		[BoxGroup("Character")]
		[SerializeField]
		private GameObject aliveGO;
		[BoxGroup("Character")]
		[SerializeField]
		private GameObject deathGO;
		[BoxGroup("UI")]
		[SerializeField]
		private CharacterHealthBarUI healthBarUI;
		[BoxGroup("UI")]
		[SerializeField]
		private Transform uiActionBarXform;
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
		internal CharacterStats characterStats;

		private PlayerInputController inputController;
		[ShowInInspector, ReadOnly]
		private CharacterInfoData info;

		[ShowInInspector, ReadOnly]
		private TeamInfo teamInfo;
		[ShowInInspector, ReadOnly]
		private Vector2Int currentPosition;
		[ShowInInspector, ReadOnly]
		private bool isTurnActive = false;

		public string ID => gameObject.name;
		public CharacterInfoData Info { get => info; }
		public Vector2Int CurrentPosition { get => currentPosition; }

		public event Action OnTurnCompleted;

		public event Action OnHealthChanged;

		public void Initialize(CharacterInfoData info, CharacterEquipmentSetup equipmentData, PlayerInputController inputController, TeamInfo teamInfo)
		{
			this.info = info;
			this.teamInfo = teamInfo;
			this.inputController = inputController;
			isTurnActive = false;
			InitializeActions();
			InitializeStats();
			InitializeEquipment(equipmentData);

			UpdateVariableStats();
			InitializeHealthbar();
		}

		private void InitializeHealthbar()
		{
			healthBarUI.Initialize(this);
		}

		private void InitializeEquipment(CharacterEquipmentSetup equipmentData)
		{
			equipmentController.Initialize(this, equipmentData);
		}

		private void InitializeStats()
		{
			characterStats = new CharacterStats(info);
		}

		private void UpdateVariableStats()
		{
			characterStats.UpdateVariableStats();
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
			characterStats.ApplyDamage(damage);
			OnHealthChanged?.Invoke();

			EventBus.Publish<OnShowFloatingUiText>(new OnShowFloatingUiText(uiActionBarXform, $"-{damage}"));

			if (!characterStats.IsAlive)
			{
				deathGO.SetActive(true);
				aliveGO.SetActive(false);
				EventBus.Publish<OnCharacterDiedEvent>(new OnCharacterDiedEvent(this));
			}

		}

		public bool HasAnyActions()
		{
			if (actorActions == null || actorActions.Count == 0 || !characterStats.IsAlive) return false;

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

		public float GetStatValueFloat(StatType type)
		{
			var stat = characterStats.GetStat(type);
			if (stat != null)
			{
				return stat.GetValue();
			}
			return 0;
		}

		public int GetStatValueInt(StatType type)
		{
			return Mathf.RoundToInt(GetStatValueFloat(type));
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
			int minAttack = GetStatValueInt(StatType.AttackMin);
			int maxAttack = GetStatValueInt(StatType.AttackMin);

			return UnityEngine.Random.Range(minAttack, maxAttack);
		}

		public float GetTurnSpeed()
		{
			return GetStatValueFloat(StatType.Speed);
		}

		public Transform GetUIXform()
		{
			return uiActionBarXform;
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
			return characterStats.IsAlive;
		}
		#endregion
	}
}
