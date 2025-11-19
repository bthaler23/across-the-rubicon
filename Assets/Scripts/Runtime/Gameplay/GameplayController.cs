using Game.Data;
using Game.Grid;
using Game.Settings;
using GamePlugins.Singleton;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
	public class GameplayController : Singleton<GameplayController>
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
		private ActorInfo startingCharacter;
		[SerializeField]
		private PlayerInputController playerInput;

		[ShowInInspector, ReadOnly]
		private List<ActorController> actors;

		public void InitializeGameplay()
		{
			actors = new List<ActorController>();
			gridExpansionController.InitializeGridWithDimensions();
			gameFlowManager.Inintialize(GetTeamActors(gameplaySettings.TeamInfos));
		}

		public ActorController GetActorAt(Vector2Int index)
		{
			foreach (var actor in actors)
			{
				if (actor.CurrentPosition == index)
				{
					return actor;
				}
			}
			return null;
		}

		private Vector2Int GetRandomStartingPosition()
		{
			return gridData.GetRandomStartingPosition();
		}

		private List<Vector2Int> GetStartingPositions(int count)
		{
			return gridData.GetRandomPositions(count);
		}

		private ActorController SpawnCharacter(Vector2Int gridPosition, ActorInfo characterInfo, TeamInfo team)
		{
			ActorController character = Instantiate<ActorController>(characterInfo.CharacterPrefab);
			character.Initialize(characterInfo, playerInput, team);
			character.Move(gridPosition);
			actors.Add(character);
			return character;
		}

		private List<TeamActors> GetTeamActors(IReadOnlyList<TeamInfo> teams)
		{
			List<TeamActors> resultTeams = new List<TeamActors>();
			foreach (var team in teams)
			{
				List<ITurnActor> turnActor = new List<ITurnActor>();
				foreach (var characterInfo in team.Characters)
				{
					ITurnActor actor = SpawnCharacter(GetRandomStartingPosition(), characterInfo, team);
					turnActor.Add(actor);
				}
				TeamActors newTeam = new TeamActors(team.TeamID, turnActor);
				resultTeams.Add(newTeam);
			}
			return resultTeams;
		}
	}
}
