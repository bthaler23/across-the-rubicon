using Game.Data;
using Game.Progress;
using Game.UI.CharacterSelect;
using GamePlugins.Utils;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class DungeonRoomSelectUI : BaseViewUI
	{
		[Title("ROOM SELECT")]
		[SerializeField]
		private Button continueButton;
		[SerializeField]
		private TextMeshProUGUI dungeonNameLabel;
		[SerializeField]
		private ItemCollectionUI<DungeonRoomInfo, DungeonRoomInfoUI> dungeonRooms;

		private DungeonRoomInfoUI selectedRoomItem;

		protected override void Awake()
		{
			base.Awake();
			continueButton.onClick.AddListener(() =>
			{
				if (selectedRoomItem != null)
				{
					UIController.Instance.SelectDungeonRoom(selectedRoomItem.Data);
				}
			});
		}

		public override void Show()
		{
			base.Show();
			dungeonNameLabel.SetTextSafe(ProgressManager.Instance.CurrentDungeon.GetDungeonName());
			UpdateRoomsUI();
		}

		private void UpdateRoomsUI()
		{
			dungeonRooms.Update(ProgressManager.Instance.GetPossibleDungeonRooms(), OnRoomSelected, OnRoomInfoCustomShow);
		}

		private void OnRoomInfoCustomShow(DungeonRoomInfoUI uI)
		{
			uI.ToggleSelected(selectedRoomItem == uI);
		}

		private void OnRoomSelected(object obj)
		{
			if (obj is DungeonRoomInfoUI roomInfoUI)
			{
				selectedRoomItem = roomInfoUI;
				UpdateRoomsUI();
			}
		}
	}
}