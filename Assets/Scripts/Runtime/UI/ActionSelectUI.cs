using Game.Gameplay;
using GamePlugins.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class ActionSelectUI : MonoBehaviour
	{
		[SerializeField]
		private Button button;
		[SerializeField]
		private Image actionImage;
		[SerializeField]
		private GameObject selectionGO;

		private Action onSelectAction;

		private void Awake()
		{
			if (button != null)
			{
				button.onClick.AddListener(OnButtonClicked);
			}
		}

		private void OnButtonClicked()
		{
			onSelectAction?.Invoke();
		}

		public void Show(ITurnAction action, bool isActiveAction, Action onSelectAction)
		{
			gameObject.SetGameObjectActive(true);
			this.onSelectAction = onSelectAction;
			selectionGO.SetGameObjectActive(isActiveAction);
			actionImage.SetIconSafe(action.GetIcon());
		}

		public void Hide()
		{
			gameObject.SetGameObjectActive(false);
		}
	}
}
