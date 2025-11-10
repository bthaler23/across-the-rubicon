using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GamePlugins.ObjectPool
{
	public class ObjectPool : MonoBehaviour
	{
		public List<PrefabAndCount> sceneAssetsToPreload;
		public event Action OnCachingComplete;

		private Dictionary<GameObject, AdvancedObjectPool> pools;
		private static ObjectPool _instance;

		private Transform xForm;

		public static ObjectPool Instance { get { return _instance; } }

		public Dictionary<GameObject, AdvancedObjectPool> Pools { get { return pools; } }

		private void Awake()
		{
			if (_instance == null)
			{
				xForm = transform;
				_instance = this;
				pools = new Dictionary<GameObject, AdvancedObjectPool>();
			}
			else
			{
				Debug.LogError("More than one Object pools found!!!", gameObject);
			}
		}

		private void OnDestroy()
		{
			if (_instance == this)
			{
				if (_cacheMainMenuCoroutine != null)
					StopCoroutine(_cacheMainMenuCoroutine);

				_instance = null;
			}
		}

		public void Cleanup()
		{
			Destroy(gameObject);
			//foreach (var item in Pools)
			//{
			//    item.Value.Cleanup();
			//}
			//pools.Clear();
		}

		public void CacheGameplay()
		{

		}

		public void CacheMainMenu()
		{
			if (_cacheMainMenuCoroutine != null)
				StopCoroutine(_cacheMainMenuCoroutine);
			_cacheMainMenuCoroutine = StartCoroutine(CacheMainMenuCO());
		}

		private Coroutine _cacheMainMenuCoroutine = null;

		private IEnumerator CacheMainMenuCO()
		{
			yield return null;

		}

		public void CachePrefab(GameObject prefab, Transform poolParent, int count = 1)
		{
			AdvancedObjectPool pool;
			if (pools.TryGetValue(prefab, out pool))
			{
				pool.Cleanup();
				pools.Remove(prefab);
			}

			count++; // need one more first time we add to pool, that will be destroyed after we show it to camera
			pools.Add(prefab, new AdvancedObjectPool(prefab, count, poolParent));
		}

		public bool IsCached(GameObject prefab)
		{
			if (pools.TryGetValue(prefab, out AdvancedObjectPool objectPool))
			{
				if (!objectPool.IsValid())
				{
					objectPool.Cleanup();
					pools.Remove(prefab);
					return false;
				}
				else
				{
					return true;
				}
			}
			else
				return false;
		}

		public void CachePrefab(GameObject prefab, int count = 1)
		{
			CachePrefab(prefab, xForm, count);
		}

		private GameObject GetObj(GameObject prefab)
		{
			AdvancedObjectPool objectPool;
			if (pools.TryGetValue(prefab, out objectPool))
			{
				return objectPool.GetNextElement();
			}
			else
			{
				objectPool = new AdvancedObjectPool(prefab, 1, xForm, false);
				pools.Add(prefab, objectPool);
				return objectPool.GetNextElement();
			}
		}

		private void ReturnObj(GameObject gObject)
		{
			foreach (AdvancedObjectPool pool in pools.Values)
			{
				if (pool.IsPartOfPool(gObject))
				{
					pool.ReturnElementToPool(gObject);
					return;
				}
			}
			gObject.gameObject.SetActive(false);
			Debug.LogError(gObject.name + " was not part of the ObjectPool, disabling it");
		}

		private bool ContainsObj(GameObject gObject)
		{
			foreach (AdvancedObjectPool pool in pools.Values)
			{
				if (pool.IsPartOfPool(gObject))
				{
					return true;
				}
			}
			return false;
		}

		public static T GetObject<T>(T objPrefab) where T : Component
		{
			if (_instance != null)
			{
				GameObject go = _instance.GetObj(objPrefab.gameObject);
				go.gameObject.SetActive(true);
				return go.GetComponent<T>();
			}
			return null;
		}

		public static GameObject GetObject(GameObject prefab)
		{
			if (_instance != null)
			{
				GameObject go = _instance.GetObj(prefab);
				go.gameObject.SetActive(true);
				return go;
			}
			return null;
		}

		public static void ReturnObject(GameObject gObject)
		{
			if (_instance != null)
			{
				_instance.ReturnObj(gObject);
			}
			else
			{
				gObject.SetActive(false);
				Debug.LogError("No Objectpool in scene, disabling object");
			}
		}

		public static bool ContainsObject(GameObject gObject)
		{
			if (_instance != null)
			{
				return _instance.ContainsObj(gObject);
			}
			else
			{
				Debug.LogError("No Objectpool in scene, disabling object");
				return false;
			}
		}

		#region Other Classes
		[Serializable]
		public class AdvancedObjectPool
		{
			private GameObject prefab;
			private Transform poolParent;
			protected HashSet<GameObject> freePool;
			protected int index;
			protected HashSet<GameObject> originalPool;
			private Transform poolsRoot;
			private int size;

			private bool poolCleanedUp = false;

			public void Cleanup()
			{
				poolCleanedUp = true;
				if (poolParent != null)
					Destroy(poolParent.gameObject);
				poolParent = null;
				freePool.Clear();
				originalPool.Clear();
			}

			public bool IsValid()
			{
				return poolParent != null;
			}

			public AdvancedObjectPool(GameObject goPrefab, int count, Transform poolsParent, bool autoDeactivateObjects = true)
			{
				InstantiatePool(goPrefab, count, poolsParent, autoDeactivateObjects);
				poolCleanedUp = false;
			}

			private Transform CreatePoolParentTransform(string poolName)
			{
				var obj = new GameObject();
				obj.transform.parent = poolsRoot;
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localRotation = Quaternion.identity;
				obj.transform.localScale = Vector3.one;
				obj.name = poolName;
				return obj.transform;
			}

			private void InstantiatePool(GameObject goPrefab, int count, Transform poolsParent, bool autoDeactivateObjects)
			{
				this.prefab = goPrefab;
				this.poolsRoot = poolsParent;
				size = count;
				poolParent = CreatePoolParentTransform(prefab.name);
				originalPool = new HashSet<GameObject>();
				freePool = new HashSet<GameObject>();
				for (int i = 0; i < size; i++)
				{
					AddElementToPool(autoDeactivateObjects);
				}
			}

			public GameObject GetNextElement()
			{
				if (!poolCleanedUp)
				{
					if (freePool.Count == 0)
					{
						size++;
						AddElementToPool(false);
						//BA: removed this for prototyping    Debug.LogError("WARNING: Object pool increased for " + prefab + " new object instantiated!");
					}

					foreach (GameObject item in freePool)
					{
						GameObject nextObject = item;
						freePool.Remove(nextObject);
						return nextObject;
					}
				}

				return null;//This should never be called
			}

			public void IncreasePoolSize(int sizeDiff)
			{
				for (int i = 0; i < sizeDiff; i++)
				{
					size++;
					AddElementToPool();
				}
			}

			public bool IsPartOfPool(GameObject gObj)
			{
				return originalPool.Contains(gObj);
			}

			public void ReturnElementToPool(GameObject obj)
			{
				if (!poolCleanedUp)
				{
					if (originalPool.Contains(obj))
					{//Only pool elements should be returned
						obj.SetActive(false);
						obj.transform.SetParent(poolParent, false);
						freePool.Add(obj);
					}
				}
			}
			public IEnumerator ReturnElementToPoolCR(GameObject obj)
			{
				if (originalPool.Contains(obj))
				{//Only pool elements should be returned
					obj.SetActive(false);
					obj.transform.SetParent(poolParent, false);
					yield return null;
					freePool.Add(obj);
				}
				yield return null;
			}

			public void RemoveElementFromPool(GameObject obj)
			{
				if (!poolCleanedUp)
				{
					originalPool.Remove(obj);
					if (freePool.Contains(obj))
					{
						freePool.Remove(obj);
					}
					size--;
				}
			}

			private void AddElementToPool(bool autoDeactivate = true)
			{
				GameObject newElement = Instantiate(prefab, poolParent);
				if (autoDeactivate)
				{
					newElement.gameObject.SetActive(false);
				}
				originalPool.Add(newElement);
				freePool.Add(newElement);
			}
		}

		[Serializable]
		public class PrefabAndCount
		{
			public int count;
			public GameObject prefab;
		}

		#endregion

	}
}