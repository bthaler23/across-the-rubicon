using Game.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings
{
	[CreateAssetMenu(fileName = "GameplaySettings", menuName = "Rubicon/GameplaySettings")]
	public class GameplaySettings : ScriptableObject
	{
		[SerializeField]
		private List<ActorInfo> heros;

		[SerializeField]
		private List<DungeonInfo> dungeonInfos;

		[SerializeField]
		private GridTileInstance gridTilePrefab;

		[SerializeField]
		private List<TeamInfo> teamInfos;

		public IReadOnlyList<TeamInfo> TeamInfos { get => teamInfos; }
		public IReadOnlyList<DungeonInfo> DungeonInfos { get => dungeonInfos; }
		public IReadOnlyList<ActorInfo> Heros { get => heros; }
	}
}
