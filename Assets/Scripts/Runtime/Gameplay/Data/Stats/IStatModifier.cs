using UnityEngine;

namespace Game.Stats
{
	public enum ModifierType { Add, Multiplier }

	public interface IStatModifier
	{
		float Value { get; }
		ModifierType Type { get; }
		object Source { get; }   // who applied this modifier?
	}
}
