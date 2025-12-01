using Game.Data;
using Game.Settings;
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
		public DungeonRoomInfo CurrentDungeonRoom => progressData.currentRoom;
		public List<ActorInfo> CurrentHeroes => progressData.currentHeroes;

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

		public void ResetProgress()
		{
			progressData = new ProgressData();
		}

		internal void EnsureProgress()
		{
			if (progressData == null)
			{
				progressData = new ProgressData();
			}

			var gameplaySettings = ResourceManager.Instance.RequestResource<GameplaySettings>();

			if (progressData.currentDungeon == null)
			{
				Debug.LogWarning("No Dungeon Selected.");
				var defaultDungeon = gameplaySettings.DefaultDungeon;
				SelectDungeon(defaultDungeon);
				SelectDungeonRoom(defaultDungeon.GetFirstRoom());
			}

			if(!progressData.HasHeros())
			{
				Debug.LogWarning("No heroes selected for the dungeon.");
				SelectCharacters(gameplaySettings.GetDefaultHeroes(progressData.currentDungeon.HeroCount));
			}

			CheckDebugOverridesSetup();
		}

		private void CheckDebugOverridesSetup()
		{
			var debugSettings = ResourceManager.Instance.RequestResource<DebugSettings>();
			if (debugSettings.OverrideDungeon)
			{
				Debug.LogWarning("DebugSettings OverrideDungeon is enabled. Overriding current dungeon.");
				SelectDungeon(debugSettings.DebugDungeon);
				SelectDungeonRoom(debugSettings.DebugDungeon.GetFirstRoom());
			}
			if (debugSettings.OverrideHeros)
			{
				Debug.LogWarning("DebugSettings OverrideHeros is enabled. Overriding current heroes.");
				SelectCharacters(debugSettings.DebugHeros);
			}
		}
	}
}
