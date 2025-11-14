using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
	public class TurnManager : MonoBehaviour
	{
		[ShowInInspector, ReadOnly]
		private int index;
		[ShowInInspector, ReadOnly]
		private List<ITurnActor> actors;
		private OnEndGame endGameCallback;
		private EndGameCondition endGameCondition;

		public delegate void OnEndGame();
		public delegate bool EndGameCondition();

		public void Initialize(List<ITurnActor> actors, EndGameCondition endGameCondition, OnEndGame endGameCallback)
		{
			index = -1;
			this.actors = actors;
			this.endGameCondition = endGameCondition;
			this.endGameCallback = endGameCallback;

			foreach(var actor in this.actors)
			{
				actor.OnTurnCompleted += SignalActorDone;
			}
		}

		public void Dispose()
		{
			endGameCallback = null;
			endGameCondition = null;
			foreach (var actor in actors)
			{
				actor.OnTurnCompleted -= SignalActorDone;
			}
			actors = null;
		}

		public void StartTurns()
		{
			ProceedToNextActor();
		}

		void ProceedToNextActor()
		{
			// Advance index
			index = (index + 1) % actors.Count;
			var actor = actors[index];

			// Skip if actor has no actions
			if (!actor.HasAnyActions())
			{
				Debug.Log(actor.ID + " skipped (no available actions)");
				ProceedToNextActor();
				return;
			}

			// Start their turn
			actor.TurnStart();
		}

		// Called by an actor when they finish their turn
		public void SignalActorDone()
		{
			// End of turn
			var actor = actors[index];
			actor.TurnEnd();

			// Check for win conditions
			if (endGameCondition())
			{
				endGameCallback();
				return;
			}
			// Move to next actor
			ProceedToNextActor();
		}
	}
}