using Game.Character;
using Game.Data;
using Game.Events;
using Game.Grid;
using Game.Progress;
using Game.Settings;
using Game.UI;
using GamePlugins.Events;
using GamePlugins.ObjectPool;
using GamePlugins.Singleton;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using CharacterBehaviour = Game.Character.CharacterBehaviour;

namespace Game.Gameplay
{
	public class GameplayController : Singleton<GameplayController>
	{
		private const string HERO_TEAM = "Heroes";
		private const string ENEMY_TEAM = "Foes";

		[SerializeField]
		private RandomDirectionFadeWidget floatingTextUIPrefab;
		[SerializeField]
		private CameraController cameraController;

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
		private List<CharacterBehaviour> actors;
		[ShowInInspector, ReadOnly]
		private List<TeamActors> teams;

		protected override void OnAwakeCalled()
		{
			base.OnAwakeCalled();
			ResourceManager.Instance.RegisterResource<CameraController>(cameraController);
			EventBus.Subscribe<OnGameEndedEvent>(OnGameEnded);
			EventBus.Subscribe<OnShowFloatingUiText>(ShowFloatingUIText);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			ResourceManager.Instance.RemoveResource<CameraController>();
			EventBus.Unsubscribe<OnGameEndedEvent>(OnGameEnded);
			EventBus.Unsubscribe<OnShowFloatingUiText>(ShowFloatingUIText);
		}

		public void InitializeGameplay()
		{
			ObjectPool.Instance.CachePrefab(floatingTextUIPrefab.gameObject, UINavigator.Instance.UiObjectPoolParent, 10);
			actors = new List<CharacterBehaviour>();
			gridExpansionController.InitializeGrid(ProgressManager.Instance.CurrentDungeonRoom.GridSetup);
			teams = GetTeams();
			gameFlowManager.Inintialize(teams);
			cameraController.CenterCameraOnDungeon(gridManager.GetGridCenter(), gridManager.GetGridSize());
		}

		public CharacterBehaviour GetActorAt(Vector2Int index)
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

		private CharacterBehaviour SpawnCharacter(Vector2Int gridPosition, CharacterSetupData characterSetup, TeamInfo team)
		{
			CharacterBehaviour character = Instantiate<CharacterBehaviour>(characterSetup.CharacterInfo.CharacterPrefab);
			character.Initialize(characterSetup.CharacterInfo, characterSetup.EquipmentData, playerInput, team);
			character.Move(gridPosition);
			actors.Add(character);
			return character;
		}

		private List<TeamActors> GetTeams()
		{
			List<TeamInfo> teams = new List<TeamInfo>();

			var gameplaySettings = ResourceManager.Instance.RequestResource<GameplaySettings>();

			TeamInfo playerTeam = new TeamInfo(HERO_TEAM, gameplaySettings.HeroTeamColor);
			foreach (var character in ProgressManager.Instance.CurrentHeroes)
			{
				playerTeam.AddCharacter(character);
			}
			TeamInfo enemyTeam = new TeamInfo(ENEMY_TEAM, gameplaySettings.EnemyTeamColor);
			foreach (var character in ProgressManager.Instance.CurrentDungeonRoom.EnemyActors)
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

		private bool IsPlayerTeamAlive()
		{
			foreach (var team in teams)
			{
				if (team.TeamID == HERO_TEAM)
				{
					return team.HasAliveMembers();
				}
			}
			return false;
		}

		private void ShowFloatingUIText(OnShowFloatingUiText eventParams)
		{
			var floatingUI = ObjectPool.GetObject<RandomDirectionFadeWidget>(floatingTextUIPrefab);
			floatingUI.ShowText(eventParams.worldTarget, cameraController.Camera, 1f, eventParams.text);
		}

		private void OnGameEnded(OnGameEndedEvent @event)
		{
			bool playerTeamWon = IsPlayerTeamAlive();
			if (playerTeamWon)
			{
				EndDungeonVictouris();
			}
			else
			{
				EndDungeonFaliure();
			}
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
