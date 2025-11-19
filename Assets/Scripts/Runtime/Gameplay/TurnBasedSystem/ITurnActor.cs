using Game.Stats;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Gameplay
{
	public interface ITurnActor
	{
		public event Action OnTurnCompleted;
		string ID { get; }
		public IReadOnlyList<ITurnAction> GetActions();
		public ITurnAction GetActiveAction();
		public void SetActiveAction(ITurnAction action);
		public bool HasAnyActions();
		public void TurnStart();
		public void TurnEnd();
		Sprite GetActorIcon();
		Color GetTeamColor();
		IStatValue GetStatValue(StatType type);
	}
}