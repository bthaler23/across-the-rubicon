using Game.Gameplay;
using GamePlugins.Events;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Events
{
	public class TurnChangeEvent : IGenericEvent
	{
		public readonly List<ITurnActor> actors;
		public readonly bool isActiveTurn;

		public TurnChangeEvent(List<ITurnActor> actors, bool isActiveTurn)
		{
			this.actors = actors;
			this.isActiveTurn = isActiveTurn;
		}
	}

	public class ActiveActorRefreshEvent : IGenericEvent
	{
		public readonly ITurnActor active;

		public ActiveActorRefreshEvent(ITurnActor active)
		{
			this.active = active;
		}
	}
}
