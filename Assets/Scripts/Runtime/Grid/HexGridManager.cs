using GamePlugins.Attributes;
using GamePlugins.Singleton;
using GamePlugins.Utils;
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
		private IHexGrid hexGrid;

		[SerializeField]
		private Camera mainCamera;

		private Vector2Int mouseOnGridIndex;
		public Vector2Int MouseOnGridIndex => mouseOnGridIndex;

		public GridMapData GridMap { get => gridMap; }

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
			foundation.transform.position = new Vector3(worldPos.x, worldPos.y);
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

		public List<Vector2Int> GetMovementRangePositions(Vector2Int currentPosition, int movementRange)
		{
			// BFS traversal limited by movementRange steps, constrained to existing grid cells.
			var result = new List<Vector2Int>();
			if (gridMap == null || movementRange <= 0) return result;

			// Determine which positions are valid (present on the grid)
			// Prefer ContentCells if available, else use GridMap keys.
			HashSet<Vector2Int> validPositions = gridMap.ContentCells != null && gridMap.ContentCells.Count > 0
				? gridMap.ContentCells
				: new HashSet<Vector2Int>(gridMap.GridMap.Keys);

			if (!validPositions.Contains(currentPosition)) return result; // origin not on map

			// Neighbor offsets (axial directions) match HexGridHelper.NeighbourOffsetMap
			var directions = HexGridHelper.NeighbourOffsetMap.Values;

			var visited = new HashSet<Vector2Int> { currentPosition };
			var frontier = new Queue<(Vector2Int pos, int dist)>();
			frontier.Enqueue((currentPosition, 0));

			while (frontier.Count > 0)
			{
				var (pos, dist) = frontier.Dequeue();
				if (dist == movementRange) continue; // can't expand further

				foreach (var offset in directions)
				{
					var next = pos + offset;
					if (!validPositions.Contains(next)) continue; // skip missing cells
					if (visited.Contains(next)) continue;
					visited.Add(next);
					int nextDist = dist + 1;
					if (nextDist <= movementRange)
					{
						result.Add(next);
						frontier.Enqueue((next, nextDist));
					}
				}
			}

			return result;
		}

		internal void HighlightPositions(List<Vector2Int> possibleMovementPositions)
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
