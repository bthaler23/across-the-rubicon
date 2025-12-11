using Game.Gameplay;
using Game.Character;
using Game.Grid;
using GamePlugins.ObjectPool;
using GamePlugins.Utils;
using System;
using UnityEngine;
using Game.Data;

namespace Game
{
	public class AbilityAction : BaseActorAction
	{
		[SerializeField]
		private GameObject attackEffectPrefab;
		[SerializeField]
		private float attackEffectDuration = 0.5f;

		private GameObject currentAttackEffect;

		public override void Initialize(AbilityInfo action, ITurnActor owner)
		{
			base.Initialize(action, owner);
			ObjectPool.Instance.CachePrefab(attackEffectPrefab, 5);
		}

		protected override void OnCellClicked(Vector2Int gridIndex)
		{
			base.OnCellClicked(gridIndex);
			if (IsInRange(gridIndex) && !IsActionStarted())
			{
				// Find an actor at the clicked cell
				CharacterBehaviour target = GameplayController.Instance.GetActorAt(gridIndex);

				// Attack only enemies and not ourselves
				if (target != null && !ReferenceEquals(target, Owner))
				{
					// Use team color difference as enemy check (TeamInfo is not exposed)
					if (target.GetTeamColor() != Owner.GetTeamColor())
					{
						HitData hitData = Owner.GetHitData(target);
						target.ApplyDamage(hitData);
					}
				}
				SpawnAttackFX(gridIndex);
				AllocateSelfKeywords();
			}
		}

		protected override void OnExecuteInvoked()
		{
			base.OnExecuteInvoked();
			AllocateSelfKeywords();
			FireOnCompletedEvent();
		}

		private void SpawnAttackFX(Vector2Int gridIndex)
		{
			var pos = HexGridManager.Instance.GridIndexToWordPosition(gridIndex);
			GameObject attackFX = ObjectPool.GetObject(attackEffectPrefab);
			attackFX.transform.position = pos;
			attackFX.SetGameObjectActive(true);

			StartAction();

			Action delayedActions = () =>
			{
				ObjectPool.ReturnObject(attackFX);
				FireOnCompletedEvent();
			};

			StartCoroutine(DelayedExecuteAction(attackEffectDuration, delayedActions));
		}

		public override bool IsAvailable()
		{
			return true;
		}

		protected override bool HasRange()
		{
			return true;
		}

		public override string GetDescription()
		{
			if (actionInfo.DescriptionOption == AbilityInfo.DescriptionType.DynamicByScript)
				return $"ATK [{Owner.GetStatValueInt(Stats.StatType.AttackMin)} - {Owner.GetStatValueInt(Stats.StatType.AttackMax)}]  Range: {GetActionRange()}";
			return base.GetDescription();
		}

		public override Sprite GetIcon()
		{
			if (actionInfo.OverrideWithWeaponIcon)
			{
				var weapon = Owner.EquipmentController.GetEquippedWeapon();
				return weapon?.Icon ?? base.GetIcon();
			}
			return base.GetIcon();
		}
	}
}
