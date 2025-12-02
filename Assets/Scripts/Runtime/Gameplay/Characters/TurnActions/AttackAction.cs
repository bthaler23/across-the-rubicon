using Game.Gameplay;
using Game.Grid;
using GamePlugins.ObjectPool;
using GamePlugins.Utils;
using System;
using UnityEngine;
namespace Game
{
	public class AttackAction : BaseActorAction
	{
		[SerializeField]
		private GameObject attackEffectPrefab;
		[SerializeField]
		private float attackEffectDuration = 0.5f;

		private GameObject currentAttackEffect;

		public override void Initialize(ITurnActor owner)
		{
			base.Initialize(owner);
			ObjectPool.Instance.CachePrefab(attackEffectPrefab, 5);
		}

		protected override void OnCellClicked(Vector2Int gridIndex)
		{
			base.OnCellClicked(gridIndex);
			if (IsInRange(gridIndex) && !IsActionStarted())
			{
				// Find an actor at the clicked cell
				ActorController target = GameplayController.Instance.GetActorAt(gridIndex);

				// Attack only enemies and not ourselves
				if (target != null && !ReferenceEquals(target, Owner))
				{
					// Use team color difference as enemy check (TeamInfo is not exposed)
					if (target.GetTeamColor() != Owner.GetTeamColor())
					{
						target.ApplyDamage(Owner.GetCharacterAttackDamage());
					}
				}

				SpawnAttackFX(gridIndex);
			}
		}

		private void SpawnAttackFX(Vector2Int gridIndex)
		{
			var pos = HexGridManager.Instance.GridIndexToWordPosition(gridIndex);
			GameObject attackFX = ObjectPool.GetObject(attackEffectPrefab);
			attackFX.transform.position = pos;
			attackFX.SetGameObjectActive(true);

			StartAction();

			Action delatedActions = () =>
			{
				ObjectPool.ReturnObject(attackFX);
				FireOnCompletedEvent();
			};

			StartCoroutine(DelayedExecuteAction(attackEffectDuration, delatedActions));
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
