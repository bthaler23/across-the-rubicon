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
	}
}
