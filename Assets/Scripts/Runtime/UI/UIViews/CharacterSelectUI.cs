using Game.Data;
using Game.Progress;
using Game.UI.CharacterSelect;
using GamePlugins.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class CharacterSelectUI : BaseViewUI
	{
		[Title("CHARACTER SELECT")]
		[SerializeField]
		private Button continueButton;
		[SerializeField]
		private ItemCollectionUI<ActorInfo, CharacterIconUI> characterCollectionUI;
		[SerializeField]
		private ItemCollectionUI<ActorInfo, CharacterSlotUI> characterSlotsUI;
		[SerializeField]
		private CharacterInfoUI characterInfoUI;

		private ActorInfo[] choosenCharacter;
		private ActorInfo selectedCharacter;

		protected override void Awake()
		{
			base.Awake();
			continueButton.onClick.AddListener(() =>
			{
				var selectedCharacters = choosenCharacter.Where(c => c != null).ToList();
				if (selectedCharacters.Count == choosenCharacter.Length)
				{
					UINavigator.Instance.SelectCharacter(selectedCharacters);
				}
			});
		}

		public override void Show()
		{
			base.Show();

			choosenCharacter = new ActorInfo[ProgressManager.Instance.GetCurrentDungeonCharacterCount()];
			RefreshCharacterSlots();

			var heroesCollection = GameManager.Instance.GameplaySettings.Heros;
			SelectCharacter(heroesCollection[0]);
		}

		private void RefreshCharacterCollection()
		{
			var heroesCollection = GameManager.Instance.GameplaySettings.Heros;
			characterCollectionUI.Update(heroesCollection, OnCharacterSelected, OnCharacterShowCustom);
		}

		private void RefreshCharacterSlots()
		{
			var heroesCollection = GameManager.Instance.GameplaySettings.Heros;
			characterSlotsUI.Update(choosenCharacter.ToList(), OnSlotSelected, null);
		}

		private void OnSlotSelected(object uiItem)
		{
			if (uiItem is CharacterSlotUI slotIcon && selectedCharacter != null)
			{
				int index = characterSlotsUI.IndexOf(uiItem);
				choosenCharacter[index] = selectedCharacter;
				RefreshCharacterSlots();
			}
		}

		private void OnCharacterShowCustom(CharacterIconUI uI)
		{
			uI.ToggleState(
				isChosen: choosenCharacter.Contains(uI.Data),
				isSelected: selectedCharacter == uI.Data);
		}

		private void OnCharacterSelected(object uiItem)
		{
			if (uiItem is CharacterIconUI characterIcon && characterIcon.Data != null)
			{
				SelectCharacter(characterIcon.Data);
			}
		}

		private void SelectCharacter(ActorInfo character)
		{
			selectedCharacter = character;
			characterInfoUI.Show(character);
			RefreshCharacterCollection();
		}
	}
}