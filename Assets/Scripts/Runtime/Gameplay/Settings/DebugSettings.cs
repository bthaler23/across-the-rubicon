using Game.Data;
using Game.Character;
using GamePlugins.Attributes;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Settings
{
	[CreateAssetMenu(fileName = "DebugSettings", menuName = "Rubicon/DebugSettings")]
	public class DebugSettings : ScriptableObject, IResource
	{
		[RequiredBool(false)]
		[SerializeField]
		private bool overrideDungeon = false;
		[ShowIf("@this.overrideDungeon==true")]
		[SerializeField]
		private DungeonInfo debugDungeon;
		[ShowIf("@this.overrideDungeon==true")]
		[SerializeField]
		private DungeonRoomInfo debugDungeonRoom;

		[RequiredBool(false)]
		[PropertySpace(SpaceBefore = 10)]
		[SerializeField]
		private bool overrideHeros = false;
		[ShowIf("@this.overrideHeros==true")]
		[SerializeField]
		private List<CharacterSetup> debugHeros;

		public bool OverrideDungeon { get => overrideDungeon; }
		public bool OverrideHeros { get => overrideHeros; }
		public List<CharacterSetup> DebugHeros { get => debugHeros; }
		public DungeonInfo DebugDungeon { get => debugDungeon; }
		public DungeonRoomInfo DebugDungeonRoom { get => debugDungeonRoom; }

		public void Initialize()
		{
		}

		public void Dispose()
		{
		}
	}
}
