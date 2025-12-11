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
		internal CharacterEquipmentController equipmentController;
		[BoxGroup("Character")]
		[SerializeField]
		internal GameObject aliveGO;
		[BoxGroup("Character")]
		[SerializeField]
		internal GameObject deathGO;
		[BoxGroup("UI")]
		[SerializeField]
		internal CharacterHealthBarUI healthBarUI;
		[BoxGroup("UI")]
		[SerializeField]
		internal Transform uiActionBarXform;
		[BoxGroup("UI")]
		[SerializeField]
		internal Color statsLostUiTextColor = Color.red;
		[BoxGroup("UI")]
		[SerializeField]
		internal Color statsGainUiTextColor = Color.red;
		[BoxGroup("Actions")]
		[SerializeField]
		internal Transform actionParentXform;
		[ShowInInspector, ReadOnly]
		internal TurnActionBase activeAction;
		[BoxGroup("Actions")]
		[ShowInInspector, ReadOnly]
		internal SerializedDictionary<AbilityInfo, TurnActionBase> actorActions;
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
		public CharacterEquipmentController EquipmentController { get => equipmentController; }

		public event Action OnTurnCompleted;
		public event Action OnHealthChanged;
		public event Action<HitData> OnPrepareHit;
		public event Action<HitData> OnReceiveHit;

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
				EquipAction(action);
			}
		}

		public void Move(Vector2Int positionIndex)
		{
			currentPosition = positionIndex;
			Vector3 worldPosition = HexGridManager.Instance.GridIndexToWordPosition(positionIndex);
			transform.position = worldPosition;
		}

		public void ApplyDamage(HitData hitData)
		{
			OnReceiveHit?.Invoke(hitData);

			characterStats.ApplyDamage(hitData.damage);
			OnHealthChanged?.Invoke();

			EventBus.Publish<OnShowFloatingUiText>(new OnShowFloatingUiText(uiActionBarXform, $"-{hitData.damage}", statsLostUiTextColor, null));

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
			int minAttack = GetStatValueInt(StatType.AttackMin);
			int maxAttack = GetStatValueInt(StatType.AttackMax);

			return UnityEngine.Random.Range(minAttack, maxAttack);
		}

		public HitData GetHitData(CharacterBehaviour target)
		{
			int damage = GetCharacterAttackDamage();
			HitData hitData = new HitData(damage, this, target);
			OnPrepareHit?.Invoke(hitData);
			return hitData;
		}

		public float GetTurnSpeed()
		{
			return GetStatValueFloat(StatType.Speed);
		}

		public Transform GetUIXform()
		{
			return uiActionBarXform;
		}

		public void EquipAction(AbilityInfo actionInfo)
		{
			if (actorActions.ContainsKey(actionInfo)) return;
			TurnActionBase actionInstance = Instantiate(actionInfo.ActionPrefab, actionParentXform);
			actionInstance.Initialize(actionInfo, this);
			actorActions.Add(actionInfo, actionInstance);
		}

		public void RemoveAction(AbilityInfo actionInfo)
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
