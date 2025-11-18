using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Gameplay
{
	public interface ITurnAction
	{
		event Action OnActionCompleted;
		void Initialize(ITurnActor owner);
		bool IsAvailable();
		void ActivateAction();
		void DisableAction();
		Sprite GetIcon();
	}
}
