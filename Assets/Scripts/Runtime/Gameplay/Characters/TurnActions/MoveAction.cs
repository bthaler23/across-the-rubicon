using Game.Gameplay;
using Game.Grid;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class MoveAction : BaseActorAction
	{
		[SerializeField, Tooltip("Units per second movement speed for interpolated movement.")]
		private float movementSpeed = 5f;

		// Runtime state
		private bool isMoving;
		private Vector3 movementTargetWorldPos;
		private Vector2Int? movementTargetGridIndex;

		protected override void OnCellClicked(Vector2Int gridIndex)
		{
			base.OnCellClicked(gridIndex);

			if (isMoving) return; // prevent selecting a new target while moving

			if (IsInRange(gridIndex))
			{
				StartAction();
				movementTargetGridIndex = gridIndex;
				movementTargetWorldPos = HexGridManager.Instance.GridIndexToWordPosition(gridIndex);
				isMoving = true; // start movement interpolation
			}
		}

		private void Update()
		{
			if (!isMoving) return;
			if (Owner == null) { isMoving = false; return; }

			// Smooth movement (constant speed)
			var current = Owner.transform.position;
			var target = movementTargetWorldPos;
			Owner.transform.position = Vector3.MoveTowards(current, target, movementSpeed * Time.deltaTime);

			// Check arrival
			if (Vector3.SqrMagnitude(Owner.transform.position - target) <= 0.0001f)
			{
				// Snap logical position when arrived
				Owner.Move(movementTargetGridIndex.Value);
				isMoving = false;
				FireOnCompletedEvent();
			}
		}

		public override void ActivateAction()
		{
			base.ActivateAction();
			isMoving = false;
			movementTargetGridIndex = null;
		}

		public override void DisableAction()
		{
			base.DisableAction();
			isMoving = false;
			if (movementTargetGridIndex.HasValue)
				Owner.Move(movementTargetGridIndex.Value);
		}

		public override bool IsAvailable()
		{
			return true;
		}

		protected override bool HasRange()
		{
			return true;
		}

		protected override int GetRange()
		{
			return Owner.GetStatValueInt(Stats.StatType.MovementRange);
		}

		public override string GetDescription()
		{
			return $"Movement Range: {GetRange()}";
		}
	}
}
