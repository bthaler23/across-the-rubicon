using Game.Gameplay;
using Game.Grid;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	public abstract class BaseActorAction : MonoBehaviour, ITurnAction
	{
		[SerializeField]
		private ActionInfo actionInfo;
		[ShowInInspector, ReadOnly]
		private ActorController owner;

		[ShowInInspector, ReadOnly]
		private List<Vector2Int> rangePositions;
		[ShowInInspector, ReadOnly]
		private bool isActionStarted = false;

		public ActorController Owner { get => owner; }
		public ActionInfo ActionInfo { get => actionInfo; }
		public List<Vector2Int> RangePositions { get => rangePositions; }

		public event Action OnActionCompleted;

		public virtual void Initialize(ITurnActor owner)
		{
			this.owner = owner as ActorController;
			rangePositions = new List<Vector2Int>();
		}

		public bool IsActionStarted()
		{
			return isActionStarted;
		}

		protected void StartAction()
		{
			isActionStarted = true;
		}

		public virtual void ActivateAction()
		{
			if (HasRange())
			{
				rangePositions = HexGridManager.Instance.GetMovementRangePositions(Owner.CurrentPosition, GetRange());
				HexGridManager.Instance.HighlightPositions(Owner.CurrentPosition, Owner.GetTeamColor(), RangePositions, ActionInfo.Color);
			}
			Owner.RegisterHexCellClickEvent(OnCellClicked);
			isActionStarted = false;
		}

		public virtual void DisableAction()
		{
			if (HasRange())
			{
				HexGridManager.Instance.ResetPositions();
			}
			Owner.UnRegisterHexCellClickEvent(OnCellClicked);
			isActionStarted = false;
		}

		public abstract bool IsAvailable();

		protected virtual void OnCellClicked(Vector2Int gridIndex)
		{
		}

		protected void FireOnCompletedEvent()
		{
			OnActionCompleted?.Invoke();
		}

		protected bool IsInRange(Vector2Int targetPosition)
		{
			return rangePositions.Contains(targetPosition);
		}

		protected virtual bool HasRange()
		{
			return false;
		}

		protected virtual int GetRange()
		{
			return 0;
		}

		public Sprite GetIcon()
		{
			return actionInfo.Icon;
		}
	}
}
