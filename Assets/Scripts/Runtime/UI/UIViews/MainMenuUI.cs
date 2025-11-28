using Game.Gameplay;
using GamePlugins.Utils;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Game.UI
{
	public class MainMenuUI : BaseViewUI
	{
		[Title("Main Menu")]
		[SerializeField]
		private Button startGameButton;
		[SerializeField]
		private Button settingsButton;
		[SerializeField]
		private Button quitButton;

		protected override void Awake()
		{
			base.Awake();
			startGameButton.onClick.AddListener(OnStartGameButtonClicked);
			settingsButton.onClick.AddListener(OnSettingsButtonClicked);
			quitButton.onClick.AddListener(OnQuitButtonClicked);
		}

		private void OnStartGameButtonClicked()
		{
			UINavigator.Instance.ShowDungeonSelectUI();
			//GameplayController.Instance.InitializeGameplay();
		}

		private void OnQuitButtonClicked()
		{
			GameManager.Instance.QuitGame();
		}

		private void OnSettingsButtonClicked()
		{
			throw new NotImplementedException();
		}
	}
}
