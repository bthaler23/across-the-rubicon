using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Game.Stats
{
	public class StatValue : IStatValue
	{
		private float baseValue;
		private List<IStatModifier> modifiers = new();

		public event Action<float> OnValueChanged;

		public StatValue(float baseValue)
		{
			this.baseValue = baseValue;
		}

		public float GetValue()
		{
			return CalculateFinalValue();
		}

		public void AddModifier(IStatModifier modifier)
		{
			modifiers.Add(modifier);
			OnValueChanged?.Invoke(GetValue());
		}

		public void RemoveModifier(object owner)
		{
			modifiers.RemoveAll(x => x.Owner == owner);
			OnValueChanged?.Invoke(GetValue());
		}

		private float CalculateFinalValue()
		{
			float value = baseValue;

			// 1. Additive modifiers (Flat +)
			foreach (var m in modifiers.Where(m => m.Type == ModifierType.Add))
				value += m.Value;

			// 2. Multiply modifiers (%)
			foreach (var m in modifiers.Where(m => m.Type == ModifierType.Multiplier))
				value *= 1 + m.Value;

			return value;
		}
	}
}
