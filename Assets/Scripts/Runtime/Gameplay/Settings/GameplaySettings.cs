using Game.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings
{
	[CreateAssetMenu(fileName = "GameplaySettings", menuName = "Rubicon/GameplaySettings")]
	public class GameplaySettings : ScriptableObject
	{
		[SerializeField]
		private GridTileInstance gridTilePrefab;

		[SerializeField]
		private List<TeamInfo> teamInfos;

		public IReadOnlyList<TeamInfo> TeamInfos { get => teamInfos; }
	}
}
