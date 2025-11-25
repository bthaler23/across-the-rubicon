using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Data
{
	[Serializable]
	public class DungeonRoomInfo
	{
		[BoxGroup("Grid")]
		[SerializeField]
		private Vector2Int gridDimensions;
		[BoxGroup("Grid")]
		[SerializeField]
		[PropertyRange(0, 1)]
		private float edgeRemovalChance;
		[BoxGroup("Grid")]
		[SerializeField]
		[PropertyRange(0, 1)]
		private float innerRemovalChance;

		[BoxGroup("Enemies")]
		[SerializeField]
		private ActorInfo[] enemyActors;

		public Vector2Int GetGridDimensions() => gridDimensions;
		public float GetEdgeRemovalChance() => edgeRemovalChance;
		public float GetInnerRemovalChance() => innerRemovalChance;
	}
}
