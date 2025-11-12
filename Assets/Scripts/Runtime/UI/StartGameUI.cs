using Game.Gameplay;
using GamePlugins.Utils;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Game.UI
{
	public class StartGameUI : MonoBehaviour
	{
		[SerializeField]
		private GameplayController gameplayController;
		[SerializeField]
		private Button startGameButton;

		private void Awake()
		{
			startGameButton.onClick.AddListener(OnStartGameButtonClicked);
		}

		private void OnStartGameButtonClicked()
		{
			gameplayController.InitializeGameplay();
			gameObject.SetGameObjectActive(false);
		}
	}
}
