using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlugins.Events
{
	public interface IGenericEvent
	{
	}

	public class EventBus
	{
		private static EventBus _instance;
		private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

		private static void EnsureInstance()
		{
			if (_instance == null)
			{
				_instance = new EventBus();
			}
		}

		public static void Subscribe<T>(Action<T> callback) where T : IGenericEvent
		{
			EnsureInstance();
			var type = typeof(T);
			if (!_instance._subscribers.ContainsKey(type))
			{
				_instance._subscribers[type] = new List<Delegate>();
			}

			var delegates = _instance._subscribers[type];
			if (!delegates.Contains(callback))
				delegates.Add(callback);
		}

		// Unsubscribe from an event of type T
		public static void Unsubscribe<T>(Action<T> callback) where T : IGenericEvent
		{
			EnsureInstance();
			var type = typeof(T);
			if (_instance._subscribers.TryGetValue(type, out var list))
			{
				list.Remove(callback);
				if (list.Count == 0)
					_instance._subscribers.Remove(type);
			}
		}

		// Publish an event of type T
		public static void Publish<T>(T evt) where T : IGenericEvent
		{
			EnsureInstance();
			var type = typeof(T);
			if (_instance._subscribers.TryGetValue(type, out var list))
			{
				// Copy to avoid modification during iteration
				var listeners = list.ToArray();
				foreach (var del in listeners)
				{
					(del as Action<T>)?.Invoke(evt);
				}
			}
		}

		// Optional: Clear all subscribers (e.g., on scene unload)
		public static void Clear()
		{
			_instance._subscribers.Clear();
		}
	}
}
