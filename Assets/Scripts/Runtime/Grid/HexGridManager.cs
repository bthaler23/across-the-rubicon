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


		internal void HighlightPositions(Vector2Int selectedTilePosition, List<Vector2Int> possibleMovementPositions)
		{
			if (possibleMovementPositions == null || gridMap == null) return;
			foreach (var pos in possibleMovementPositions)
			{
				if (!gridMap.ContentCells.Contains(pos)) continue;
				var cell = gridMap.GetCell(pos);
				if (cell?.TileInstance != null)
				{
					cell.TileInstance.SetHighlightColor();
				}
			}

			var selectedCell = gridMap.GetCell(selectedTilePosition);
			if (selectedCell?.TileInstance != null)
			{
				selectedCell.TileInstance.SetSelected();
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
	}
}
