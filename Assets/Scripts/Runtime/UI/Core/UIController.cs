using Game.UI;
using GamePlugins.Attributes;
using GamePlugins.Singleton;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.UI
{

	[AutoCreateSingleton(false, false)]
	public class UIController : Singleton<UIController>
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
			UIManager.Instance.Open(mainMenuUI);
		}

		public void ShowDungeonSelectUI()
		{
			dungeonSelectUI.PopulateDungeons(GameManager.Instance.GameplaySettings.DungeonInfos);
			UIManager.Instance.Open(dungeonSelectUI);
		}

		public void ShowCharacterSelectUI()
		{
			UIManager.Instance.Open(characterSelectUI);
		}
	}
}
