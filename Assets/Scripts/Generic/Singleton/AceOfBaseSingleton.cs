using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GamePlugins.Singleton
{
	public class AceOfBaseSingleton : MonoBehaviour
	{
		#region Static Fields
		static bool _isDestroying = false;
#if UNITY_EDITOR
		/// <summary>
		/// There is time(loading?) between ExitingEditMode EnteredPlayMode
		/// </summary>
		static PlayModeStateChange playModeState;
#endif
		#endregion

#if UNITY_EDITOR
		static AceOfBaseSingleton()
		{
			EditorApplication.playModeStateChanged += LogPlayModeState;
		}

		private static void LogPlayModeState(PlayModeStateChange state)
		{
			playModeState = state;
		}
#endif

		#region Properties
#if UNITY_EDITOR
		protected static bool IsPlaying => playModeState == PlayModeStateChange.EnteredPlayMode || playModeState == PlayModeStateChange.ExitingEditMode;
#endif

		protected static bool IsDestroying
		{
			get
			{
#if UNITY_EDITOR
				if (IsPlaying)
					return _isDestroying;
				else
					return false;
#else
				return _isDestroying;
#endif
			}
			set
			{
#if UNITY_EDITOR
				if (IsPlaying)
#endif
					_isDestroying = value;
			}
		}
		#endregion

		#region Methods
		//Order: Static > Abstract > Virtual > Override > Simple Methods > Eventhandlers
#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void Init()
		{
			//Execute in edit mode is causing awake not to be called, so we destroy the object to force Awake
			//InitOnDomainNotReloadPlay();
			_isDestroying = false;
		}
#endif
		#endregion
	}
}
