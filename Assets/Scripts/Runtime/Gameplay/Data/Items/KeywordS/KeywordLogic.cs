using Game.Character;
using Game.Data;
using Game.Gameplay;
using Game.Stats;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character
{
	public class KeywordLogic
	{
		internal KeywordInfo keyword;
		internal int stackCount;
		internal CharacterBehaviour ownerCharacter;

		public KeywordLogic(KeywordInfo keyword)
		{
			this.keyword = keyword;
			this.stackCount = 1;
		}

		internal virtual void InitializeLogic(CharacterBehaviour ownerCharacter)
		{
			this.ownerCharacter = ownerCharacter;
			ApplyStatsModifiers();
			ownerCharacter.OnPrepareHit += OwnerCharacter_OnPrepareHit;
			ownerCharacter.OnReceiveHit += OwnerCharacter_OnReceiveHit;
		}

		internal virtual void DestroyLogic()
		{
			ownerCharacter.OnPrepareHit -= OwnerCharacter_OnPrepareHit;
			ownerCharacter.OnReceiveHit -= OwnerCharacter_OnReceiveHit;
			RemoveStatsModifiers();
		}

		protected virtual void ApplyStatsModifiers()
		{
			List<StatModifier> statModifiers = GetStatModifiers();
			foreach (var modifier in statModifiers)
				ownerCharacter.characterStats.AddModifier(modifier);
		}

		protected virtual List<StatModifier> GetStatModifiers()
		{
			return new List<StatModifier>();
		}

		protected virtual void RemoveStatsModifiers()
		{
			ownerCharacter.characterStats.RemoveModifier(this);
		}

		internal virtual void IncerementStack()
		{
			if (keyword.IsStackable())
				stackCount++;
		}

		internal virtual void DecrementStack()
		{
			if (keyword.IsStackable() && stackCount > 0)
			{
				stackCount--;
			}
		}

		internal bool HasStacks()
		{
			return stackCount > 0;
		}

		protected virtual void OwnerCharacter_OnReceiveHit(HitData data)
		{
		}

		protected virtual void OwnerCharacter_OnPrepareHit(HitData data)
		{
		}
	}
}
