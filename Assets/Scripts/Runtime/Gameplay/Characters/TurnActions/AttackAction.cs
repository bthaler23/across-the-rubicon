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
				// Find an actor at the clicked cell
				ActorController target = GameplayController.Instance.GetActorAt(gridIndex);

				// Attack only enemies and not ourselves
				if (target != null && !ReferenceEquals(target, Owner))
				{
					// Use team color difference as enemy check (TeamInfo is not exposed)
					if (target.GetTeamColor() != Owner.GetTeamColor())
					{
						target.ApplyDamage(Owner.Info.Damage);
					}
				}

				FireOnCompletedEvent();
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
