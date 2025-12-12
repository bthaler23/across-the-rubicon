using Game.Gameplay;
using UnityEngine;
namespace Game
{
	public class WaitAction : TurnActionBase
	{
		protected override void ExecuteOnUIClickAction()
		{
			base.ExecuteOnUIClickAction();
			StartCoroutine(DelayedExecuteAction(0.001f, () => { FireOnCompletedEvent(); }));
		}

		public override bool IsAvailable()
		{
			return true;
		}
	}
}