using UnityEngine;
namespace Game.Grid
{
	public abstract class IHexGrid : MonoBehaviour
	{
		public abstract void Initialize();
		public abstract Vector3 GridIndexToWordPosition(Vector2Int cellIndex);
		public abstract Vector2Int WordPositionToGridIndex(Vector2 cellIndex);
		public abstract Vector3 ScreenPositionToHexGridWorldPosition(Vector3 screenPosition, Camera mainCamera);
		public abstract Vector2Int ScreenPositionToHexGridIndex(Vector3 screenPosition, Camera mainCamera);
	}
}
