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
			var actionIcon = action.GetIcon();
			gameObject.SetGameObjectActive(true);
			this.onSelectAction = onSelectAction;
			selectionGO.SetGameObjectActive(isActiveAction);

			bool hasIcon = actionIcon != null;
			actionImage.SetGameObjectActive(hasIcon);
			actionLabel.SetGameObjectActive(!hasIcon);

			actionImage.SetIconSafe(actionIcon);
			actionLabel.SetTextSafe(action.GetName());
		}

		public void Hide()
		{
			gameObject.SetGameObjectActive(false);
		}
	}
}
