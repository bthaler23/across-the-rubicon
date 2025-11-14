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

		[SerializeField]
		private List<Vector2Int> availlabelStartingPositions;

		public HashSet<Vector2Int> ContentCells { get => contentCells; }

		public SerializedDictionary<Vector2Int, GridCell> GridMap { get => gridMap; }

		public void Initialize()
		{
			contentCells = new HashSet<Vector2Int>();
			gridMap = new SerializedDictionary<Vector2Int, GridCell>();
			availlabelStartingPositions = new List<Vector2Int>(contentCells);
		}

		public void Dispose()
		{
			foreach (var gridCell in gridMap)
			{
				gridCell.Value.Dispose();
			}
			gridMap.Clear();
			availlabelStartingPositions.Clear();
			contentCells.Clear();
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
			{
				availlabelStartingPositions.Add(cellIndex);
				contentCells.Add(cellIndex);
			}



			return cell;
		}

		public Vector2Int GetRandomStartingPosition()
		{
			if (availlabelStartingPositions.Count > 0)
			{
				var pos = availlabelStartingPositions[UnityEngine.Random.Range(0, availlabelStartingPositions.Count)];
				availlabelStartingPositions.Remove(pos);
				return pos;
			}

			//Default to all content cells
			return contentCells.ToList()[UnityEngine.Random.Range(0, contentCells.Count)];
		}

		public List<Vector2Int> GetRandomPositions(int count)
		{
			var result = new List<Vector2Int>();
			if (count <= 0)
				return result;

			if (contentCells == null || contentCells.Count == 0)
				return result;

			var cells = contentCells.ToList();

			// Fisher-Yates shuffle
			for (int i = cells.Count - 1; i > 0; i--)
			{
				int j = UnityEngine.Random.Range(0, i + 1);
				var tmp = cells[i];
				cells[i] = cells[j];
				cells[j] = tmp;
			}

			int uniqueToTake = Mathf.Min(count, cells.Count);
			for (int i = 0; i < uniqueToTake; i++)
			{
				result.Add(cells[i]);
			}

			// If requested more than available, repeat the last picked position
			for (int i = uniqueToTake; i < count; i++)
			{
				result.Add(result[uniqueToTake - 1]);
			}

			return result;
		}
	}
}