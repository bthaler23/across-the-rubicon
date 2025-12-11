using Game.Gameplay;
using GamePlugins.Events;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	public class OnCharacterDiedEvent : IGenericEvent
	{
		public readonly ITurnActor deadActor;
		public OnCharacterDiedEvent(ITurnActor deadActor)
		{
			this.deadActor = deadActor;
		}
	}

	public class OnGameEndedEvent : IGenericEvent
	{
		public OnGameEndedEvent()
		{
		}
	}

	public class OnShowFloatingUiText : IGenericEvent
	{
		public readonly Transform worldTarget;
		public readonly string text;
		public readonly Color? textColor;
		public readonly Sprite icon;
		public OnShowFloatingUiText(Transform worldTarget, string text, Color? textColor, Sprite icon)
		{
			this.worldTarget = worldTarget;
			this.text = text;	
			this.textColor = textColor;
			this.icon = icon;
		}
	}
}
