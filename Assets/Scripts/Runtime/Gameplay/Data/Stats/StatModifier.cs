using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Stats
{
	[Serializable]
	public class StatModifierInfo
	{
		[HideLabel]
		[HorizontalGroup]
		[SerializeField]
		private StatType stat;
		[HideLabel]
		[HorizontalGroup]
		[SerializeField]
		private ModifierType type;
		[HorizontalGroup]
		[SerializeField]
		private float value;

		public float Value { get => value; }
		public ModifierType Type { get => type; }
		public StatType Stat { get => stat; }
	}


	[Serializable]
	public class StatModifier : IStatModifier
	{
		private StatModifierInfo info;
		private object owner;

		public StatModifierInfo Info => info;

		public float Value => info.Value;

		public ModifierType Type => info.Type;

		public object Owner => owner;

		public StatModifier(StatModifierInfo info, object owner)
		{
			this.info = info;
			this.owner = owner;
		}
	}
}