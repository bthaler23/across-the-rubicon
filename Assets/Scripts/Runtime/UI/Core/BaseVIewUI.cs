using Game.UI;
using GamePlugins.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Game.UI
{
	/// <summary>
	/// Base class for all UI views (pages, popups)
	/// </summary>
	public abstract class BaseViewUI : BaseElementUI
	{
		public enum ViewType
		{
			Page,
			Popup,
		}

		[SerializeField]
		private Button backButton;
		[SerializeField]
		private float timeoutBeforeManualClose = 0;
		[SerializeField]
		private ViewType type = ViewType.Page;
		[BoxGroup("Animator")]
		[SerializeField, ShowIf("@this.animator!=null")]
		[BoxGroup("Animator")]
		private float hideDelay = 0;
		[SerializeField, ShowIf("@this.animator!=null")]
		[BoxGroup("Animator")]
		private string showAnimationName = "Show";
		[SerializeField, ShowIf("@this.animator!=null")]
		[BoxGroup("Animator")]
		private string hideAnimationName = "Hide";

		private float closeTimeut = 0;

		private bool internalShowState = false;

		IEnumerator hideDelayedCO;

		public ViewType Type => type;

		public virtual bool CanClose => closeTimeut <= 0;

		protected override void Awake()
		{
			base.Awake();
			if (backButton)
				backButton.onClick.AddListener(Close);
			//Views are opend using UIManager so they should start closed
			if (!internalShowState)
				gameObject.SetActive(false);
		}

		protected virtual void Update()
		{
			if (closeTimeut > 0)
			{
				if (Time.timeScale == 0)
					closeTimeut -= Time.unscaledDeltaTime;
				else
					closeTimeut -= Time.deltaTime;
			}
		}

		public override void Show()
		{
			if (!internalShowState && gameObject.activeSelf)
			{
				gameObject.SetActive(false);//this will force unity to replay animation
			}

			internalShowState = true;
			base.Show();
			closeTimeut = timeoutBeforeManualClose;
			PlayShowAnimation();
			Refresh();
		}

		private bool HasAnimation()
		{
			return animator != null;
		}

		private void PlayShowAnimation()
		{
			if (animator)
			{
				animator.Play(showAnimationName);
			}
		}

		private void PlayHideAnimation()
		{
			if (animator)
			{
				animator.Play(hideAnimationName);
			}
		}

		public override void Hide()
		{
			internalShowState = false;
			if (!HasAnimation())
			{
				base.Hide();
			}
			else
			{
				PlayHideAnimation();
				hideDelayedCO = DelayedAction(hideDelay,
					() =>
					{
						if (!internalShowState)
							gameObject.SetActive(false);
					});
				StartCoroutine(hideDelayedCO);
			}
		}

		public override bool IsVisible()
		{
			return gameObject.activeSelf;
		}

		public void Close()
		{
			if (CanClose)
				UIManager.Instance.Close(this);
		}

		public void CloseForced()
		{
			UIManager.Instance.Close(this);
		}
	}
}
