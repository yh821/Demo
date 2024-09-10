using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
	[DisallowMultipleComponent]
	public class GameObjectPool : Singleton<GameObjectPool>
	{
		private Dictionary<AssetID, GameObjectCache> assetCaches = new Dictionary<AssetID, GameObjectCache>();
		private Dictionary<GameObject, GameObjectCache> objectCaches = new Dictionary<GameObject, GameObjectCache>();
		private Dictionary<GameObject, GameObjectCache> lookup = new Dictionary<GameObject, GameObjectCache>();
		private float defaultReleaseAfterFree = 30;
		private int defaultInstancePoolCount = 16;
		private Transform root;
		private Func<AssetID, GameObjectCache, bool> sweepAssetChecker;
		private Func<GameObject, GameObjectCache, bool> sweepObjectChecker;

		public GameObjectPool()
		{
			sweepAssetChecker = (id, cache) =>
			{
				if (!string.IsNullOrEmpty(cache.Error)) return true;
				cache.SweepCache();
				if (cache.Loading || cache.CacheCount != 0 || cache.SpawnCount != 0)
					return false;
				cache.ClearPrefab();
				return true;
			};
			sweepObjectChecker = (go, cache) =>
			{
				if (!string.IsNullOrEmpty(cache.Error)) return true;
				cache.SweepCache();
				if (cache.Loading || cache.CacheCount != 0 || cache.SpawnCount != 0)
					return false;
				cache.ClearPrefab();
				return true;
			};
			Scheduler.AddFrameListener(SweepCache);
			var go = new GameObject(nameof(GameObjectPool));
			go.SetActive(false);
			UnityEngine.Object.DontDestroyOnLoad(go);
			root = go.transform;
		}

		public float DefaultReleaseAfterFree
		{
			get => defaultReleaseAfterFree;
			set
			{
				defaultReleaseAfterFree = value;
				foreach (var cache in assetCaches.Values)
					cache.DefaultReleaseAfterFree = value;
				foreach (var cache in objectCaches.Values)
					cache.DefaultReleaseAfterFree = value;
			}
		}

		public int DefaultInstancePoolCount
		{
			get => defaultInstancePoolCount;
			set
			{
				defaultInstancePoolCount = value;
				foreach (var cache in assetCaches.Values)
					cache.DefaultInstancePoolCount = value;
				foreach (var cache in objectCaches.Values)
					cache.DefaultInstancePoolCount = value;
			}
		}

		public IDictionary<AssetID, GameObjectCache> AssetCaches => assetCaches;
		public IDictionary<GameObject, GameObjectCache> ObjectCaches => objectCaches;

		public WaitSpawnGameObject SpawnAsset(string bundleName, string assetName)
		{
			return SpawnAssetWithQueue(bundleName, assetName, null, 0);
		}

		public WaitSpawnGameObject SpawnAsset(AssetID assetId)
		{
			return SpawnAssetWithQueue(assetId, null, 0);
		}

		public WaitSpawnGameObject SpawnAssetWithQueue(string bundleName, string assetName,
			InstantiateQueue instantiateQueue, int instantiatePriority)
		{
			return SpawnAssetWithQueue(new AssetID(bundleName, assetName), instantiateQueue, instantiatePriority);
		}

		public WaitSpawnGameObject SpawnAssetWithQueue(AssetID assetId, InstantiateQueue instantiateQueue,
			int instantiatePriority)
		{
			Assert.IsFalse(assetId.IsEmpty);
			if (!assetCaches.TryGetValue(assetId, out var cache))
			{
				cache = new GameObjectCache(lookup);
				cache.DefaultReleaseAfterFree = defaultReleaseAfterFree;
				cache.DefaultInstancePoolCount = defaultInstancePoolCount;
				assetCaches.Add(assetId, cache);
				cache.LoadPrefab(assetId);
			}
			return new WaitSpawnGameObject(cache, instantiateQueue, instantiatePriority);
		}


		public GameObject Spawn(GameObject prefab, Transform parent)
		{
			Assert.IsNotNull(prefab);
			if (!objectCaches.TryGetValue(prefab, out var cache))
			{
				cache = new GameObjectCache(lookup);
				cache.DefaultReleaseAfterFree = defaultReleaseAfterFree;
				cache.DefaultInstancePoolCount = defaultInstancePoolCount;
				objectCaches.Add(prefab, cache);
				cache.SetPrefab(prefab);
			}
			return cache.Spawn(parent);
		}

		public T Spawn<T>(T component, Transform parent) where T : Component
		{
			Assert.IsNotNull(component);
			var t = Spawn(component.gameObject, parent).GetComponent<T>();
			return t != null ? t : default;
		}

		public void SetDefaultReleaseAfterFree(AssetID assetId, int value)
		{
			if (!assetCaches.TryGetValue(assetId, out var go))
				return;
			go.DefaultReleaseAfterFree = value;
		}

		public void Free(GameObject instance, bool destroy = false)
		{
			if (instance == null)
				Debug.LogError("Try to free a null GameObject!");
			else if (root == null)
				UnityEngine.Object.Destroy(instance);
			else
			{
				if (lookup.TryGetValue(instance, out var cache))
				{
					lookup.Remove(instance);
					instance.SetActive(false);
					instance.transform.SetParent(root);
					cache.PutBack(instance, destroy);
				}
				else UnityEngine.Object.Destroy(instance);
			}
		}

		public void Clear()
		{
			foreach (var cache in assetCaches.Values)
				cache.ClearCache();
			assetCaches.Clear();
			foreach (var cache in objectCaches.Values)
				cache.ClearCache();
			objectCaches.Clear();
			lookup.Clear();
		}

		public void ClearAllUnused()
		{
			assetCaches.RemoveAll((id, cache) =>
			{
				if (!string.IsNullOrEmpty(cache.Error)) return true;
				cache.ClearCache();
				if (cache.Loading || cache.CacheCount != 0 || cache.SpawnCount != 0) return false;
				cache.ClearPrefab();
				return true;
			});
			objectCaches.RemoveAll((id, cache) =>
			{
				if (!string.IsNullOrEmpty(cache.Error)) return true;
				cache.ClearCache();
				if (cache.Loading || cache.CacheCount != 0 || cache.SpawnCount != 0) return false;
				cache.ClearPrefab();
				return true;
			});
		}

		private void SweepCache()
		{
			assetCaches.RemoveAll(sweepAssetChecker);
			objectCaches.RemoveAll(sweepObjectChecker);
		}
	}
}