using Game.Grid;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Data
{
	[CreateAssetMenu(fileName = "DungeonRoomInfo", menuName = "Rubicon/DungeonRoomInfo", order = 1)]
	public class DungeonRoomInfo : ScriptableObject
	{
		[BoxGroup("Room")]
		[SerializeField]
		private string roomName;
		[SerializeField]
		[BoxGroup("Room")]
		private string roomDescription;
		[SerializeField]
		[InlineProperty]
		[HideLabel]
		[BoxGroup("Grid")]
		private GridSetup gridSetup;

		[BoxGroup("Enemies")]
		[SerializeField]
		private ActorInfo[] enemyActors;

		public string RoomName { get => roomName; }
		public string RoomDescription { get => roomDescription; }
		public GridSetup GridSetup { get => gridSetup; set => gridSetup = value; }
		public ActorInfo[] EnemyActors { get => enemyActors; }

		public string GetSizeDescription()
		{
			return $"{gridSetup.Size.x} x {gridSetup.Size.y}";
		}
	}
}
