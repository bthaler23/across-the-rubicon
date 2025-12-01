using Game;
using System.Collections;
using UnityEngine;

namespace Game.Core
{
	public class PlayFromEntryPoint : MonoBehaviour
	{
		[SerializeField]
		private bool disableGameplayLoad = false;

		void Awake()
		{
#if UNITY_EDITOR
			if (!disableGameplayLoad && GameManager.Instance == null)//if not started from always loaded scene - load that and it will take care of the rest
				GameManager.LoadGameplayCO_Editor();
#endif
		}
	}
}
