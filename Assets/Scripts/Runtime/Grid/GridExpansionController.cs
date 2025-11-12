using Game.Grid;
using GamePlugin.Utils;
using GamePlugins.ObjectPool;
using GamePlugins.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GamePlugin.Grid
{
	public class GridExpansionController : MonoBehaviour
	{
		[SerializeField]
		private GridTileInstance gridTilePrefab;
		[SerializeField]
		private Vector2Int initialCellCountRange;
		[SerializeField]
		private Vector2Int gridDimensions;

		private System.Random randomGenerator;

		private void Awake()
		{
			ObjectPool.Instance.CachePrefab(gridTilePrefab.gameObject, 20);
		}



		[Button]
		public void InitializeGridWithCellCount()
		{
			if (randomGenerator == null)
				randomGenerator = new System.Random();

			int cellCount = randomGenerator.Next(initialCellCountRange.x, initialCellCountRange.y);
			var cellList = ProceduralGridGenerator.GenerateRandomHexGrid(cellCount, randomGenerator);

			BuildGrid(cellList);
		}

		[Button]
		public void InitializeGridWithDimensions()
		{
			if (randomGenerator == null)
				randomGenerator = new System.Random();

			int cellCount = randomGenerator.Next(initialCellCountRange.x, initialCellCountRange.y);
			var cellList = ProceduralGridGenerator.GenerateRandomHexGrid(cellCount, randomGenerator);

			BuildGrid(cellList);
		}

		private void BuildGrid(List<HexCell> cellList)
		{
			foreach (var cell in cellList)
			{
				ExpandAt(cell);
			}
		}

		private void ExpandAt(HexCell cell)
		{
			GridTileInstance foundation = ObjectPool.GetObject<GridTileInstance>(gridTilePrefab);
			HexGridManager.Instance.AddTile(cell.Position, foundation);
		}

		[Button]
		private void ClearGrid()
		{
			HexGridManager.Instance.GridMap.Dispose();
		}
	}
}
