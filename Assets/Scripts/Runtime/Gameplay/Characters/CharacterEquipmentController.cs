using Game.Data;
using Game.Gameplay;
using Game.Stats;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;

namespace Game.Character
{
	public class CharacterEquipmentController : MonoBehaviour
	{
		[ShowInInspector, ReadOnly]
		private CharacterBehaviour owner;
		[ShowInInspector, ReadOnly]
		private CharacterEquipmentSetup equipment;
		[ShowInInspector, ReadOnly]
		private SerializedDictionary<KeywordInfo, KeywordLogic> keywordState;

		public void Initialize(CharacterBehaviour owner, CharacterEquipmentSetup sourceEquipment)
		{
			this.owner = owner;
			equipment = new CharacterEquipmentSetup();
			EquipWeapon(sourceEquipment.weaponSetup);
			EquipKeywords(sourceEquipment.keywords);
		}

		private void EquipKeywords(List<KeywordInfo> keywords)
		{
			keywordState = new SerializedDictionary<KeywordInfo, KeywordLogic>();
			foreach (var keyword in keywords)
			{
				if (keywordState.ContainsKey(keyword))
				{
					keywordState[keyword].IncerementStack();
				}
				else
					keywordState.Add(keyword, keyword.GetLogic());
			}

			foreach (var keyword in keywordState)
			{
				keyword.Value.InitializeLogic(owner);
			}
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

		public WeaponInfo GetEquippedWeapon()
		{
			return equipment.weaponSetup.weapon;
		}
	}
}
