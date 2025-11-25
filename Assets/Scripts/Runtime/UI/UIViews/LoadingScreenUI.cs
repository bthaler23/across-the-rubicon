using Game.UI;
using GamePlugins.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

namespace Game.UI
{
	/// <summary>
	/// Loading screen UI view
	/// </summary>
	public class LoadingScreenUI : MonoBehaviour
	{
		[SerializeField]
		private Animator animator;
		[SerializeField]
		private float transitionTime;

		public bool IsShowing()
		{
			return gameObject.activeSelf;
		}

		[Button]
		public void Show(bool instant = false)
		{
			gameObject.SetActive(true);
			if (!instant)
			{
				animator.PlayAnimationSafe("Show");
			}
			else
			{
				animator.PlayAnimationSafe("Idle");
			}
		}

		[Button]
		public void Hide()
		{
			if (gameObject.activeSelf)
			{
				animator.PlayAnimationSafe("Hide");
				StartCoroutine(DelayedDeactivate(transitionTime));
			}
		}

		IEnumerator DelayedDeactivate(float time)
		{
			yield return new WaitForSeconds(time);
			gameObject.SetActive(false);
		}

		public bool IsVisible()
		{
			return gameObject.activeSelf;
		}
	}
}