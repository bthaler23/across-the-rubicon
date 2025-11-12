using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Grid
{
	public class PlaneXZHexGrid : IHexGrid
	{
		[SerializeField]
		private float hexRadius;
		[SerializeField]
		private PlaneXZHexGridDraw gridDrawer;

		public float HexRadius { get => hexRadius; set => hexRadius = value; }

		public override void Initialize()
		{
		}

		public override Vector2Int WordPositionToGridIndex(Vector2 worldPosition)
		{
			return HexGridWorldToHexGridIndex(worldPosition, hexRadius);
		}

		public override Vector3 GridIndexToWordPosition(Vector2Int cellIndex)
		{
			var worldPos = HexGridIndexToHexGridWorld(cellIndex, hexRadius);
			return new Vector3(worldPos.x,0, worldPos.y);
		}

		public override Vector3 ScreenPositionToHexGridWorldPosition(Vector3 screenPosition, Camera mainCamera)
		{
			var worldPos = ScreenPosToHexGridWorldPosition(screenPosition, mainCamera, hexRadius);
			return new Vector3(worldPos.x,0, worldPos.y);
		}

		public override Vector2Int ScreenPositionToHexGridIndex(Vector3 screenPosition, Camera mainCamera)
		{
			return ScreenPosToHexGridIndex(screenPosition, mainCamera, hexRadius);
		}

		#region Grid Position Conversion
		public static Vector2 ScreenPosToHexGridWorldPosition(Vector3 screenPosition, Camera mainCamera, float gridSize)
		{
			var planeInterSection = GetPlaneIntersection(screenPosition, 0, mainCamera);
			var gridPos = HexGridWorldToHexGridIndex(new Vector2(planeInterSection.x, planeInterSection.z), gridSize);
			var worldPos = HexGridIndexToHexGridWorld(gridPos, gridSize);
			return worldPos;
		}

		public static Vector2Int ScreenPosToHexGridIndex(Vector3 screenPosition, Camera mainCamera, float gridSize)
		{
			var planeInterSection = GetPlaneIntersection(screenPosition, 0, mainCamera);
			return HexGridWorldToHexGridIndex(new Vector2(planeInterSection.x, planeInterSection.z), gridSize);
		}

		public static Vector3 GetPlaneIntersection(Vector3 screenPosition, float planYpos, Camera camera)
		{
			Ray ray = camera.ScreenPointToRay(screenPosition);
			float delta = ray.origin.y - planYpos;
			Vector3 dirNorm = ray.direction / ray.direction.y;
			Vector3 intersectionPosition = ray.origin - dirNorm * delta;
			return intersectionPosition;
		}

		public static Vector2Int HexGridWorldToHexGridIndex(Vector2 pos, float size)
		{
			float q = (Mathf.Sqrt(3f) / 3f * pos.x - 1f / 3f * pos.y) / size;
			float r = (2f / 3f * pos.y) / size;

			float x = q;
			float z = r;
			float y = -x - z;

			int rx = Mathf.RoundToInt(x);
			int ry = Mathf.RoundToInt(y);
			int rz = Mathf.RoundToInt(z);

			float dx = Mathf.Abs(rx - x);
			float dy = Mathf.Abs(ry - y);
			float dz = Mathf.Abs(rz - z);

			if (dx > dy && dx > dz) rx = -ry - rz;
			else if (dy > dz) ry = -rx - rz;
			else rz = -rx - ry;

			return new Vector2Int(rx, rz); // axial (q, r)
		}

		public static Vector2 HexGridIndexToHexGridWorld(Vector2Int hex, float size)
		{
			float x = size * Mathf.Sqrt(3f) * (hex.x + hex.y / 2f);
			float y = size * 1.5f * hex.y;
			return new Vector2(x, y);
		}
		#endregion
	}
}
