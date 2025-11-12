using Game.Grid;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Gameplay
{
	public class PlayerInputController : MonoBehaviour
	{
		public event Action<Vector2Int> CellClicked;

		void Update()
		{
			bool mouseClick = Mouse.current.leftButton.wasReleasedThisFrame && InsideGameView();
			if (mouseClick)
				CellClicked?.Invoke(HexGridManager.Instance.MouseOnGridIndex);
		}

		private bool InsideGameView()
		{
			var mousePos = Mouse.current.position.value;
			return mousePos.x >= 0 &&
				mousePos.x <= Screen.width &&
				mousePos.y >= 0 &&
				mousePos.y <= Screen.height;
		}
	}
}
