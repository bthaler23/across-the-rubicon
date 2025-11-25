using System;
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
		private DungeonRoomInfo[] Rooms;

		public string GetDungeonName() => DungeonName;
		public int GetDifficultyLevel() => DifficultyLevel;
		public Sprite GetDungeonIcon() => DungeonIcon;
		public DungeonRoomInfo[] GetRooms() => Rooms;

		internal string GetDungeonSize()
		{
			return $"Rooms: {Rooms.Length}";
		}
	}
}
