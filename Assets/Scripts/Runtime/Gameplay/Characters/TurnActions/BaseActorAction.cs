using Game.Character;
using Game.Data;
using Game.Gameplay;
using Game.Grid;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	public abstract class BaseActorAction : TurnActionBase
	{
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

		public override void Initialize(AbilityInfo action, ITurnActor owner)
		{
			base.Initialize(action, owner);
			this.owner = owner as CharacterBehaviour;
			rangePositions = new List<Vector2Int>();
			isActionActive = false;
			isActionStarted = false;
		}

		public override bool IsActionStarted()
		{
			return isActionStarted;
		}

		protected void StartAction()
		{
			isActionStarted = true;
		}

		public override void ActivateAction()
		{
			if (HasRange())
			{
				rangePositions = HexGridManager.Instance.GetMovementRangePositions(Owner.CurrentPosition, GetActionRange());
				HexGridManager.Instance.HighlightPositions(Owner.CurrentPosition, Owner.GetTeamColor(), RangePositions, ActionInfo.Color);
			}
			Owner.RegisterHexCellClickEvent(OnCellClicked);
			isActionActive = true;
			isActionStarted = false;
		}

		public override void DisableAction()
		{
			if (HasRange())
			{
				HexGridManager.Instance.ResetPositions();
			}
			Owner.UnRegisterHexCellClickEvent(OnCellClicked);
			isActionActive = false;
			isActionStarted = false;
		}

		protected virtual void OnCellClicked(Vector2Int gridIndex)
		{
		}

		protected bool IsInRange(Vector2Int targetPosition)
		{
			return rangePositions.Contains(targetPosition);
		}

		protected virtual bool HasRange()
		{
			return false;
		}

		protected virtual int GetActionRange()
		{
			if (actionInfo.RangeBehaviourType == Data.AbilityInfo.RangeBehaviour.AdditionToCharacterRange)
				return GetCharacterRangeValue() + actionInfo.Range;
			return actionInfo.Range;
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

		protected virtual void AllocateSelfKeywords()
		{
			foreach (var allocation in actionInfo.KeywordAllocations)
			{
				if (allocation.Target == ItemAllocationTarget.Self)
				{
					Owner.EquipmentController.EquipKeyword(allocation.Keyword, allocation.Count);
				}
			}
		}
	}
}
