using System;
using UnityEngine;

namespace Game.Gameplay
{
	public interface ITurnActor
	{
		public event Action OnTurnCompleted;
		string ID { get; }
		public bool HasAnyActions();
		public void TurnStart();
		public void TurnEnd();
		Sprite GetActorIcon();
		Color GetTeamColor();
		float GetHealthNormalized();
	}
}