using Game.Character;
using Game.Stats;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Gameplay
{
	[Serializable]
	public class CharacterStats
	{
		[SerializeField]
		private int currentHealth;
		[SerializeField]
		private int currentMana;
		[SerializeField]
		private SerializedDictionary<StatType, IStatValue> statValues;

		public bool IsAlive { get { return currentHealth > 0; } }


		public CharacterStats(CharacterInfoData info)
		{
			statValues = new SerializedDictionary<StatType, IStatValue>();
			statValues.Add(StatType.MaxHealth, new StatValue(info.Health));
			statValues.Add(StatType.MovementRange, new StatValue(info.MovementRange));
			statValues.Add(StatType.AttackMin, new StatValue(info.MinAttack));
			statValues.Add(StatType.AttackMax, new StatValue(info.MaxAttack));
			statValues.Add(StatType.AttackRange, new StatValue(info.AttackRange));
			statValues.Add(StatType.Speed, new StatValue(info.Speed));
			statValues.Add(StatType.MaxMana, new StatValue(info.ManaPool));
		}

		internal void UpdateVariableStats()
		{
			currentHealth = Mathf.RoundToInt(GetStat(StatType.MaxHealth).GetValue());
			currentMana = Mathf.RoundToInt(GetStat(StatType.MaxMana).GetValue());
		}


		public void ApplyDamage(int damage)
		{
			currentHealth -= damage;
			if (currentHealth < 0)
			{
				currentHealth = 0;
			}
		}

		public void ApplyHeal(int heal)
		{
			currentHealth += heal;
			if (currentHealth > GetStat(StatType.MaxHealth).GetValue())
			{
				currentHealth = Mathf.RoundToInt(GetStat(StatType.MaxHealth).GetValue());
			}
		}

		public void ConsumeMana(int mana)
		{
			currentMana -= mana;
		}

		public IStatValue GetStat(StatType type)
		{
			if (type == StatType.Health)
				return new StatValue(currentHealth);
			if (type == StatType.Mana)
				return new StatValue(currentMana);

			if (statValues.TryGetValue(type, out var statValue))
			{
				return statValue;
			}
			return null;
		}

		public void AddModifier(StatModifier statModifier)
		{
			var stat = GetStat(statModifier.Info.Stat);
			if (stat != null)
			{
				stat.AddModifier(statModifier);
			}
		}

		public void RemoveModifier(StatType type, object owner)
		{
			var stat = GetStat(type);
			if (stat != null)
			{
				stat.RemoveModifier(owner);
			}
		}

		public void RemoveModifier(object owner)
		{
			foreach (var stat in statValues.Values)
			{
				stat.RemoveModifier(owner);
			}
		}
	}
}
