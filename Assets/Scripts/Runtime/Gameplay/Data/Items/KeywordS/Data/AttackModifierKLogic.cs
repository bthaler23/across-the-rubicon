using Game.Data;
using Game.Gameplay;
using Game.Stats;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character
{
	public class AttackModifierKLogic : KeywordLogic
	{
		private float dealtDMGModifier;
		private float receivedDMGModifier;

		public AttackModifierKLogic(float dealtDMGModifier, float receivedDMGModifier, KeywordInfo keyword) : base(keyword)
		{
			this.dealtDMGModifier = dealtDMGModifier;
			this.receivedDMGModifier = receivedDMGModifier;
		}

		protected override void OwnerCharacter_OnPrepareHit(HitData data)
		{
			base.OwnerCharacter_OnPrepareHit(data);
			data.damage = Mathf.RoundToInt(data.damage + (dealtDMGModifier * stackCount));
		}

		protected override void OwnerCharacter_OnReceiveHit(HitData data)
		{
			base.OwnerCharacter_OnReceiveHit(data);
			data.damage = Mathf.RoundToInt(data.damage + (receivedDMGModifier * stackCount));
		}

		//We'll go with a different solution for now
		//protected override List<StatModifier> GetStatModifiers()
		//{
		//	List<StatModifier> modifiers = new List<StatModifier>();
		//	modifiers.Add(new StatModifier(new StatModifierInfo(StatType.AttackMin, ModifierType.Add, dealtDMGModifier * stackCount), this));
		//	modifiers.Add(new StatModifier(new StatModifierInfo(StatType.AttackMax, ModifierType.Add, dealtDMGModifier * stackCount), this));
		//	return modifiers;
		//}
	}
}
