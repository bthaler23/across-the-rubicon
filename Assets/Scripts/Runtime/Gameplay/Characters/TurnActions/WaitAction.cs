using UnityEngine;
namespace Game
{
	public class WaitAction : BaseActorAction
	{
		protected override void OnExecuteInvoked()
		{
			base.UIInvokeExecute();
			StartCoroutine(DelayedExecuteAction(0.001f, () => { FireOnCompletedEvent(); }));
		}

		public override bool IsAvailable()
		{
			return true;
		}
	}
}