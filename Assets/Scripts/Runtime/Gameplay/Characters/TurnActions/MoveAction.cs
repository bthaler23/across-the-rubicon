using Game.Gameplay;
using Game.Grid;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class MoveAction : MonoBehaviour, ITurnAction
	{
		[SerializeField]
		private ActionInfo actionInfo;
		[ShowInInspector, ReadOnly]
		private ActorController owner;
		[ShowInInspector, ReadOnly]
		private List<Vector2Int> possibleMovementPositions;

		public event Action OnActionCompleted;

		public void Initialize(ITurnActor owner)
		{
			this.owner = owner as ActorController;
			possibleMovementPositions = new List<Vector2Int>();
		}

		private void OnCellClicked(Vector2Int gridIndex)
		{
			if (possibleMovementPositions.Contains(gridIndex))
			{
				owner.Move(gridIndex);
				OnActionCompleted?.Invoke();
			}
		}

		private void HighLightPossiblePositions()
		{
			possibleMovementPositions = HexGridManager.Instance.GetMovementRangePositions(owner.CurrentPosition, owner.Info.MovementRange);
			HexGridManager.Instance.HighlightPositions(owner.CurrentPosition, possibleMovementPositions);
		}

		public void ActivateAction()
		{
			HighLightPossiblePositions();
			owner.RegisterHexCellClickEvent(OnCellClicked);
		}

		public void DisableAction()
		{
			HexGridManager.Instance.ResetPositions();
			owner.UnRegisterHexCellClickEvent(OnCellClicked);
		}

		public bool IsAvailable()
		{
			return true;
		}
	}
}
