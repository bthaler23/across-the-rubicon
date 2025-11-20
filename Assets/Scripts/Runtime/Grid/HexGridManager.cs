using GamePlugins.Attributes;
using GamePlugins.Singleton;
using GamePlugins.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

namespace Game.Grid
{
	[AutoCreateSingleton(false, false)]
	public class HexGridManager : Singleton<HexGridManager>
	{
		[SerializeField]
		private GridMapData gridMap;

		[SerializeField]
		private IHexGrid hexGrid;

		[SerializeField]
		private Camera mainCamera;

		private Vector2Int mouseOnGridIndex;
		public Vector2Int MouseOnGridIndex => mouseOnGridIndex;

		public GridMapData GridMap { get => gridMap; }
		public IHexGrid Grid { get => hexGrid; }

		public static Transform GetPreviewParent()
		{
			if (instance != null)
				return instance.transform;
			return null;
		}

		#region Monobehaviour
		protected override void OnAwakeCalled()
		{
			base.OnAwakeCalled();
			gridMap.Initialize();
			hexGrid.Initialize();
		}

		public void Update()
		{
			mouseOnGridIndex = hexGrid.ScreenPositionToHexGridIndex(Mouse.current.position.value, mainCamera);
		}

		private void OnDrawGizmos()
		{

		}
		#endregion

		internal void AddTile(Vector2Int position, GridTileInstance foundation)
		{
			var worldPos = hexGrid.GridIndexToWordPosition(position);
			foundation.transform.position = worldPos;
			foundation.SetGameObjectActive(true);
			GridMap.AddTile(position, foundation);
		}

		public void Dispose()
		{
			gridMap.Dispose();
		}

		public Vector2 GridIndexToWordPosition(Vector2Int cellIndex)
		{
			return hexGrid.GridIndexToWordPosition(cellIndex);
		}

		public List<Vector2Int> GetMovementRangePositions(Vector2Int currentPosition, int range)
		{
			// Compute positions using axial hex distance metric (q,r) to avoid off-by-one leaks.
			var result = new List<Vector2Int>();
			if (gridMap == null || range <= 0)
				return result;

			HashSet<Vector2Int> validPositions = gridMap.ContentCells != null && gridMap.ContentCells.Count > 0
				? gridMap.ContentCells
				: new HashSet<Vector2Int>(gridMap.GridMap.Keys);

			var neighbourMap = HexGridHelper.GetPositionsInRange(currentPosition, range);

			foreach (var pos in neighbourMap)
			{
				if (validPositions.Contains(pos))
				{
					result.Add(pos);
				}
			}

			//Debug.Log($"GetMovementRangePositions from {currentPosition} with range {range}, found {result.Count} positions.");
			return result;
		}


		internal void HighlightPositions(Vector2Int selectedTilePosition, Color selectionColor, List<Vector2Int> possibleMovementPositions, Color highlightColor)
		{
			if (possibleMovementPositions == null || gridMap == null) return;
			foreach (var pos in possibleMovementPositions)
			{
				if (!gridMap.ContentCells.Contains(pos)) continue;
				var cell = gridMap.GetCell(pos);
				if (cell?.TileInstance != null)
				{
					cell.TileInstance.SetHighlightColor(highlightColor);
				}
			}

			var selectedCell = gridMap.GetCell(selectedTilePosition);
			if (selectedCell?.TileInstance != null)
			{
				selectedCell.TileInstance.SetSelected(selectionColor);
			}
		}

		internal void ResetPositions()
		{
			if (gridMap == null) return;
			foreach (var pos in gridMap.ContentCells)
			{
				var cell = gridMap.GetCell(pos);
				if (cell?.TileInstance != null)
				{
					cell.TileInstance.ResetColor();
				}
			}
		}

		public Vector2 GetGridCenter()
		{
			// Center of bounding rectangle that includes all content cells (in world coordinates)
			if (gridMap == null || gridMap.ContentCells == null || gridMap.ContentCells.Count == 0)
				return Vector2.zero;

			GetGridBorders(out float minX, out float maxX, out float minY, out float maxY);

			return new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
		}

		public Vector2 GetGridSize()
		{
			// Size (width,height) of bounding rectangle that includes all content cells (in world coordinates)
			if (gridMap == null || gridMap.ContentCells == null || gridMap.ContentCells.Count == 0)
				return Vector2.zero;

			GetGridBorders(out float minX, out float maxX, out float minY, out float maxY);

			return new Vector2(maxX - minX, maxY - minY);
		}

		private void GetGridBorders(out float minX, out float maxX, out float minY, out float maxY)
		{
			minX = float.PositiveInfinity;
			maxX = float.NegativeInfinity;
			minY = float.PositiveInfinity;
			maxY = float.NegativeInfinity;

			foreach (var cellIndex in gridMap.ContentCells)
			{
				Vector3 wp3 = hexGrid.GridIndexToWordPosition(cellIndex);
				float axisY = Mathf.Abs(wp3.y) > 0.0001f ? wp3.y : wp3.z;
				minX = Mathf.Min(minX, wp3.x);
				maxX = Mathf.Max(maxX, wp3.x);
				minY = Mathf.Min(minY, axisY);
				maxY = Mathf.Max(maxY, axisY);
			}
		}
	}
}
