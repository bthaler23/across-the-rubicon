using Game.Gameplay;
using Game.Grid;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character
{
	public class CharacterController : MonoBehaviour
	{
		private PlayerInputController inputController;
		private CharacterInfo character;
		private Vector2Int currentPosition;
		private List<Vector2Int> possibleMovementPositions;

		public void Initialize(CharacterInfo character, PlayerInputController inputController)
		{
			this.character = character;
			possibleMovementPositions = new List<Vector2Int>();
			this.inputController = inputController;
			inputController.CellClicked += OnCellClicked;
		}

		private void OnCellClicked(Vector2Int gridIndex)
		{
			if(possibleMovementPositions.Contains(gridIndex))
			{
				Move(gridIndex);
			}
		}

		public void Move(Vector2Int positionIndex)
		{
			HexGridManager.Instance.ResetPositions();
			currentPosition = positionIndex;
			Vector3 worldPosition = HexGridManager.Instance.GridIndexToWordPosition(positionIndex);
			transform.position = worldPosition;
			HighLightPossiblePositions();
		}

		private void HighLightPossiblePositions()
		{
			possibleMovementPositions = HexGridManager.Instance.GetMovementRangePositions(currentPosition, character.MovementRange);
			HexGridManager.Instance.HighlightPositions(possibleMovementPositions);
		}
	}
}
