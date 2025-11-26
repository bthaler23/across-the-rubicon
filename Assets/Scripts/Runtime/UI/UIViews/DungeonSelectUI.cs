using Game.Data;
using Game.Progress;
using Game.UI;
using GamePlugins.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class DungeonSelectUI : BaseViewUI
	{
		[SerializeField]
		private DungeonInfoUI dungeonPrefab;
		[SerializeField]
		private RectTransform dungeonItemParent;
		[SerializeField]
		private Button continueButton;
		private List<DungeonInfoUI> dungeonItems;
		private DungeonInfoUI selectedDungeon;

		protected override void Awake()
		{
			base.Awake();
			continueButton.onClick.AddListener(OnContinueButtonClicked);
		}

		private void OnContinueButtonClicked()
		{
			if (selectedDungeon != null)
			{
				UIController.Instance.SelectDungeon(selectedDungeon.Dungeon);
			}
		}

		public void PopulateDungeons(IReadOnlyList<DungeonInfo> dungeonInfos)
		{
			UpdateDungeonInfos(dungeonInfos);
		}

		private void UpdateDungeonInfos(IReadOnlyList<DungeonInfo> dungeonInfos)
		{
			if (dungeonInfos.IsNullOrEmpty())
			{
				return;
			}

			int neededCount = dungeonInfos.Count;
			dungeonItems ??= new List<DungeonInfoUI>();
			// Ensure we have enough UI widgets
			while (dungeonItems.Count < neededCount)
			{
				InstantiateDungeonInfoUI();
			}

			// Populate and activate the required widgets
			for (int i = 0; i < neededCount; i++)
			{
				var ui = dungeonItems[i];
				var info = dungeonInfos[i];
				ui.SetDungeonInfo(info);
			}

			// Deactivate any extra widgets
			for (int i = neededCount; i < dungeonItems.Count; i++)
			{
				dungeonItems[i].Hide();
			}
		}

		private DungeonInfoUI InstantiateDungeonInfoUI()
		{
			var instance = Instantiate(dungeonPrefab, dungeonItemParent);
			dungeonItems.Add(instance);
			instance.OnDungeonSelected += OnDungeonSelectedHandler;
			return instance;
		}

		private void OnDungeonSelectedHandler(DungeonInfoUI info)
		{
			if (selectedDungeon != null && info != selectedDungeon)
				selectedDungeon.ToggleSelection(false);
			selectedDungeon = info;
			selectedDungeon.ToggleSelection(true);
		}
	}
}
