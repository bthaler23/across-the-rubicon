using Game.Character;
using System;
using UnityEngine;

namespace Game.Gameplay
{
	[Serializable]
	public class HitData
	{
		public int damage;
		public CharacterBehaviour source;
		public CharacterBehaviour target;

		public HitData(int damage, CharacterBehaviour source, CharacterBehaviour target)
		{
			this.damage = damage;
			this.source = source;
			this.target = target;
		}
	}
}
