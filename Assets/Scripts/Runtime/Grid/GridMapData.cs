using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Game.Grid
{
	public class GridMapData : MonoBehaviour
	{
		[SerializeField]
		private HashSet<Vector2Int> contentCells;
		[SerializeField]
		private SerializedDictionary<Vector2Int, GridCell> gridMap;

		public HashSet<Vector2Int> ContentCells { get => contentCells; }

		public SerializedDictionary<Vector2Int, GridCell> GridMap { get => gridMap; }

		public void Initialize()
		{
			contentCells = new HashSet<Vector2Int>();
			gridMap = new SerializedDictionary<Vector2Int, GridCell>();
		}

		public void Dispose()
		{
			foreach (var gridCell in gridMap)
			{
				gridCell.Value.Dispose();
			}
			gridMap.Clear();
			Initialize();
		}

		public GridCell GetCell(Vector2Int cellIndex)
		{
			GridCell cell = null;
			if (gridMap.TryGetValue(cellIndex, out cell))
			{
				return cell;
			}
			else
			{
				cell = new GridCell(cellIndex);
				gridMap.Add(cellIndex, cell);
				return cell;
			}
		}

		public GridCell AddTile(Vector2Int cellIndex, GridTileInstance newTile)
		{
			GridCell cell = null;
			if (gridMap.TryGetValue(cellIndex, out cell))
			{
				cell.AddTile(newTile);
			}
			else
			{
				cell = new GridCell(cellIndex);
				cell.AddTile(newTile);
				gridMap.Add(cellIndex, cell);
			}

			if (!contentCells.Contains(cellIndex))
				contentCells.Add(cellIndex);

			return cell;
		}

		public Vector2Int GetRandomPosition()
		{
			return contentCells.ToList()[UnityEngine.Random.Range(0, contentCells.Count)];
		}
	}
}