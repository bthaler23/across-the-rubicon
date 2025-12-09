using Game.Data;
using Game.Gameplay;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character
{
	[Serializable]
	public struct CharacterEquipmentSetup
	{
		[SerializeField]
		internal WeaponSetup weaponSetup;
		[SerializeField]
		internal List<AbilityInfo> abilities;

		public WeaponSetup WeaponInfo { get => weaponSetup; }
		public List<AbilityInfo> Abilities { get => abilities; }
	}
}