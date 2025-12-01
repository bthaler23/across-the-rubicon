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
	public enum ShapeType
	{
		Rectangle,
		Round,
		Triangle
	}

	[Serializable]
	public class GridSetup
	{
		[SerializeField]
		private ShapeType shape;
		[ShowIf("@this.shape!=ShapeType.Rectangle")]
		[SerializeField]
		private int flatSize;
		[ShowIf("@this.shape==ShapeType.Rectangle")]
		[SerializeField]
		private Vector2Int size;
		[SerializeField]
		[PropertyRange(0, 1)]
		private float edgeRemovalChance;
		[SerializeField]
		[PropertyRange(0, 1)]
		private float innerRemovalChance;

		public int FlatSize { get => flatSize; }
		public Vector2Int Size { get => size; }
		public float EdgeRemovalChance { get => edgeRemovalChance; }
		public float InnerRemovalChance { get => innerRemovalChance; }
		public ShapeType Shape { get => shape; }
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
		private bool initialized = false;

		private void TryInitialize()
		{
			if (initialized) return;
			ObjectPool.Instance.CachePrefab(gridTilePrefab.gameObject, 20);
			initialized = true;
		}

		public void InitializeGrid(GridSetup gridSetup)
		{
			TryInitialize();

			if (randomGenerator == null)
				randomGenerator = new System.Random();

			List<HexCell> cellList = null;
			if (gridSetup.Shape == ShapeType.Round)
			{
				cellList = ProceduralGridGenerator.GenerateCircularHexGrid
					(gridSetup.FlatSize, gridSetup.EdgeRemovalChance, gridSetup.InnerRemovalChance, randomGenerator);
			}
			else if (gridSetup.Shape == ShapeType.Triangle)
			{
				cellList = ProceduralGridGenerator.GenerateTriangularHexGrid
					(gridSetup.FlatSize, gridSetup.EdgeRemovalChance, gridSetup.InnerRemovalChance, randomGenerator);
			}
			else
			{
				//Default is rectangle
				cellList = ProceduralGridGenerator.GenerateVariableRectHexGrid
					(gridSetup.Size.x, gridSetup.Size.y, gridSetup.EdgeRemovalChance, gridSetup.InnerRemovalChance, randomGenerator);
			}
			BuildGrid(cellList);
		}

		[Button]
		private void InitializeGridWithCellCount()
		{
			TryInitialize();
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
