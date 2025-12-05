using Game.Gameplay;
using GamePlugins.Utils;
using System;
using TMPro;
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
		[SerializeField]
		private TextMeshProUGUI actionLabel;

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

		public void Show(TurnActionBase action, bool isActiveAction, Action onSelectAction)
		{
			var actionInfo = action.ActionInfo;
			gameObject.SetGameObjectActive(true);
			this.onSelectAction = onSelectAction;
			selectionGO.SetGameObjectActive(isActiveAction);

			bool hasIcon = actionInfo.Icon != null;
			actionImage.SetGameObjectActive(hasIcon);
			actionLabel.SetGameObjectActive(!hasIcon);

			actionImage.SetIconSafe(actionInfo.Icon);
			actionLabel.SetTextSafe(actionInfo.ActionName);
		}

		public void Hide()
		{
			gameObject.SetGameObjectActive(false);
		}
	}
}
