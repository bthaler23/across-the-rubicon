using Game.Data;
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

		[RequiredBool(false)]
		[PropertySpace(SpaceBefore = 10)]
		[SerializeField]
		private bool overrideHeros = false;
		[ShowIf("@this.overrideHeros==true")]
		[SerializeField]
		private List<ActorInfo> debugHeros;

		public bool OverrideDungeon { get => overrideDungeon; }
		public bool OverrideHeros { get => overrideHeros; }
		public List<ActorInfo> DebugHeros { get => debugHeros; }
		public DungeonInfo DebugDungeon { get => debugDungeon; }

		public void Initialize()
		{
		}

		public void Dispose()
		{
		}
	}
}
