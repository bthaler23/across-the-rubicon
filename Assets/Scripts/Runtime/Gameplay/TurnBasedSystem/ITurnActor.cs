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
		public IReadOnlyList<TurnActionBase> GetActions();
		public TurnActionBase GetActiveAction();
		public void SetActiveAction(TurnActionBase action);
		public bool HasAnyActions();
		public bool IsAlive();
		public void TurnStart();
		public void TurnEnd();
		public float GetTurnSpeed();
		Sprite GetActorIcon();
		Color GetTeamColor();
		IStatValue GetStat(StatType type);
		Transform GetUIXform();
	}
}