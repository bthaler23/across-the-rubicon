using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
	public abstract class BaseElementUI : MonoBehaviour
	{
		protected bool isInitialized = false;
		protected bool isDisposed = false;

		[SerializeField]
		[BoxGroup("Animator")]
		protected Animator animator;
		[SerializeField]
		protected UILayoutGroupUpdate layoutUpdater;

		protected virtual void Awake()
		{

		}

		protected virtual void OnDestroy()
		{
			if (!isDisposed)
				Dispose();
		}

		public void Initialize()
		{
			if (isInitialized)
			{
				return;
			}
			InitializeActual();
			isInitialized = true;
			isDisposed = false;
		}

		protected virtual void InitializeActual()
		{
		}

		public virtual void Dispose()
		{
			isDisposed = true;
		}

		public virtual void Show()
		{
			Initialize();
			RefreshLayout();
			gameObject.SetActive(true);
		}

		protected void RefreshLayout()
		{
			if (layoutUpdater)
				layoutUpdater.MarkRefresh();
		}

		public virtual void Hide()
		{
			gameObject.SetActive(false);
		}

		public abstract bool IsVisible();

		public virtual void Refresh()
		{
			RefreshLayout();
		}

		protected IEnumerator DelayedAction(float delay, Action callback)
		{
			yield return new WaitForSecondsRealtime(delay);
			callback.Invoke();
		}
	}
}
