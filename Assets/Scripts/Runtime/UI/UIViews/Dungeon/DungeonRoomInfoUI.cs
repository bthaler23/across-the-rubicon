using Game.Data;
using GamePlugins.Utils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class DungeonRoomInfoUI : BaseCollectionElementUI<DungeonRoomInfo>
	{
		[SerializeField]
		public TextMeshProUGUI roomNameLabel;
		[SerializeField]
		public TextMeshProUGUI roomDescriptionLabel;
		[SerializeField]
		public TextMeshProUGUI roomSizeLabel;
		[SerializeField]
		public GameObject selectionGO;

		public override void Show(DungeonRoomInfo data, Action<object> onSelected)
		{
			base.Show(data, onSelected);
			roomNameLabel.SetTextSafe(data.RoomName);
			roomDescriptionLabel.SetTextSafe(data.RoomDescription);
			roomSizeLabel.SetTextSafe(data.GetSizeDescription());
		}

		public void ToggleSelected(bool value)
		{
			selectionGO.SetGameObjectActive(value);
		}
	}
}
