using Game.Data;
using Game.Grid;
using GamePlugin.Utils;
using GamePlugins.ObjectPool;
using GamePlugins.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Grid
{
	[Serializable]
	public class GridSetup
	{
		[SerializeField]
		private Vector2Int size;
		[SerializeField]
		[PropertyRange(0, 1)]
		private float edgeRemovalChance;
		[SerializeField]
		[PropertyRange(0, 1)]
		private float innerRemovalChance;

		public Vector2Int Size { get => size; }
		public float EdgeRemovalChance { get => edgeRemovalChance; }
		public float InnerRemovalChance { get => innerRemovalChance; }
	}

	public class GridExpansionController : MonoBehaviour
	{
		[SerializeField]
		private GridTileInstance gridTilePrefab;
		[SerializeField]
		private Vector2Int initialCellCountRange;
		[SerializeField]
		private GridSetup debugGridSetup;

		private System.Random randomGenerator;

		private void Awake()
		{
			ObjectPool.Instance.CachePrefab(gridTilePrefab.gameObject, 20);
		}

		public void InitializeGrid(GridSetup gridSetup)
		{
			if (randomGenerator == null)
				randomGenerator = new System.Random();

			int cellCount = gridSetup.Size.x * gridSetup.Size.y;
			var cellList = ProceduralGridGenerator.GenerateVariableRectHexGrid
				(gridSetup.Size.x, gridSetup.Size.y, gridSetup.EdgeRemovalChance, gridSetup.InnerRemovalChance, randomGenerator);

			BuildGrid(cellList);
		}

		[Button]
		private void InitializeGridWithCellCount()
		{
			if (randomGenerator == null)
				randomGenerator = new System.Random();

			int cellCount = randomGenerator.Next(initialCellCountRange.x, initialCellCountRange.y);
			var cellList = ProceduralGridGenerator.GenerateRandomHexGrid(cellCount, randomGenerator);

			BuildGrid(cellList);
		}

		[Button]
		private void InitializeGridWithDimensions()
		{
			InitializeGrid(debugGridSetup);
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
