using GamePlugins.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class BaseCollectionElementUI<T> : MonoBehaviour where T : class
	{
		[SerializeField]
		protected Button button;
		protected Action<object> onSelected;
		protected T data;

		public T Data => data;

		protected virtual void Awake()
		{
			if (button)
				button.onClick.AddListener(OnButtonClicked);
		}

		private void OnButtonClicked()
		{
			onSelected?.Invoke(this);
		}

		public virtual void Show(T data, Action<object> onSelected)
		{
			this.data = data;
			this.onSelected = onSelected;
			gameObject.SetGameObjectActive(true);
		}

		public virtual void Hide()
		{
			gameObject.SetGameObjectActive(false);
		}
	}
}
