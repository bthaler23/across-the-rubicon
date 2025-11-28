using Game.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class PopupMessageUI : BaseViewUI
	{

		#region Fields
		public TextMeshProUGUI title;
		public TextMeshProUGUI message;

		public Button buttonA;
		public TextMeshProUGUI buttonAText;
		public Button buttonB;
		public TextMeshProUGUI buttonBText;

		private Action actionA;
		private Action actionB;
		#endregion

		#region Properties

		#endregion

		#region Events

		#endregion

		#region MonoBehaviour
		protected override void Awake()
		{
			base.Awake();
			buttonA.onClick.AddListener(() => ButtonAClick());
			buttonB.onClick.AddListener(() => ButtonBClick());
		}
		#endregion

		#region Methods
		//Order: Static > Abstract > Virtual > Override > Simple Methods > Eventhandlers
		public void ShowPopup(string title, string message, string buttonA, Action actionA, string buttonB = null, Action actionB = null)
		{
			this.title.text = title;
			this.message.text = message;

			this.buttonAText.text = buttonA;
			this.actionA = actionA;

			if (!string.IsNullOrEmpty(buttonB) && actionB != null)
			{
				this.buttonBText.text = buttonB;
				this.actionB = actionB;
				this.buttonB.gameObject.SetActive(true);
			}
			else
			{
				this.buttonB.gameObject.SetActive(false);
			}
			gameObject.SetActive(true);
		}

		private void ButtonAClick()
		{
			UIManager.Instance.Close(this);
			actionA?.Invoke();
		}

		private void ButtonBClick()
		{
			UIManager.Instance.Close(this);
			actionB?.Invoke();
		}
		#endregion
	}
}
