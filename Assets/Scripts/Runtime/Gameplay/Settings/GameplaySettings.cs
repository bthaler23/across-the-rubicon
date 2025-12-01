using Game.Data;
using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Settings
{
	[CreateAssetMenu(fileName = "GameplaySettings", menuName = "Rubicon/GameplaySettings")]
	public class GameplaySettings : ScriptableObject , IResource
	{
		[TitleGroup("Heroes")]
		[SerializeField]
		private List<ActorInfo> heros;

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
		public IReadOnlyList<ActorInfo> Heros { get => heros; }

		public DungeonInfo DefaultDungeon { get => dungeonInfos.FirstOrDefault(); }

		public void Initialize()
		{
		}

		public void Dispose()
		{
		}

		public List<ActorInfo> GetDefaultHeroes(int heroCount)
		{
			// Return the requested number of heroes. If the requested count exceeds
			// available heroes, loop over the list again.
			if (heroCount <= 0)
			{
				return new List<ActorInfo>();
			}

			if (heros == null || heros.Count == 0)
			{
				return new List<ActorInfo>();
			}

			var result = new List<ActorInfo>(heroCount);
			for (int i = 0; i < heroCount; i++)
			{
				result.Add(heros[i % heros.Count]);
			}
			return result;
		}
	}
}
