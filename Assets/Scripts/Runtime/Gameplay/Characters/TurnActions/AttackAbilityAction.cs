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
	public class AttackAbilityAction : TurnActionBase
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

		protected override void ExecuteOnCellClickAction(Vector2Int gridIndex)
		{
			if (IsInRange(gridIndex) && !IsActionStarted())
			{
				// Find an actor at the clicked cell
				CharacterBehaviour target = GameplayController.Instance.GetActorAt(gridIndex);
				if (CanExecuteAction(target))
				{
					base.ExecuteOnCellClickAction(gridIndex);
					DealAttacks(target);
					DealSelfAttacks();

					StartCoroutine(DelayedExecuteAction(attackEffectDuration, () =>
					{
						FireOnCompletedEvent();
					}));
				}
			}
		}

		public bool CanExecuteAction(CharacterBehaviour target)
		{
			TargetCategory targetType = TargetCategory.None;

			// Attack only enemies and not ourselves
			if (target != null && !ReferenceEquals(target, Owner))
			{
				// Use team color difference as enemy check (TeamInfo is not exposed)
				if (target.GetTeamColor() != Owner.GetTeamColor())
				{
					targetType = TargetCategory.Enemy;
				}
				else
				{
					targetType = TargetCategory.Ally;
				}
			}

			if (targetType == TargetCategory.None || (abilityInfo.Target != TargetCategory.Any && abilityInfo.Target != targetType)) return false;

			return base.CanExecuteAction();
		}

		protected override void ExecuteOnUIClickAction()
		{
			base.ExecuteOnUIClickAction();
			DealSelfAttacks();
			FireOnCompletedEvent();
		}

		protected override void ExecuteCommonActions()
		{
			DealSelfAttacks();
			HealSelf();
			base.ExecuteCommonActions();
		}

		private void HealSelf()
		{
			if (abilityInfo.HealSelf)
			{
				Owner.ApplyHeal(abilityInfo.HealSelfAmount);
			}
		}

		protected virtual void DealAttacks(CharacterBehaviour target)
		{
			if (abilityInfo.DealAttack)
				DealAttack(abilityInfo.Attack, target);
		}

		protected virtual void DealSelfAttacks()
		{
			if (abilityInfo.DealSelfAttack)
				DealAttack(abilityInfo.SelfAttack, Owner);
		}

		protected void DealAttack(AttackDefinition attack, CharacterBehaviour target)
		{
			HitData hitData = Owner.GetHitData(target);
			hitData.damage = attack.CalculateAttackDamage(hitData.damage);
			target.ApplyDamage(hitData);
			SpawnAttackFX(target.CurrentPosition);
		}

		private void SpawnAttackFX(Vector2Int gridIndex)
		{
			var pos = HexGridManager.Instance.GridIndexToWordPosition(gridIndex);
			GameObject attackFX = ObjectPool.GetObject(attackEffectPrefab);
			attackFX.transform.position = pos;
			attackFX.SetGameObjectActive(true);

			Action delayedActions = () =>
			{
				ObjectPool.ReturnObject(attackFX);
			};

			StartCoroutine(DelayedExecuteAction(attackEffectDuration, delayedActions));
		}

		public override string GetDescription()
		{
			if (abilityInfo.DescriptionOption == AbilityInfo.DescriptionType.DynamicByScript)
				return $"ATK [{Owner.GetStatValueInt(Stats.StatType.AttackMin)} - {Owner.GetStatValueInt(Stats.StatType.AttackMax)}]  Range: {GetActionRange()}";
			return base.GetDescription();
		}
	}
}
