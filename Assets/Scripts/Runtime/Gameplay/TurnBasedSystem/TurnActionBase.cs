using Game.Character;
using Game.Data;
using Game.Grid;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Gameplay
{
	public abstract class TurnActionBase : MonoBehaviour
	{
		[ShowInInlineEditors, ReadOnly]
		protected AbilityInfo abilityInfo;
		[ShowInInspector, ReadOnly]
		private CharacterBehaviour owner;
		[ShowInInspector, ReadOnly]
		private List<Vector2Int> rangePositions;
		[ShowInInspector, ReadOnly]
		private bool isActionActive = false;
		[ShowInInspector, ReadOnly]
		private bool isActionStarted = false;

		public CharacterBehaviour Owner { get => owner; }
		public List<Vector2Int> RangePositions { get => rangePositions; }
		public AbilityInfo ActionInfo { get => abilityInfo; }

		public event Action OnActionCompleted;

		public virtual void Initialize(AbilityInfo action, ITurnActor owner)
		{
			abilityInfo = action;
			this.owner = owner as CharacterBehaviour;
			rangePositions = new List<Vector2Int>();
			isActionActive = false;
			isActionStarted = false;
		}

		public virtual bool CanExecuteAction()
		{
			int availableMana = Owner.GetStatValueInt(Stats.StatType.Mana);
			bool hasMana = false;
			bool hasKewords = true;
			if (abilityInfo.ConsumeAllMana)
				hasMana = availableMana > 0;
			else
				hasMana = abilityInfo.ManaCost <= availableMana;

			foreach (var i in abilityInfo.KeywordCost)
			{
				int ownedCount = Owner.EquipmentController.GetKeywordCount(i.Keyword);
				if (ownedCount < i.Count)
				{
					hasKewords = false;
					break;
				}
			}

			bool canExecute = hasMana && hasKewords;
			if (!canExecute)
				Owner.ShowNotEnoughMana();
			return canExecute;
		}

		public virtual bool IsActionStarted()
		{
			return isActionStarted;
		}

		public virtual bool IsAvailable()
		{
			return true;
		}

		public virtual void ActivateAction()
		{
			if (HasRange())
			{
				rangePositions = HexGridManager.Instance.GetMovementRangePositions(Owner.CurrentPosition, GetActionRange());
				HexGridManager.Instance.HighlightPositions(Owner.CurrentPosition, Owner.GetTeamColor(), RangePositions, ActionInfo.Color);
			}
			Owner.RegisterHexCellClickEvent(OnCellClicked);
			RegisterModifiers();
			isActionActive = true;
			isActionStarted = false;
		}

		public virtual void DisableAction()
		{
			if (HasRange())
			{
				HexGridManager.Instance.ResetPositions();
			}
			UnRegisterModifiers();
			Owner.UnRegisterHexCellClickEvent(OnCellClicked);
			isActionActive = false;
			isActionStarted = false;
		}

		private void RegisterModifiers()
		{
			foreach (var modifier in abilityInfo.StatModifiers)
			{
				Owner.CharacterStats.AddModifier(new Stats.StatModifier(modifier, this));
			}
		}

		private void UnRegisterModifiers()
		{
			Owner.CharacterStats.RemoveModifier(this);
		}

		protected void StartAction()
		{
			isActionStarted = true;
			if (abilityInfo.ConsumeAllMana)
			{
				int availableMana = Owner.GetStatValueInt(Stats.StatType.Mana);
				Owner.ConsumeMana(availableMana);
			}
			else
				Owner.ConsumeMana(abilityInfo.ManaCost);

			foreach (var i in abilityInfo.KeywordCost)
			{
				Owner.EquipmentController.RemoveKeyword(i.Keyword, i.Count);
			}
		}

		protected void FireOnCompletedEvent()
		{
			OnActionCompleted?.Invoke();
		}

		public void OnUIInvokeAction()
		{
			if (UseExecuteButton())
			{
				if (CanExecuteAction())
				{
					ExecuteOnUIClickAction();
				}
			}
		}

		protected virtual void ExecuteOnUIClickAction()
		{
			ExecuteCommonActions();
		}

		protected void OnCellClicked(Vector2Int gridIndex)
		{
			if (CanExecuteAction())
			{
				ExecuteOnCellClickAction(gridIndex);
			}
		}

		protected virtual void ExecuteOnCellClickAction(Vector2Int gridIndex)
		{
			ExecuteCommonActions();

			CharacterBehaviour target = GameplayController.Instance.GetActorAt(gridIndex);
			AllocateKeywords(target);
		}

		protected virtual void ExecuteCommonActions()
		{
			StartAction();
			AllocateSelfKeywords();
			if (abilityInfo.Target != TargetCategory.None && abilityInfo.TargetCategoryMode == TargetCategoryMode.All)
			{
				List<CharacterBehaviour> targets = new List<CharacterBehaviour>();
				IReadOnlyCollection<CharacterBehaviour> characterBehaviours = GameplayController.Instance.GetAllActors();
				foreach (var i in characterBehaviours)
				{
					if (abilityInfo.Target == TargetCategory.Ally && i.GetTeamColor() != Owner.GetTeamColor())
						continue;
					if (abilityInfo.Target == TargetCategory.Enemy && i.GetTeamColor() == Owner.GetTeamColor())
						continue;
					if (i != owner)
						targets.Add(i);
				}

				foreach (var target in targets)
				{
					AllocateKeywords(target);
				}
			}
		}

		protected virtual void AllocateSelfKeywords()
		{
			foreach (var allocation in abilityInfo.SelfkeywordAllocations)
			{
				Owner.EquipmentController.EquipKeyword(allocation.Keyword, allocation.Count);
			}
		}

		protected virtual void AllocateKeywords(CharacterBehaviour target)
		{
			foreach (var allocation in abilityInfo.KeywordAllocations)
			{
				target.EquipmentController.EquipKeyword(allocation.Keyword, allocation.Count);
			}
		}

		public bool UseExecuteButton()
		{
			return abilityInfo.ExecutionMode == AbilityInfo.ExecutionType.ButtonPress;
		}

		public virtual string GetDescription()
		{
			if (abilityInfo.DescriptionOption != AbilityInfo.DescriptionType.StaticText)
			{
				Debug.LogError($"Action {abilityInfo.name} has dynamic description but script does not override it!");
			}
			return abilityInfo.Description;
		}

		public string GetName()
		{
			return abilityInfo.ItemName;
		}

		public virtual Sprite GetIcon()
		{
			if (abilityInfo.OverrideWithWeaponIcon)
			{
				var weapon = Owner.EquipmentController.GetEquippedWeapon();
				return weapon?.Icon ?? abilityInfo.Icon;
			}
			return abilityInfo.Icon;
		}

		protected bool IsInRange(Vector2Int targetPosition)
		{
			return rangePositions.Contains(targetPosition);
		}

		protected virtual bool HasRange()
		{
			return abilityInfo.ExecutionMode == AbilityInfo.ExecutionType.GridSelect;
		}

		protected virtual int GetActionRange()
		{
			if (abilityInfo.RangeBehaviourType == Data.AbilityInfo.ValueModifierType.Additional)
				return GetCharacterRangeValue() + abilityInfo.Range;
			return abilityInfo.Range;
		}

		protected virtual int GetCharacterRangeValue()
		{
			return Owner.GetStatValueInt(Stats.StatType.AttackRange);
		}

		protected IEnumerator DelayedExecuteAction(float delay, Action action)
		{
			yield return new WaitForSeconds(delay);
			action?.Invoke();
		}

	}
}
