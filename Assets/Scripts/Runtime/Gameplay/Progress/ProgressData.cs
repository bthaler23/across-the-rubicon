using Game.Data;
using Game.Character;
using Game.UI;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Progress
{
	[Serializable]
	public class ProgressData 
	{
		[SerializeField]
		internal DungeonInfo currentDungeon;
		[SerializeField]
		internal DungeonRoomInfo currentRoom;
		[SerializeField]
		internal List<CharacterSetup> currentHeroes;

		public bool HasHeros()
		{
			return currentHeroes != null && currentHeroes.Count > 0;
		}
	}
}