using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Grid
{
	public class GridCell
	{
		[ShowInInspector, ReadOnly]
		private GridCellProxy dataProxy;
		[ShowInInspector, ReadOnly]
		private Vector2Int cellIndex;
		[ShowInInspector, ReadOnly]
		private GridTileInstance gridTileInstance;

		public Vector2Int CellIndex { get => cellIndex; }
		public GridTileInstance TileInstance { get => gridTileInstance; }

		public GridCell(Vector2Int cellIndex)
		{
			this.cellIndex = cellIndex;
		}

		public void AddTile(GridTileInstance tileInstance)
		{
			EnsureParentGameobject();
			dataProxy.transform.position = tileInstance.transform.position;
			gridTileInstance = tileInstance;
		}

		private void EnsureParentGameobject()
		{
			if (dataProxy == null)
			{
				GameObject contentHolder = new GameObject($"Cell_[{cellIndex.x},{cellIndex.y}]");
				contentHolder.transform.parent = HexGridManager.GetPreviewParent();
				dataProxy = contentHolder.AddComponent<GridCellProxy>();
				dataProxy.SetCellRef(this);
			}
		}

		public void Dispose()
		{
			if (dataProxy != null)
			{
				UnityEngine.Object.Destroy(dataProxy.gameObject);
			}
			dataProxy = null;
		}
	}
}
