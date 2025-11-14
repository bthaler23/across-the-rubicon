using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Gameplay
{
	public class GameFlowManager : MonoBehaviour
	{
		[SerializeField]
		private TurnManager turnManager;
		[SerializeField]
		private List<TeamActors> teams;

		public void Inintialize(List<TeamActors> teams)
		{
			this.teams = teams;
			StartCoroutine(Initialize_CO());
		}

		private IEnumerator Initialize_CO()
		{
			turnManager.Initialize(GetTurnOrder(teams), CheckEndGameCondition, OnGameEnded);
			yield return null;
			turnManager.StartTurns();
		}

		private List<ITurnActor> GetTurnOrder(List<TeamActors> teams)
		{
			// Interleave actors round-robin: first of each team, then second of each team, etc.,
			// using TeamActors.TryGetActor to access actors safely.
			var order = new List<ITurnActor>();
			if (teams == null || teams.Count == 0)
			{
				return order;
			}

			int index = 0;
			while (true)
			{
				bool addedAny = false;
				foreach (var team in teams)
				{
					if (team == null) continue;
					if (team.TryGetActor(index, out ITurnActor actor) && actor != null)
					{
						order.Add(actor);
						addedAny = true;
					}
				}

				if (!addedAny)
				{
					break; // No teams had an actor at this index; we're done.
				}
				index++;
			}

			return order;
		}

		private void OnGameEnded()
		{
			Debug.Log("Game Ended");
		}

		private bool CheckEndGameCondition()
		{
			// End when zero or one teams still have actors with available actions.
			if (teams == null || teams.Count == 0)
			{
				return true;
			}

			int activeTeams = 0;
			for (int i = 0; i < teams.Count; i++)
			{
				var team = teams[i];
				if (team == null) continue;
				if (team.HasActiveActors())
				{
					activeTeams++;
					if (activeTeams > 1)
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
