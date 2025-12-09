using Game.Data;
using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CharacterInfoData = Game.Character.CharacterInfoData;

namespace Game.Settings
{
	[CreateAssetMenu(fileName = "GameplaySettings", menuName = "Rubicon/GameplaySettings")]
	public class GameplaySettings : ScriptableObject , IResource
	{
		[TitleGroup("Heroes")]
		[SerializeField]
		private List<CharacterInfoData> heroSetups;

		[TitleGroup("Dungeons")]
		[SerializeField]
		private List<DungeonInfo> dungeonInfos;

		[TitleGroup("Teams")]
		[SerializeField]
		private Color heroTeamColor;
		[TitleGroup("Teams")]
		[SerializeField]
		private Color enemyTeamColor;

		public Color HeroTeamColor { get => heroTeamColor; }
		public Color EnemyTeamColor { get => enemyTeamColor; }

		public IReadOnlyList<DungeonInfo> DungeonInfos { get => dungeonInfos; }
		public IReadOnlyList<CharacterInfoData> Heros { get => heroSetups; }

		public DungeonInfo DefaultDungeon { get => dungeonInfos.FirstOrDefault(); }

		public void Initialize()
		{
		}

		public void Dispose()
		{
		}

		public List<CharacterInfoData> GetDefaultHeroes(int heroCount)
		{
			// Return the requested number of heroes. If the requested count exceeds
			// available heroes, loop over the list again.
			if (heroCount <= 0)
			{
				return new List<CharacterInfoData>();
			}

			if (heroSetups == null || heroSetups.Count == 0)
			{
				return new List<CharacterInfoData>();
			}

			var result = new List<CharacterInfoData>(heroCount);
			for (int i = 0; i < heroCount; i++)
			{
				result.Add(heroSetups[i % heroSetups.Count]);
			}
			return result;
		}
	}
}
