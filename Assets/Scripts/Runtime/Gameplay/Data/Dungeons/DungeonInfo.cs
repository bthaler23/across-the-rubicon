using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;

namespace Game.Data
{
	[CreateAssetMenu(fileName = "DungeonInfo", menuName = "Rubicon/DungeonInfo", order = 1)]
	public class DungeonInfo : ScriptableObject
	{
		[SerializeField]
		private string DungeonName;
		[SerializeField]
		private int DifficultyLevel;
		[SerializeField]
		private Sprite DungeonIcon;
		[SerializeField]
		[PropertyRange(1, 10)]
		private int heroCount = 1;
		[SerializeField]
		private DungeonRoomInfo[] Rooms;

		public int HeroCount { get => heroCount; }

		public string GetDungeonName() => DungeonName;
		public int GetDifficultyLevel() => DifficultyLevel;
		public Sprite GetDungeonIcon() => DungeonIcon;
		public DungeonRoomInfo GetFirstRoom() => Rooms.FirstOrDefault();
		public DungeonRoomInfo[] GetRooms() => Rooms;

		internal string GetDungeonSize()
		{
			return $"Rooms: {Rooms.Length}";
		}
	}
}
