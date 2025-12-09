using Game.Data;
using Game.Stats;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Character
{
	public class CharacterEquipmentController : MonoBehaviour
	{
		[ShowInInspector, ReadOnly]
		private CharacterBehaviour owner;
		[ShowInInspector, ReadOnly]
		private CharacterEquipmentSetup equipment;

		public void Initialize(CharacterBehaviour owner, CharacterEquipmentSetup sourceEquipment)
		{
			this.owner = owner;
			equipment = new CharacterEquipmentSetup();
			EquipWeapon(sourceEquipment.weaponSetup);
		}

		private void EquipWeapon(WeaponSetup weaponSetup)
		{
			if (weaponSetup.IsEmpty()) return;

			RemoveWeapon();
			equipment.weaponSetup = weaponSetup;
			foreach (var modifier in equipment.weaponSetup.weapon.StatModifiers)
			{
				owner.characterStats.AddModifier(new StatModifier(modifier, equipment.weaponSetup.weapon));
			}
		}

		private void RemoveWeapon()
		{
			if (equipment.weaponSetup.IsEmpty()) return;

			foreach (var modifier in equipment.weaponSetup.weapon.StatModifiers)
			{
				owner.characterStats.RemoveModifier(modifier.Stat, equipment.weaponSetup.weapon);
			}
			equipment.weaponSetup = default;
		}
	}
}
