using GamePlugin.Grid;
using UnityEngine;
using Game.Character;
using CharacterInfo = Game.Character.CharacterInfo;
using CharacterController = Game.Character.CharacterController;
using System;
using Game.Grid;

namespace Game.Gameplay
{
	public class GameplayController : MonoBehaviour
	{
		[SerializeField]
		private GridMapData gridData;
		[SerializeField]
		private HexGridManager gridManager;
		[SerializeField]
		private GridExpansionController gridExpansionController;

		[SerializeField]
		private CharacterInfo startingCharacter;
		[SerializeField]
		private PlayerInputController playerInput;

		public void InitializeGameplay()
		{
			gridExpansionController.InitializeGridWithDimensions();
			SpawnCharacter(GetStartingPosition(), startingCharacter);
		}

		private Vector2Int GetStartingPosition()
		{
			return gridData.GetRandomPosition();
		}

		private void SpawnCharacter(Vector2Int gridPosition, CharacterInfo startingCharacter)
		{
			CharacterController character = Instantiate<CharacterController>(startingCharacter.CharacterPrefab);
			character.Initialize(startingCharacter, playerInput);
			character.Move(gridPosition);
		}
	}
}
