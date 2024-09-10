using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class PrefabPool : Singleton<PrefabPool>
	{
		private Dictionary<AssetID, PrefabCache> caches = new Dictionary<AssetID, PrefabCache>();
		Dictionary<GameObject, PrefabCache> lookup = new Dictionary<GameObject, PrefabCache>();
		private float defaultReleaseAfterFree = 30f;
		private int unloadedCountInTime = 0;
		private float lastCheckSweepTime = 0;
		Queue<PrefabLoadItem> loadQueue = new Queue<PrefabLoadItem>();
		private int maxLoadingCount = 1;
		private int loadingCount = 0;
		private Func<AssetID, PrefabCache, bool> sweepChecker;

		public Dictionary<AssetID, PrefabCache> Caches => caches;

		public float DefaultReleaseAfterFree
		{
			get => defaultReleaseAfterFree;
			set
			{
				defaultReleaseAfterFree = value;
				foreach (var cache in caches.Values)
					cache.DefaultReleaseAfterFree = value;
			}
		}

		public PrefabPool()
		{
			sweepChecker = (assetId, cache) =>
			{
				if (!string.IsNullOrEmpty(cache.Error)) return true;
				if (cache.ReferenceCount > 0 || unloadedCountInTime >= 5 ||
				    Time.time <= cache.LastFreeTime + cache.ReleaseAfterFree)
					return false;
				unloadedCountInTime++;
				var key = cache.GetObject();
				if (key != null)
				{
					lookup.Remove(key);
					AssetManager.UnloadAssetBundle(assetId.BundleName);
				}
				return true;
			};
			Scheduler.AddFrameListener(Update);
		}

		private void Update()
		{
			QueueLoadPrefab();
			if (Time.time - lastCheckSweepTime < 1) return;
			lastCheckSweepTime = Time.time;
			unloadedCountInTime = 0;
			caches.RemoveAll(sweepChecker);
		}

		private void QueueLoadPrefab()
		{
			var num = maxLoadingCount + loadQueue.Count / 5;
			while (loadQueue.Count > 0 && num - loadingCount > 0)
			{
				loadingCount++;
				var item = loadQueue.Dequeue();
				Scheduler.RunCoroutine(LoadAsyncImplInQueueLoad(item.assetId, item.complete));
			}
		}

		public void SetMaxLoadingCount(int value)
		{
			maxLoadingCount = value;
		}

		public GameObject Instantiate(GameObject prefab)
		{
			if (prefab == null) return null;
			var go = UnityEngine.Object.Instantiate(prefab);
			var prefabRef = ReferenceDict.AddPrefabReference(go, prefab);
			go.GetOrAddComponent<PrefabReferenceHolder>().SetPrefabReference(prefabRef);
			return go;
		}

		public bool Retain(GameObject prefab)
		{
			if (prefab == null) return false;
			if (!lookup.TryGetValue(prefab, out var prefabCache)) return false;
			prefabCache.Retain();
			return true;
		}

		public void Free(GameObject prefab, bool destroy = false)
		{
			if (prefab == null) return;
			if (!lookup.TryGetValue(prefab, out var prefabCache)) return;
			prefabCache.Release();
			if (!destroy || prefabCache.ReferenceCount != 0) return;
			var assetId = prefabCache.AssetId;
			var key = prefabCache.GetObject();
			if (key != null)
			{
				AssetManager.UnloadAssetBundle(assetId.BundleName);
				lookup.Remove(key);
			}
			caches.Remove(assetId);
		}

		public void Clear()
		{
			caches.Clear();
			lookup.Clear();
		}

		public void ClearAllUnused()
		{
			caches.RemoveAll((assetId, cache) =>
			{
				if (!string.IsNullOrEmpty(cache.Error)) return true;
				if (cache.ReferenceCount > 0) return false;
				var key = cache.GetObject();
				if (key != null)
				{
					AssetManager.UnloadAssetBundle(assetId.BundleName);
					lookup.Remove(key);
				}
				return true;
			});
		}

		public void Load(AssetID assetId, Action<GameObject> complete, bool isSync = false)
		{
			if (!assetId.AssetName.EndsWith(".prefab"))
				assetId.AssetName += ".prefab";
			if (AssetManager.Simulator.SimulateAssetBundle)
				Scheduler.RunCoroutine(LoadAsyncImpl(assetId, complete));
			else
			{
				assetId.AssetName = assetId.AssetName.ToLower();
				if (isSync) Scheduler.RunCoroutine(LoadSyncImpl(assetId, complete));
				else if (!AssetManager.IsVersionCached(assetId.BundleName))
					Scheduler.RunCoroutine(LoadAsyncImpl(assetId, complete));
				else loadQueue.Enqueue(new PrefabLoadItem(assetId, complete));
			}
		}

		public IEnumerator LoadAsyncImpl(AssetID assetId, Action<GameObject> complete)
		{
			var wait = InternalLoad(assetId, false);
			if (wait.keepWaiting) yield return wait;
			complete(string.IsNullOrEmpty(wait.Error) ? wait.LoadedObject : null);
		}

		public IEnumerator LoadAsyncImplInQueueLoad(AssetID assetId, Action<GameObject> complete)
		{
			var wait = InternalLoad(assetId, false);
			if (wait.keepWaiting) yield return wait;
			loadingCount--;
			if (loadingCount < 0) Debug.LogError($"[PrefabPool] loadingCount is occur error {loadingCount}");
			complete(string.IsNullOrEmpty(wait.Error) ? wait.LoadedObject : null);
		}

		public IEnumerator LoadSyncImpl(AssetID assetId, Action<GameObject> complete)
		{
			var wait = InternalLoad(assetId, true);
			if (wait.keepWaiting) yield return wait;
			complete(string.IsNullOrEmpty(wait.Error) ? wait.LoadedObject : null);
		}

		public WaitLoadPrefab InternalLoad(AssetID assetId, bool isSync)
		{
			if (caches.TryGetValue(assetId, out var cache1))
			{
				cache1.Retain();
				return new WaitLoadPrefab(cache1);
			}
			var cache2 = new PrefabCache(assetId, lookup);
			cache2.DefaultReleaseAfterFree = DefaultReleaseAfterFree;
			cache2.IsSync = isSync;
			cache2.LoadObject(assetId);
			cache2.Retain();
			caches.Add(assetId, cache2);
			return new WaitLoadPrefab(cache2);
		}
	}
}