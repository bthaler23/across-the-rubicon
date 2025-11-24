using Game.Gameplay;
using GamePlugins.Utils;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Game.UI
{
	public class StartGameUI : BaseViewUI
	{
		[SerializeField]
		private Button startGameButton;

		protected override void Awake()
		{
			base.Awake();
			startGameButton.onClick.AddListener(OnStartGameButtonClicked);
		}

		private void OnStartGameButtonClicked()
		{
			GameplayController.Instance.InitializeGameplay();
			gameObject.SetGameObjectActive(false);
		}
	}
}
