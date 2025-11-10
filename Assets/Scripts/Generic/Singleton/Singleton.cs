using GamePlugins.Attributes;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace GamePlugins.Singleton
{
	public class Singleton<T> : AceOfBaseSingleton where T : Singleton<T>
	{
		#region Fields
		[SerializeField, ReadOnly]
		private bool initialized = false;

		protected static T instance;
		static readonly object _lock = new object();
		#endregion

		#region Properties
		public static T Instance
		{
			get
			{
				if (IsDestroying)
				{
					//Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' is being destroyed!");
					return null;
				}

				lock (_lock)
				{
					if (instance == null)
					{
						instance = (T)FindObjectOfType(typeof(T));

						if (FindObjectsOfType(typeof(T)).Length > 1)
						{
							Debug.LogError("[Singleton] Something went really wrong - there should never be more than 1 singleton!", instance);
							return instance;
						}

						if (instance == null)
						{
							GetAutoCreateSettings(out bool autoCreateRuntime, out bool autoCreateEditor);
							if (Application.isPlaying)
							{
								if (autoCreateRuntime)
								{
									CreateInstance();
								}
							}
							else
							{
#if UNITY_EDITOR
								if (autoCreateEditor)
								{
									CreateInstanceEditor();
								}
#endif
							}
						}
						else
						{
							instance.TryInitialize();//if instance was created by manually adding it to an object it is not initialzied
							Debug.Log("<color=#6e6e6e>[Singleton] Using instance already created: " + instance.gameObject.name + "</color>");
						}
					}
					return instance;
				}
			}
		}

		public virtual bool IsInitialized
		{
			get => initialized;
		}
		#endregion

		#region Methods
		protected static void CreateInstance()
		{
			GameObject singleton = new GameObject();
			instance = singleton.AddComponent<T>();
			singleton.name = "(singleton) " + typeof(T).ToString();
			Debug.Log($"<color=#6e6e6e>[Singleton] An instance of {typeof(T)} is needed in the scene, so '{singleton}' was created!</color>");
		}
		protected static void GetAutoCreateSettings(out bool autoCreateRuntime, out bool autoCreateEditor)
		{
			autoCreateRuntime = false;
			autoCreateEditor = false;
			System.Reflection.MemberInfo info = typeof(T);
			foreach (object attrib in info.GetCustomAttributes(true))
			{
				if (attrib is AutoCreateSingleton)
				{
					AutoCreateSingleton attr = (AutoCreateSingleton)attrib;
					autoCreateRuntime = attr.autoCreateRuntime;
					autoCreateEditor = attr.autoCreateEditor;
					break;
				}
			}
		}
		protected static bool GetDontDestryoOnLoadFlag()
		{
			System.Reflection.MemberInfo info = typeof(T);
			foreach (object attrib in info.GetCustomAttributes(true))
			{
				if (attrib is DontDestroyOnLoadSingleton)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Called one when is created
		/// </summary>
		protected virtual void InitializeInstance()
		{
			bool dontDestroyOnLoad = GetDontDestryoOnLoadFlag();
			if (dontDestroyOnLoad && Application.isPlaying)
			{
				DontDestroyOnLoad(instance);
			}
			initialized = true;
		}

		/// <summary>
		/// Called when play mode starts
		/// </summary>
		protected virtual void OnAwakeCalled()
		{
			TryInitialize();
		}

		/// <summary>
		/// Only works at runtime
		/// </summary>
		public static void InstantiateSingleton()
		{
			if (!Instance)
			{
				CreateInstance();
			}
		}

		protected void ResetIntializedFlag()
		{
			initialized = false;
		}

		public GameObject InstantiateGameObject(GameObject prefab)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab);
			else
#endif
				return Instantiate(prefab);
		}

		private void TryInitialize()
		{
			if (!initialized)
			{
				InitializeInstance();
			}
		}
#if UNITY_EDITOR
		public static void CreateInstanceEditor()
		{
			if (instance)
				return;

			if (Application.isPlaying)
			{
				Debug.Log("[Singleton] Can't create editor instance of " + typeof(T) + " in play mode!");
			}
			else
			{
				instance = (T)FindObjectOfType(typeof(T));

				if (FindObjectsOfType(typeof(T)).Length > 1)
				{
					Debug.LogError("[Singleton] Something went really wrong - there should never be more than 1 singleton!", instance);
				}

				if (instance)
				{
					instance.TryInitialize();//if instance was created by manually adding it to an object it is not initialzied
					return;
				}

				CreateInstance();

				instance.InitializeInstance();

				UnityEditor.Undo.RegisterCreatedObjectUndo(instance.gameObject, "Create " + instance.gameObject.name);
			}
		}
#endif
		#endregion

		#region MonoBehaviour
		protected void Awake()
		{
			IsDestroying = false;
			if (instance == null)
			{
				instance = this as T;
				if (Application.isPlaying)
				{
					OnAwakeCalled();
				}
			}
			else if (instance == this)//someone already called Instance so the isntance is set but awake is not called yet
			{
				if (Application.isPlaying)
				{
					OnAwakeCalled();
				}
			}
			else
			{
				Debug.LogWarning("[Singleton] there should never me more than 1 singleton! " + gameObject, this);
				enabled = false;
				Destroy(this);
				return;
			}

		}

		protected virtual void OnDestroy()
		{
			if (instance == this)
			{
				if (GetDontDestryoOnLoadFlag())
					IsDestroying = true;
				instance = null;
			}
		}
		#endregion
	}
}