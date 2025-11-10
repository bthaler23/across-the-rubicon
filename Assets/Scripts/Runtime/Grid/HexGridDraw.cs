using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Grid
{
	public class HexGridDraw : MonoBehaviour
	{
		[SerializeField]
		private bool enableDraw = true;
		[SerializeField]
		private Vector2Int gridCenterIndex = Vector2Int.zero;
		[SerializeField]
		private float hexRadius = 1;
		[SerializeField]
		private int gridHeight;
		[SerializeField]
		private int gridWidth;
		[SerializeField]
		private float gridDrawRadius = 0;
		[SerializeField]
		private bool drawCenter = true;
		[SerializeField]
		private bool drawHexagon = true;
		[SerializeField]
		private float drawAngleOffset;
		[SerializeField]
		private Color gridColor;
		[SerializeField]
		private bool placeObjects;
		[SerializeField]
		private Vector3 positionOffset;
		[SerializeField]
		private GameObject gridPrefab;
		[SerializeField]
		private bool drawArea = false;

		[SerializeField]
		private List<GameObject> gridObjectsCache;

		private HashSet<Vector2Int> positionIndexes;

		private void Update()
		{
			if (placeObjects)
				DrawGridAssets();
		}

		public void UpdateGridCenter(Vector2Int gridCoordinates)
		{
			gridCenterIndex = gridCoordinates;
			positionIndexes = null;
		}

		public void UpdateGridCenter(Vector2Int gridCoordinates, HashSet<Vector2Int> positions)
		{
			gridCenterIndex = gridCoordinates;
			this.positionIndexes = positions;
		}

		public void ToggleDraw(bool value)
		{
			drawHexagon = value;
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = gridColor;
			if (!placeObjects)
				DrawGridGizmos();
		}

		public void DrawGridAssets()
		{
			if (drawHexagon && enableDraw)
			{
				gridObjectsCache ??= new List<GameObject>();

				int objectCache = 0;
				List<Vector2> positions = GetGridPositions();
				foreach (var pos in positions)
				{
					DrawHexAsset(pos, ref objectCache);
				}
			}
			else
			{
				ToggleObjects(false);
			}
		}

		public void DrawGridGizmos()
		{
			if (drawHexagon && enableDraw)
			{
				gridObjectsCache ??= new List<GameObject>();

				int objectCache = 0;
				List<Vector2> positions = GetGridPositions();
				foreach (var pos in positions)
				{
					DrawHexGizmos(pos, ref objectCache);
				}
			}
			else
			{
				ToggleObjects(false);
			}
		}

		private List<Vector2> GetGridPositions()
		{
			if (positionIndexes == null || drawArea)
			{
				return GetAreaDrawPositions();
			}
			else
			{
				List<Vector2> position = new List<Vector2>();
				foreach (var p in positionIndexes)
				{
					position.Add(HexGridManager.HexGridToWorld(p, hexRadius));
				}
				return position;
			}
		}

		private List<Vector2> GetAreaDrawPositions()
		{
			List<Vector2> hexPositions = new List<Vector2>();

			Vector2 gridCenter = HexGridManager.HexGridToWorld(gridCenterIndex, hexRadius);
			float hexWidth = Mathf.Sqrt(3) * hexRadius;
			float hexHeight = hexRadius * 2f;

			float xSpacing = hexWidth;
			float zSpacing = 3 / 2f * hexRadius;

			int gridWidthHalf = gridWidth / 2;
			int gridHeightHalf = gridHeight / 2;

			for (int x = gridCenterIndex.x - gridWidthHalf; x < gridCenterIndex.x + gridWidthHalf; x++)
			{
				for (int y = gridCenterIndex.y - gridHeightHalf; y < gridCenterIndex.y + gridHeightHalf; y++)
				{
					Vector2Int position = new Vector2Int(x, y);
					Vector2 worldPos = HexGridManager.HexGridToWorld(position, hexRadius);
					bool enableDraw =
						gridDrawRadius == 0
						|| gridDrawRadius * gridDrawRadius >= (worldPos - gridCenter).sqrMagnitude;

					if (enableDraw)
					{
						hexPositions.Add(worldPos);
					}
				}
			}
			return hexPositions;
		}

		private void DrawHexAsset(Vector2 worldPos, ref int objectcacheIndex)
		{
			Vector3 worldPos3D = new Vector3(worldPos.x, 0, worldPos.y);
			if (drawHexagon)
			{
				PlaceObject(objectcacheIndex, worldPos3D);
				objectcacheIndex++;
			}
			else
			{
				ToggleObjects(false);
			}
		}

		private void DrawHexGizmos(Vector2 worldPos, ref int objectcacheIndex)
		{
			Vector3 worldPos3D = new Vector3(worldPos.x, 0, worldPos.y);
			if (drawHexagon)
			{
				DrawHex(worldPos3D, hexRadius, drawAngleOffset);
			}
			if (drawCenter)
				Gizmos.DrawWireSphere(worldPos3D, hexRadius / 5f);
		}

		private void ToggleObjects(bool v)
		{
			foreach (var i in gridObjectsCache)
			{
				i.gameObject.SetActive(v);
			}
		}

		private void PlaceObject(int cacheIndex, Vector3 worldPos3D)
		{
			GameObject gridObject = null;
			if (gridObjectsCache.Count > cacheIndex)
			{
				gridObject = gridObjectsCache[cacheIndex];
			}
			else
			{
				gridObject = Instantiate(gridPrefab, transform);
				gridObjectsCache.Add(gridObject);
			}
			gridObject.SetActive(true);
			gridObject.transform.position = worldPos3D + positionOffset;
		}

		static void DrawHex(Vector3 center, float radius, float drawAngleOffset)
		{
			for (int i = 0; i < 6; i++)
			{
				float angle1 = Mathf.Deg2Rad * (drawAngleOffset + (60 * i));
				float angle2 = Mathf.Deg2Rad * (drawAngleOffset + (60 * (i + 1)));
				Vector3 p1 = center + new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1)) * radius;
				Vector3 p2 = center + new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2)) * radius;
				Gizmos.DrawLine(p1, p2);
			}
		}
	}
}
