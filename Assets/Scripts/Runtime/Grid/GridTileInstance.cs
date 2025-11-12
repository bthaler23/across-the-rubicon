using UnityEngine;

namespace Game.Grid
{
	public class GridTileInstance : MonoBehaviour
	{
		[SerializeField]
		private SpriteRenderer spriteRenderer;
		[SerializeField]
		private Color defaultColor;
		[SerializeField]
		private Color highlightColor;

		public void SetHighlightColor()
		{
			spriteRenderer.color = highlightColor;
		}

		public void ResetColor()
		{
			spriteRenderer.color = defaultColor;
		}
	}
}
