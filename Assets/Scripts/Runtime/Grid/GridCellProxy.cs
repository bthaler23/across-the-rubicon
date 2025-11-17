using Game.Grid;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Grid
{
	public class GridCellProxy : MonoBehaviour
	{
		[ShowInInspector, ReadOnly]
		private GridCell cell;

		public void SetCellRef(GridCell cell)
		{
			this.cell = cell;
		}

		[Button]
		void DebugTestGridPosition()
		{
			var gridIndex = HexGridManager.Instance.Grid.WordPositionToGridIndex(transform.position);
			var worldPos = HexGridManager.Instance.Grid.GridIndexToWordPosition(cell.CellIndex);
			Debug.Log($"Cell Index: {cell.CellIndex} World {transform.position} => Actual Grid Index: {gridIndex} World {worldPos}");
		}
	}
}
