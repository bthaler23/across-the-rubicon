using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Grid
{
	public class TilemapHexGrid : IHexGrid
	{
		[SerializeField]
		private Tilemap unityTilemap;

		public override void Initialize()
		{
		}

		public override Vector3 GridIndexToWordPosition(Vector2Int cellIndex)
		{
			return unityTilemap.CellToWorld(new Vector3Int(cellIndex.x, cellIndex.y));
		}

		public override Vector2Int WordPositionToGridIndex(Vector2 worldPos)
		{
			return (Vector2Int)unityTilemap.WorldToCell(worldPos);
		}

		public override Vector2Int ScreenPositionToHexGridIndex(Vector3 screenPosition, Camera mainCamera)
		{
			Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPosition);
			return WordPositionToGridIndex(worldPos);
		}

		public override Vector3 ScreenPositionToHexGridWorldPosition(Vector3 screenPosition, Camera mainCamera)
		{
			Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPosition);
			var gridIndex = WordPositionToGridIndex(worldPos);
			return GridIndexToWordPosition(gridIndex);
		}
	}
}
