using Game.Character;
using Game.Data;
using Game.Grid;
using Game.Settings;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using CharacterController = Game.Character.CharacterController;
using CharacterInfo = Game.Character.CharacterInfo;

namespace Game.Gameplay
{
	public class GameplayController : MonoBehaviour
	{
		[SerializeField]
		private GameplaySettings gameplaySettings;

		[PropertySpace(SpaceBefore = 10)]
		[Title("Game Flow")]
		[SerializeField]
		private GameFlowManager gameFlowManager;

		[PropertySpace(SpaceBefore = 10)]
		[Title("Grid Setup")]
		[SerializeField]
		private GridMapData gridData;
		[SerializeField]
		private HexGridManager gridManager;
		[SerializeField]
		private GridExpansionController gridExpansionController;

		[PropertySpace(SpaceBefore = 10)]
		[SerializeField]
		private CharacterInfo startingCharacter;
		[SerializeField]
		private PlayerInputController playerInput;

		public void InitializeGameplay()
		{
			gridExpansionController.InitializeGridWithDimensions();
			gameFlowManager.Inintialize(GetTeamActors(gameplaySettings.TeamInfos));
		}

		private Vector2Int GetRandomStartingPosition()
		{
			return gridData.GetRandomStartingPosition();
		}

		private List<Vector2Int> GetStartingPositions(int count)
		{
			return gridData.GetRandomPositions(count);
		}

		private CharacterController SpawnCharacter(Vector2Int gridPosition, CharacterInfo characterInfo)
		{
			CharacterController character = Instantiate<CharacterController>(characterInfo.CharacterPrefab);
			character.Initialize(characterInfo, playerInput);
			character.Move(gridPosition);
			return character;
		}

		private List<TeamActors> GetTeamActors(IReadOnlyList<TeamInfo> teams)
		{
			List<TeamActors> resultTeams = new List<TeamActors>();
			foreach (var team in teams)
			{
				List<ITurnActor> turnActor = new List<ITurnActor>();
				foreach(var characterInfo in team.Characters)
				{
					ITurnActor actor = SpawnCharacter(GetRandomStartingPosition(), characterInfo);
					turnActor.Add(actor);
				}
				TeamActors newTeam = new TeamActors(team.TeamID, turnActor);
				resultTeams.Add(newTeam);
			}
			return resultTeams;
		}
	}
}
