using Game.Data;
using Game.Events;
using Game.Gameplay;
using Game.Stats;
using GamePlugins.Events;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
			equipment = sourceEquipment;
			EquipWeapon(sourceEquipment.weaponSetup);
			EquipKeywords(sourceEquipment.keywords);
			EquipAbilities(sourceEquipment.abilities);
		}

		private void EquipAbilities(List<AbilityInfo> abilities)
		{
			foreach(var ab in abilities)
			{
				owner.EquipAction(ab);
			}
		}

		private void EquipKeywords(List<KeywordInfo> keywords)
		{
			keywordState ??= new SerializedDictionary<KeywordInfo, KeywordLogic>();
			foreach (var keyword in keywords)
			{
				AddKeyword(keyword, 1);
			}
		}

		private void AddKeyword(KeywordInfo keyword, int count)
		{
			if (keywordState.ContainsKey(keyword))
			{
				keywordState[keyword].IncerementStack(count);
			}
			else
			{
				var logic = keyword.GetLogic();
				logic.InitializeLogic(owner, count);
				keywordState.Add(keyword, logic);
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

		internal void EquipKeyword(KeywordInfo keyword, int count)
		{
			AddKeyword(keyword, count);
			EventBus.Publish<OnShowFloatingUiText>(new OnShowFloatingUiText(owner.uiActionBarXform, $"+{count}", owner.statsGainUiTextColor, keyword.Icon));
		}

		internal void RemoveKeyword(KeywordInfo keyword, int count)
		{
			if (!keywordState.ContainsKey(keyword)) return;
			keywordState[keyword].DecrementStack(count);

			if (keywordState[keyword].CurrentStack <= 0)
			{
				keywordState[keyword].DestroyLogic();
				keywordState.Remove(keyword);
			}
			EventBus.Publish<OnShowFloatingUiText>(new OnShowFloatingUiText(owner.uiActionBarXform, $"-{count}", owner.statsLostUiTextColor, keyword.Icon));
		}
	}
}
