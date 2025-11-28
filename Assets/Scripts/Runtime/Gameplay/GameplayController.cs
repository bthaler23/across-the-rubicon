using Game.Data;
using Game.Grid;
using Game.Progress;
using Game.Settings;
using Game.UI;
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
		private CameraController cameraController;
		[SerializeField]
		private float cameraBoarderPadding = 2f;

		[PropertySpace(SpaceBefore = 10)]
		[SerializeField]
		private PlayerInputController playerInput;

		[PropertySpace(SpaceBefore = 10)]
		[Title("Game Flow")]
		[SerializeField]
		private GameplayFlowManager gameFlowManager;

		[PropertySpace(SpaceBefore = 10)]
		[Title("Grid Setup")]
		[SerializeField]
		private GridMapData gridData;
		[SerializeField]
		private HexGridManager gridManager;
		[SerializeField]
		private GridExpansionController gridExpansionController;

		[ShowInInspector, ReadOnly]
		private List<ActorController> actors;

		public void InitializeGameplay()
		{
			actors = new List<ActorController>();
			gridExpansionController.InitializeGrid(ProgressManager.Instance.CurrentDungeonRoom.GridSetup);
			gameFlowManager.Inintialize(GetTeams());
			CenterCameraOnDungeon(gridManager, cameraController, cameraBoarderPadding);
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

		private List<TeamActors> GetTeams()
		{
			List<TeamInfo> teams = new List<TeamInfo>();

			TeamInfo playerTeam = new TeamInfo();
			foreach(var character in ProgressManager.Instance.CurrentHeroes)
			{
				playerTeam.AddCharacter(character);
			}
			TeamInfo enemyTeam = new TeamInfo();
			foreach(var character in ProgressManager.Instance.CurrentDungeonRoom.EnemyActors)
			{
				enemyTeam.AddCharacter(character);
			}

			teams.Add(playerTeam);
			teams.Add(enemyTeam);

			return GetTeamActors(teams);
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

		[BoxGroup("DEBUG")]
		[Button]
		private static void CenterCameraOnDungeon(HexGridManager gridManager, CameraController cameraController, float cameraBoarderPadding)
		{
			Vector2 stageCenter = gridManager.GetGridCenter();
			Vector2 stageSize = gridManager.GetGridSize();

			cameraController.MoveToPosition(new Vector3(stageCenter.x, stageCenter.y));
			//change camera size to adjust to stage
			Vector3 bottomLeft = cameraController.Camera.ScreenToWorldPoint(new Vector3(0, 0, 0));
			Vector3 topRight = cameraController.Camera.ViewportToWorldPoint(new Vector3(1, 1, 0));

			float verticalDistance = topRight.y - bottomLeft.y;
			float horizontalDistance = topRight.x - bottomLeft.x;

			float neededVerticalDistance = stageSize.y + cameraBoarderPadding;
			float neededHorizontalDistance = stageSize.x + cameraBoarderPadding;

			float orhographicsSize = cameraController.GetOrthographicSize();
			float targetOrtographicSizeX = neededVerticalDistance * orhographicsSize / verticalDistance;
			float targetOrtographicSizeY = neededHorizontalDistance * orhographicsSize / horizontalDistance;
			cameraController.SetOrthographicSize(Math.Max(targetOrtographicSizeX, targetOrtographicSizeY));
		}

		[GUIColor("@Color.forestGreen")]
		[HorizontalGroup("DEBUG/ENDGAME")]
		[Button]
		public void EndDungeonVictouris()
		{
			UIManager.Instance.ShowPopupMessage(
				"VICTORIOUS!",
				"You have successfully cleared the dungeon.",
				"Continue",
				() =>
				{
					UINavigator.Instance.ShowDungeonRoomSelectUI();
				});
		}

		[GUIColor("@Color.orangeRed")]
		[HorizontalGroup("DEBUG/ENDGAME")]
		[Button]
		public void EndDungeonFaliure()
		{
			UIManager.Instance.ShowPopupMessage(
				"DEFEAT!",
				"You have failed to clear the dungeon.",
				"Retry",
				() =>
				{
					UINavigator.Instance.ShowMainMenuUI();
				});
		}
	}
}
