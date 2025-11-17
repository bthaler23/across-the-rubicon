using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Gameplay
{
	[Serializable]
	public class TeamActors
	{
		public string TeamID { get; private set; }

		[ShowInInspector, ReadOnly]
		private List<ITurnActor> actors;

		public TeamActors(string teamID, List<ITurnActor> actors)
		{
			this.TeamID = teamID;
			this.actors = actors;
		}

		public bool HasActiveActors()
		{
			return actors.Any(a => a.HasAnyActions());
		}

		public bool TryGetActor(int index, out ITurnActor actor)
		{
			actor = null;
			if (index < 0 || index >= actors.Count)
			{
				return false;
			}
			actor = actors[index];
			return true;
		}
	}
}
