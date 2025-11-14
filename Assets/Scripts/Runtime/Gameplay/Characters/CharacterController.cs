using Game.Gameplay;
using Game.Grid;
using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character
{
	public class CharacterController : MonoBehaviour, ITurnActor
	{
		private PlayerInputController inputController;
		[ShowInInspector, ReadOnly]
		private CharacterInfo character;
		[ShowInInspector, ReadOnly]
		private Vector2Int currentPosition;
		[ShowInInspector, ReadOnly]
		private bool isTurnActive = false;
		private List<Vector2Int> possibleMovementPositions;


		public string ID => gameObject.name;

		public event Action OnTurnCompleted;

		public void Initialize(CharacterInfo character, PlayerInputController inputController)
		{
			this.character = character;
			possibleMovementPositions = new List<Vector2Int>();
			this.inputController = inputController;
			isTurnActive = false;
		}

		private void OnCellClicked(Vector2Int gridIndex)
		{
			if (isTurnActive)
			{
				if (possibleMovementPositions.Contains(gridIndex))
				{
					Move(gridIndex);
					OnTurnCompleted?.Invoke();
				}
			}
		}

		public void Move(Vector2Int positionIndex)
		{
			currentPosition = positionIndex;
			Vector3 worldPosition = HexGridManager.Instance.GridIndexToWordPosition(positionIndex);
			transform.position = worldPosition;
		}

		private void HighLightPossiblePositions()
		{
			possibleMovementPositions = HexGridManager.Instance.GetMovementRangePositions(currentPosition, character.MovementRange);
			HexGridManager.Instance.HighlightPositions(currentPosition, possibleMovementPositions);
		}

		public bool HasAnyActions()
		{
			return true;
		}

		public void TurnStart()
		{
			isTurnActive = true;
			HighLightPossiblePositions();
			inputController.CellClicked += OnCellClicked;
		}

		public void TurnEnd()
		{
			isTurnActive = false;
			HexGridManager.Instance.ResetPositions();
			inputController.CellClicked -= OnCellClicked;
		}
	}
}
