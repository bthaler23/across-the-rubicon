using GamePlugins.Attributes;
using GamePlugins.Singleton;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Game.Grid
{
	[AutoCreateSingleton(false, false)]
	public class HexGridManager : Singleton<HexGridManager>
	{
		[SerializeField]
		private GridMapData gridMap;
		[SerializeField]
		private float hexRadius;
		[SerializeField]
		private HexGridDraw gridDrawer;
		[SerializeField]
		private Camera mainCamera;

		private Vector2Int screenCenterOnGridIndex;
		private Vector2 screenCenterOnGridPosition;
		private Vector2Int mouseOnGridIndex;
		private Vector3 mouseOnGridPosition;
		private Vector3 mousePlanePoistion;

		public Vector2 MousePlanePosition => mousePlanePoistion;
		public Vector2 MouseOnGridPosition => mouseOnGridPosition;
		public Vector2Int MouseOnGridIndex => mouseOnGridIndex;
		public Vector2 ScreenCenterOnGridPosition => screenCenterOnGridPosition;
		public Vector2Int ScreenCenterOnGridIndex => screenCenterOnGridIndex;
		public GridMapData GridMap { get => gridMap; }
		public float HexRadius { get => hexRadius; set => hexRadius = value; }

		protected override void OnAwakeCalled()
		{
			base.OnAwakeCalled();
			gridMap.Initialize();
		}

		public void Dispose()
		{
			gridMap.Dispose();
		}

		public void Update()
		{
			var screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
			mousePlanePoistion = GetPlaneIntersection(Mouse.current.position.value, 0, mainCamera);
			screenCenterOnGridPosition = ScreenPosToHexGridWorldPosition(screenCenter, mainCamera, hexRadius);
			gridDrawer.UpdateGridCenter(WorldToHexGrid(screenCenterOnGridPosition, hexRadius));
			screenCenterOnGridIndex = WorldToHexGrid(screenCenterOnGridPosition, hexRadius);

			mouseOnGridPosition = GetMouseOnHexGridPosition();
			mouseOnGridIndex = WorldToHexGrid(mouseOnGridPosition, hexRadius);
		}

		public Vector2Int WorldPositionToIndex(Vector2 worldPos)
		{
			return WorldToHexGrid(worldPos, hexRadius);
		}

		public Vector2 IndexToWordPosition(Vector2Int cellIndex)
		{
			return HexGridToWorld(cellIndex, hexRadius);
		}

		public Vector2 GetMouseOnHexGridPosition()
		{
			return ScreenPosToHexGridWorldPosition(Mouse.current.position.value, mainCamera, hexRadius);
		}

		public Vector2 HexGridToWorld(Vector2Int cellIndex)
		{
			return HexGridToWorld(cellIndex, hexRadius);
		}

		public Vector3 GetScreenSpacePlaneIntersection(Vector2 screenSpace)
		{
			return GetPlaneIntersection(screenSpace, 0, mainCamera);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			var pos = GetMouseOnHexGridPosition();
			Gizmos.DrawWireSphere(new Vector3(pos.x, 0, pos.y), .25f);
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(new Vector3(screenCenterOnGridPosition.x, 0, screenCenterOnGridPosition.y), .1f);
		}

		#region Grid Data Handling
		#endregion

		#region Grid Position Conversion
		public static Vector2 ScreenPosToHexGridWorldPosition(Vector3 screenPosition, Camera mainCamera, float gridSize)
		{
			var planeInterSection = GetPlaneIntersection(screenPosition, 0, mainCamera);
			var gridPos = WorldToHexGrid(new Vector2(planeInterSection.x, planeInterSection.z), gridSize);
			var worldPos = HexGridToWorld(gridPos, gridSize);
			//Debug.Log($"PlaneIntersetion {planeInterSection} Grid {gridPos} Word {worldPos}");
			return worldPos;
		}

		public static Vector2Int ScreenPosToHexGridPosition(Vector3 screenPosition, Camera mainCamera, float gridSize)
		{
			var planeInterSection = GetPlaneIntersection(screenPosition, 0, mainCamera);
			var gridPos = WorldToHexGrid(new Vector2(planeInterSection.x, planeInterSection.z), gridSize);
			return gridPos;
		}

		public static Vector3 GetPlaneIntersection(Vector3 screenPosition, float planYpos, Camera camera)
		{
			Ray ray = camera.ScreenPointToRay(screenPosition);
			float delta = ray.origin.y - planYpos;
			Vector3 dirNorm = ray.direction / ray.direction.y;
			Vector3 intersectionPosition = ray.origin - dirNorm * delta;
			return intersectionPosition;
		}

		public static Vector2Int WorldToHexGrid(Vector2 pos, float size)
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

		public static Vector2 HexGridToWorld(Vector2Int hex, float size)
		{
			float x = size * Mathf.Sqrt(3f) * (hex.x + hex.y / 2f);
			float y = size * 1.5f * hex.y;
			return new Vector2(x, y);
		}

		public static Transform GetPreviewParent()
		{
			if (instance)
			{
				return instance.transform;
			}
			return null;
		}
		#endregion
	}
}
