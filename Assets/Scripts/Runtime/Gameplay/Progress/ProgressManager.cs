using Game.Data;
using GamePlugins.Attributes;
using GamePlugins.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Progress
{
	[AutoCreateSingleton(false, false)]
	public class ProgressManager : Singleton<ProgressManager>
	{
		[SerializeField]
		private ProgressData progressData;

		public DungeonInfo CurrentDungeon => progressData.currentDungeon;

		protected override void OnAwakeCalled()
		{
			base.OnAwakeCalled();
			progressData = new ProgressData();
		}

		public void SelectDungeon(DungeonInfo dungeon)
		{
			progressData.currentDungeon = dungeon;
		}

		public void SelectCharacters(List<ActorInfo> heroes)
		{
			progressData.currentHeroes = new List<ActorInfo>();
			foreach (var h in heroes)
			{
				if (h != null)
					progressData.currentHeroes.Add(h);
			}
		}

		public int GetCurrentDungeonCharacterCount()
		{
			if (progressData.currentDungeon == null)
				return 0;
			return progressData.currentDungeon.HeroCount;
		}

		public List<DungeonRoomInfo> GetPossibleDungeonRooms()
		{
			if (progressData.currentDungeon == null)
				return new List<DungeonRoomInfo>();
			return progressData.currentDungeon.GetRooms().ToList();
		}

		public void SelectDungeonRoom(DungeonRoomInfo data)
		{
			progressData.currentRoom = data;
		}
	}
}
