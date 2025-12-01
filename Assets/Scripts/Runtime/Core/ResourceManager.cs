using GamePlugins.Attributes;
using GamePlugins.Singleton;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	[DontDestroyOnLoadSingleton]
	[AutoCreateSingleton(true, false)]
	public class ResourceManager : Singleton<ResourceManager>
	{
		private Dictionary<Type, IResource> resourceStorage;

		protected override void OnAwakeCalled()
		{
			base.OnAwakeCalled();
			resourceStorage = new Dictionary<Type, IResource>();
		}

		public void RegisterResource<T>(IResource service) where T : IResource
		{
			if (!resourceStorage.ContainsKey(typeof(T)))
			{
				service.Initialize();
				resourceStorage.Add(typeof(T), service);
			}
			else
			{
				Debug.LogError($"Service {typeof(T)} already registered!");
			}
		}

		public void RemoveResource<T>() where T : IResource
		{
			if (resourceStorage.TryGetValue(typeof(T), out IResource resource))
			{
				resource.Dispose();
				resourceStorage.Remove(typeof(T));
			}
			else
			{
				Debug.LogWarning($"Service does not contain type {typeof(T)}, cannot be removed!");
			}
		}

		public T RequestResource<T>()
		{
			if (resourceStorage.TryGetValue(typeof(T), out IResource serviceRef) && serviceRef is T service)
			{
				return service;
			}
			Debug.LogError($"Service {typeof(T)} missing!");
			return default(T);
		}
	}

	public interface IResource
	{
		void Initialize();
		void Dispose();
	}
}
