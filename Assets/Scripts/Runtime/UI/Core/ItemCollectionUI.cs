using Game.Data;
using GamePlugins.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
	[Serializable]
	public class ItemCollectionUI<T, D> where T : class where D : BaseCollectionElementUI<T>
	{
		[SerializeField]
		private D uiPrefab;
		[SerializeField]
		private RectTransform uiEleemntParent;

		[ShowInInspector, ReadOnly]
		private List<D> uiElements;

		public int DataIndexOf(D data)
		{
			if (uiElements == null || data == null)
				return -1;
			for (int i = 0; i < uiElements.Count; i++)
			{
				if (uiElements[i].Data == data)
					return i;
			}
			return -1;
		}

		public int IndexOf(object element)
		{
			if (uiElements == null || element == null)
				return -1;
			for (int i = 0; i < uiElements.Count; i++)
			{
				if (uiElements[i] == element)
					return i;
			}
			return -1;
		}

		public void Update(IReadOnlyList<T> collection, Action<object> onSelected, Action<D> onShowCustom)
		{
			if (collection.IsNullOrEmpty())
			{
				return;
			}

			int neededCount = collection.Count;
			uiElements ??= new List<D>();
			// Ensure we have enough UI widgets
			while (uiElements.Count < neededCount)
			{
				InstantiateUI();
			}

			// Populate and activate the required widgets
			for (int i = 0; i < neededCount; i++)
			{
				var ui = uiElements[i];
				var info = collection[i];
				ui.Show(info, onSelected);
				onShowCustom?.Invoke(ui);
			}

			// Deactivate any extra widgets
			for (int i = neededCount; i < uiElements.Count; i++)
			{
				uiElements[i].Hide();
			}
		}

		private D InstantiateUI()
		{
			var instance = GameObject.Instantiate(uiPrefab, uiEleemntParent);
			uiElements.Add(instance);
			return instance;
		}
	}
}
