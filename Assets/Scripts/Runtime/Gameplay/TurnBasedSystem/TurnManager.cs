using Game.Events;
using GamePlugins.Events;
using GamePlugins.Utils;
using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Gameplay
{
	public struct TurnTracker
	{
		public ITurnActor Actor;
		public float TurnMeter;
		public TurnTracker(ITurnActor actor, float turnMeter)
		{
			Actor = actor;
			TurnMeter = turnMeter;
		}

		public void IncreaseMeter(float value)
		{
			Assert.IsTrue(value > 0, "Increase amount must be greater than zero.");
			TurnMeter += value;
		}

		public void DecreaseMeter(float value)
		{
			Assert.IsTrue(value > 0, "Decrese amount must be greater than zero.");
			TurnMeter -= value;
		}
	}

	public class TurnManager : MonoBehaviour, IResource
	{
		[SerializeField]
		private int turnMeterThreshold = 100;
		[SerializeField]
		private int turnRoundsToDisplay = 2;
		[ShowInInspector, ReadOnly]
		private ITurnActor currentActor;
		[ShowInInspector, ReadOnly]
		private List<ITurnActor> actors;
		[ShowInInspector, ReadOnly]
		private List<TurnTracker> turnTracker;
		private OnEndGame endGameCallback;
		private EndGameCondition endGameCondition;

		public int TurnMeterThreshold { get => turnMeterThreshold; }

		public delegate void OnEndGame();
		public delegate bool EndGameCondition();

		public void Initialize()
		{
			EventBus.Subscribe<OnCharacterDiedEvent>(OnCharacterDied);
		}

		public void Initialize(List<ITurnActor> actors, EndGameCondition endGameCondition, OnEndGame endGameCallback)
		{
			this.endGameCondition = endGameCondition;
			this.endGameCallback = endGameCallback;
			this.actors = new List<ITurnActor>();
			this.actors.AddRange(actors);

			foreach (var actor in this.actors)
			{
				actor.OnTurnCompleted += SignalActorDone;
			}

			InitializeCharactersMeterTurn();
		}

		private void InitializeCharactersMeterTurn()
		{
			turnTracker = new List<TurnTracker>();
			foreach (var actor in actors)
			{
				turnTracker.Add(new TurnTracker(actor, 0));
			}

			while (HasActionableActor(turnTracker) == false)
			{
				ProcessTicks(turnTracker);
				SortActorsBasedOnTurns(turnTracker);
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
			turnTracker = null;
			EventBus.Unsubscribe<OnCharacterDiedEvent>(OnCharacterDied);
		}

		public void StartTurns()
		{
			StartCoroutine(ProceedToNextActor(false));
		}

		private void OnCharacterDied(OnCharacterDiedEvent eventParam)
		{
			var actor = eventParam.deadActor;
			if (actor != null && actors.Contains(actor))
			{
				actors.Remove(actor);
				turnTracker.RemoveAll(t => t.Actor == actor);
			}
		}

		IEnumerator ProceedToNextActor(bool increaseTurnMeter)
		{
			if (increaseTurnMeter)
				yield return StartCoroutine(UpdateMetersCO());

			ITurnActor nextActor = null;
			for (int i = 0; i < turnTracker.Count; i++)
			{
				var turn = turnTracker[i];
				if (turn.Actor.HasAnyActions())
				{
					nextActor = turn.Actor;
					break;
				}
			}

			if (nextActor != null)
			{
				// Start their turn
				currentActor = nextActor;
				currentActor.TurnStart();

				EventBus.Publish<TurnChangeEvent>(new TurnChangeEvent(CalulateTurnsforUI(), true));
			}
			else
			{
				endGameCallback();
			}
			yield return null;
		}

		private IEnumerator UpdateMetersCO()
		{
			while (HasActionableActor(turnTracker) == false)
			{
				ProcessTicks(turnTracker);
				SortActorsBasedOnTurns(turnTracker);
				EventBus.Publish<TurnChangeEvent>(new TurnChangeEvent(CalulateTurnsforUI(), false));
				yield return null;
			}

			foreach (var turn in turnTracker)
			{
				Debug.Log($"Actor {turn.Actor.ID} Turn Meter: {turn.TurnMeter}");
			}
		}

		// Called by an actor when they finish their turn
		public void SignalActorDone()
		{
			// End of turn
			HandleEndTurn();

			// Check for win conditions
			if (endGameCondition())
			{
				endGameCallback();
				return;
			}
			// Move to next actor
			StartCoroutine(ProceedToNextActor(true));
		}

		private void HandleEndTurn()
		{
			currentActor.TurnEnd();
			currentActor = null;
			var current = turnTracker[0];
			current.DecreaseMeter(turnMeterThreshold);
			turnTracker.RemoveAt(0);
			turnTracker.Add(current);
		}

		private List<ITurnActor> CalulateTurnsforUI()
		{
			int turnsToDisplay = Mathf.Max(turnTracker.Count, turnTracker.Count * turnRoundsToDisplay);
			return CalulateTurns(new List<TurnTracker>(turnTracker), turnsToDisplay);
		}

		private List<ITurnActor> CalulateTurns(List<TurnTracker> actors, int targetTurns)
		{
			List<ITurnActor> results = new List<ITurnActor>();

			while (results.Count < targetTurns)
			{
				//Pick turns
				while (HasActionableActor(actors) == true && results.Count <= targetTurns)
				{
					actors = SortActorsBasedOnTurns(actors);
					var actor = actors[0];
					results.Add(actor.Actor);
					actor.DecreaseMeter(turnMeterThreshold);
					actors.RemoveAt(0);
					actors.Add(actor);
				}

				//Do Ticks
				while (HasActionableActor(actors) == false)
				{
					ProcessTicks(actors);
				}
			}

			return results;
		}

		private void ProcessTicks(List<TurnTracker> turnTracker)
		{
			for (int i = 0; i < turnTracker.Count; i++)
			{
				var turn = turnTracker[i];
				turn.IncreaseMeter(turn.Actor.GetTurnSpeed());
				turnTracker[i] = turn;
			}
		}

		private bool? HasActionableActor(List<TurnTracker> actors)
		{
			if(actors.IsNullOrEmpty()) return null;
			foreach (var actor in actors)
			{
				if (actor.TurnMeter >= turnMeterThreshold)
					return true;
			}
			return false;
		}

		private List<TurnTracker> SortActorsBasedOnTurns(List<TurnTracker> actors)
		{
			actors.Sort((a, b) => b.TurnMeter.CompareTo(a.TurnMeter));
			return actors;
		}

		internal float GetTurnMeter(ITurnActor actor)
		{
			foreach (var turn in turnTracker)
			{
				if (turn.Actor == actor)
				{
					return turn.TurnMeter;
				}
			}
			return 0;
		}
	}
}