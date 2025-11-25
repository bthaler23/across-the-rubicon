using Game.Data;
using GamePlugins.Utils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class DungeonInfoUI : MonoBehaviour
	{
		[SerializeField]
		private GameObject selectionGO;
		[SerializeField]
		private TextMeshProUGUI dungeonNameText;
		[SerializeField]
		private TextMeshProUGUI dungeonSizeText;
		[SerializeField]
		private Image dungeonIconImage;
		[SerializeField]
		private Button selectButton;

		private DungeonInfo dungeon;
		public DungeonInfo Dungeon { get => dungeon; }

		public event Action<DungeonInfoUI> OnDungeonSelected;

		private void Awake()
		{
			selectButton.onClick.AddListener(SelectDungeon);
		}

		public void SetDungeonInfo(DungeonInfo dungeon)
		{
			this.dungeon = dungeon;
			dungeonNameText.SetTextSafe(dungeon.GetDungeonName());
			dungeonSizeText.SetTextSafe(dungeon.GetDungeonSize());
			dungeonIconImage.SetIconSafe(dungeon.GetDungeonIcon());
			gameObject.SetActive(true);
			ToggleSelection(false);
		}

		public void SelectDungeon()
		{
			OnDungeonSelected?.Invoke(this);
		}

		internal void Hide()
		{
			gameObject.SetActive(false);
		}

		internal void ToggleSelection(bool value)
		{
			selectionGO.SetGameObjectActive(value);
		}
	}
}
