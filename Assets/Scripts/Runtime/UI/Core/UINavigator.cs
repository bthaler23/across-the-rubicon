using Game.Data;
using Game.Progress;
using Game.UI;
using GamePlugins.Attributes;
using GamePlugins.Singleton;
using GamePlugins.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{

	[AutoCreateSingleton(false, false)]
	public class UINavigator : Singleton<UINavigator>
	{
		[TitleGroup("UI Pages")]
		[SerializeField]
		private MainMenuUI mainMenuUI;
		[TitleGroup("UI Pages")]
		[SerializeField]
		private DungeonSelectUI dungeonSelectUI;
		[TitleGroup("UI Pages")]
		[SerializeField]
		private CharacterSelectUI characterSelectUI;
		[TitleGroup("UI Pages")]
		[SerializeField]
		private DungeonRoomSelectUI dungeonRoomSelectUI;

		public void Initialize()
		{
			mainMenuUI.Initialize();
		}

		public void ShowMainMenuUI()
		{
			ProgressManager.Instance.ResetProgress();
			UIManager.Instance.Open(mainMenuUI);
		}

		public void ShowDungeonRoomSelectUI()
		{
			UIManager.Instance.Open(dungeonRoomSelectUI);
		}

		public void ShowDungeonSelectUI()
		{
			dungeonSelectUI.PopulateDungeons(GameManager.Instance.GameplaySettings.DungeonInfos);
			UIManager.Instance.Open(dungeonSelectUI); 
		}

		public void SelectDungeon(DungeonInfo dungeon)
		{
			if (dungeon == null) return;

			ProgressManager.Instance.SelectDungeon(dungeon);
			UIManager.Instance.Open(characterSelectUI);
		}

		public void SelectCharacter(List<ActorInfo> characters)
		{
			if (characters.IsNullOrEmpty()) return;

			ProgressManager.Instance.SelectCharacters(characters);
			ShowDungeonRoomSelectUI();
		}

		public void SelectDungeonRoom(DungeonRoomInfo data)
		{
			ProgressManager.Instance.SelectDungeonRoom(data);
			GameManager.Instance.LoadGameplay();
		}
	}
}
