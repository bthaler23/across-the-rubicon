using UnityEngine;

namespace Game.Stats
{
	public interface IStatValue
	{
		float GetValue();

		void AddModifier(IStatModifier modifier);
		void RemoveModifier(object owner);
	}
}
