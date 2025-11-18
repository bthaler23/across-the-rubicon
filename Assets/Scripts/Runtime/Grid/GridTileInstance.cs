using GamePlugins.Utils;
using System;
using UnityEngine;

namespace Game
{
	public class GridTileInstance : MonoBehaviour
	{
		[SerializeField]
		private SpriteRenderer gridDefaultSpriteRenderer;
		[SerializeField]
		private SpriteRenderer selectionSpriteRenderer;
		[SerializeField]
		private Color defaultColor;
		[SerializeField]
		private Color highlightColor;

		public void SetHighlightColor()
		{
			SetHighlightColor(highlightColor);
		}

		public void SetHighlightColor(Color color)
		{
			gridDefaultSpriteRenderer.color = color;
		}

		public void ResetColor()
		{
			gridDefaultSpriteRenderer.color = defaultColor;
			selectionSpriteRenderer.SetGameObjectActive(false);
			gridDefaultSpriteRenderer.SetGameObjectActive(true);
		}

		public void SetSelected(Color color)
		{
			gridDefaultSpriteRenderer.SetGameObjectActive(false);
			selectionSpriteRenderer.SetGameObjectActive(true);
			selectionSpriteRenderer.color = color;
		}
	}
}
