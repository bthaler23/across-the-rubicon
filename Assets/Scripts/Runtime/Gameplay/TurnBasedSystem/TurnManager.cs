using Game.Events;
using GamePlugins.Events;
using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
	public class TurnManager : MonoBehaviour, IResource
	{
		[SerializeField]
		private int turnMeterThreshold = 100;
		[ShowInInspector, ReadOnly]
		private ITurnActor currentActor;
		[ShowInInspector, ReadOnly]
		private List<ITurnActor> actors;
		private OnEndGame endGameCallback;
		private EndGameCondition endGameCondition;

		public int TurnMeterThreshold { get => turnMeterThreshold; }

		public delegate void OnEndGame();
		public delegate bool EndGameCondition();

		public void Initialize(List<ITurnActor> actors, EndGameCondition endGameCondition, OnEndGame endGameCallback)
		{
			this.actors = actors;
			this.endGameCondition = endGameCondition;
			this.endGameCallback = endGameCallback;

			foreach (var actor in this.actors)
			{
				actor.OnTurnCompleted += SignalActorDone;
			}

			InitializeCharactersMeterTurn();
		}

		private void InitializeCharactersMeterTurn()
		{
			bool isThreasholdReached = false;
			while (!isThreasholdReached)
			{
				foreach (var actor in actors)
				{
					actor.TurnMeterTick();
					if (actor.GetTurnMeter() >= turnMeterThreshold)
					{
						isThreasholdReached = true;
					}
				}
			}
			SortActorsBasedOnTurns();
		}

		private void SortActorsBasedOnTurns()
		{
			actors.Sort((a, b) => b.GetTurnMeter().CompareTo(a.GetTurnMeter()));
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
			StartCoroutine(ProceedToNextActor(false));
		}

		IEnumerator ProceedToNextActor(bool increaseTurnMeter)
		{
			if (increaseTurnMeter)
				yield return StartCoroutine(UpdateMetersCO());
			bool foundActorWithActions = false;
			ITurnActor actor = null;
			int index = 0;
			while (!foundActorWithActions && index < actors.Count)
			{
				actor = actors[index];
				index++;
				// Skip if actor has no actions
				if (!actor.HasAnyActions())
				{
					Debug.Log(actor.ID + " skipped (no available actions)");
				}
				else
				{
					foundActorWithActions = true;
				}
			}

			if (foundActorWithActions)
			{
				// Start their turn
				currentActor = actor;
				currentActor.TurnStart();

				EventBus.Publish<TurnChangeEvent>(new TurnChangeEvent(actors, currentActor));
				EventBus.Publish<ActiveActorRefreshEvent>(new ActiveActorRefreshEvent(currentActor));
			}
			else
			{
				endGameCallback();
			}
			yield return null;
		}

		private IEnumerator UpdateMetersCO()
		{
			SortActorsBasedOnTurns();//need to establish order before updating meters
			EventBus.Publish<ActiveActorRefreshEvent>(new ActiveActorRefreshEvent(null));
			bool isThreasholdReached = false;
			while (!isThreasholdReached)
			{
				foreach (var actor in actors)
				{
					actor.TurnMeterTick();
					if (actor.GetTurnMeter() >= turnMeterThreshold)
					{
						isThreasholdReached = true;
					}
				}
				SortActorsBasedOnTurns();
				EventBus.Publish<TurnChangeEvent>(new TurnChangeEvent(actors, null));
				yield return null;
			}

			foreach (var actor in actors)
			{
				Debug.Log($"Actor {actor.ID} Turn Meter: {actor.GetTurnMeter()}");
			}
		}

		// Called by an actor when they finish their turn
		public void SignalActorDone()
		{
			// End of turn
			currentActor.TurnEnd();
			currentActor.ModifyTurnMeter(-turnMeterThreshold);
			int index = actors.IndexOf(currentActor);
			if (index >= 0)
			{
				actors.RemoveAt(index);
				actors.Add(currentActor);
			}
			currentActor = null;

			// Check for win conditions
			if (endGameCondition())
			{
				endGameCallback();
				return;
			}
			// Move to next actor
			StartCoroutine(ProceedToNextActor(true));
		}

		public void Initialize()
		{
		}
	}
}