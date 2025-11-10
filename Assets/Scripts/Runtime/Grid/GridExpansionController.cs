using Game.Grid;
using GamePlugins.ObjectPool;
using GamePlugins.Utils;
using RM.Utils;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace RM.Grid
{
	public class GridExpansionController : MonoBehaviour
	{
		[SerializeField]
		private GridTileInstance gridTilePrefab;
		[SerializeField]
		private Vector2Int initialCellCountRange;

		private bool expansionEnabled = true;

		private System.Random randomGenerator;

		private void Awake()
		{
			ObjectPool.Instance.CachePrefab(gridTilePrefab.gameObject, 20);
		}

		internal void Initialize(System.Random randomGenerator)
		{
			this.randomGenerator = randomGenerator;
			expansionEnabled = true;
			InitializeGrid();
		}


		[Button]
		private void InitializeGrid()
		{
			if (randomGenerator == null)
				randomGenerator = new System.Random();

			int cellCount = randomGenerator.Next(initialCellCountRange.x, initialCellCountRange.y);
			var cellList = ProceduralGridGenerator.GenerateRandomHexGrid(cellCount, randomGenerator);
			expansionEnabled = true;

			foreach (var cell in cellList)
			{
				ExpandAt(cell);
			}
		}

		public void ExpandAt(HexCell cell)
		{
			GridTileInstance foundation = ObjectPool.GetObject<GridTileInstance>(gridTilePrefab);
			var worldPos = HexGridManager.Instance.IndexToWordPosition(cell.Position);
			foundation.transform.position = new Vector3(worldPos.x, 0, worldPos.y);
			foundation.SetGameObjectActive(true);
			HexGridManager.Instance.GridMap.AddTile(cell.Position, foundation);
		}

		[Button]
		private void ClearGrid()
		{
			HexGridManager.Instance.GridMap.Dispose();
		}
	}
}
