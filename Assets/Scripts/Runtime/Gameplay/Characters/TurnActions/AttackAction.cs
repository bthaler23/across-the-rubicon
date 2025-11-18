using Game.Gameplay;
using System;
using UnityEngine;
namespace Game
{
	public class AttackAction : BaseActorAction
	{
		protected override void OnCellClicked(Vector2Int gridIndex)
		{
			base.OnCellClicked(gridIndex);
			if (IsInRange(gridIndex))
			{

			}
		}

		public override bool IsAvailable()
		{
			return true;
		}

		protected override bool HasRange()
		{
			return true;
		}

		protected override int GetRange()
		{
			return Owner.Info.AttackRange;
		}
	}
}
