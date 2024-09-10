using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class SpritePool : Singleton<SpritePool>
	{
		private Dictionary<AssetID, SpriteCache> caches = new Dictionary<AssetID, SpriteCache>();
		Dictionary<Sprite, SpriteCache> lookup = new Dictionary<Sprite, SpriteCache>();
		private float defaultReleaseAfterFree = 30f;
		private int unloadedCountInTime = 0;
		private float lastCheckSweepTime = 0;
		Queue<SpriteLoadItem> loadQueue = new Queue<SpriteLoadItem>();
		private int maxLoadingCount = 1;
		private int loadingCount = 0;
		private Func<AssetID, SpriteCache, bool> sweepChecker;

		public Dictionary<AssetID, SpriteCache> Caches => caches;

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

		public SpritePool()
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
			QueueLoadSprite();
			if (Time.time - lastCheckSweepTime < 1) return;
			lastCheckSweepTime = Time.time;
			unloadedCountInTime = 0;
			caches.RemoveAll(sweepChecker);
		}

		private void QueueLoadSprite()
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

		public bool Retain(Sprite sprite)
		{
			if (sprite == null) return false;
			if (!lookup.TryGetValue(sprite, out var prefabCache)) return false;
			prefabCache.Retain();
			return true;
		}

		public void Free(Sprite sprite, bool destroy = false)
		{
			if (sprite == null) return;
			if (!lookup.TryGetValue(sprite, out var prefabCache)) return;
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

		public void Load(AssetID assetId, Action<Sprite> complete, bool isSync = false)
		{
			if (!assetId.AssetName.EndsWith(".png"))
				assetId.AssetName += ".png";
			if (AssetManager.Simulator.SimulateAssetBundle)
				Scheduler.RunCoroutine(LoadAsyncImpl(assetId, complete));
			else
			{
				assetId.AssetName = assetId.AssetName.ToLower();
				if (isSync) Scheduler.RunCoroutine(LoadSyncImpl(assetId, complete));
				else if (!AssetManager.IsVersionCached(assetId.BundleName))
					Scheduler.RunCoroutine(LoadAsyncImpl(assetId, complete));
				else loadQueue.Enqueue(new SpriteLoadItem(assetId, complete));
			}
		}

		public IEnumerator LoadAsyncImpl(AssetID assetId, Action<Sprite> complete)
		{
			var wait = InternalLoad(assetId, false);
			if (wait.keepWaiting) yield return wait;
			complete(string.IsNullOrEmpty(wait.Error) ? wait.LoadedObject : null);
		}

		public IEnumerator LoadAsyncImplInQueueLoad(AssetID assetId, Action<Sprite> complete)
		{
			var wait = InternalLoad(assetId, false);
			if (wait.keepWaiting) yield return wait;
			loadingCount--;
			if (loadingCount < 0) Debug.LogError($"[PrefabPool] loadingCount is occur error {loadingCount}");
			complete(string.IsNullOrEmpty(wait.Error) ? wait.LoadedObject : null);
		}

		public IEnumerator LoadSyncImpl(AssetID assetId, Action<Sprite> complete)
		{
			var wait = InternalLoad(assetId, true);
			if (wait.keepWaiting) yield return wait;
			complete(string.IsNullOrEmpty(wait.Error) ? wait.LoadedObject : null);
		}

		public WaitLoadSprite InternalLoad(AssetID assetId, bool isSync)
		{
			if (caches.TryGetValue(assetId, out var cache1))
			{
				cache1.Retain();
				return new WaitLoadSprite(cache1);
			}
			var cache2 = new SpriteCache(assetId, lookup);
			cache2.DefaultReleaseAfterFree = DefaultReleaseAfterFree;
			cache2.IsSync = isSync;
			cache2.LoadObject(assetId);
			cache2.Retain();
			caches.Add(assetId, cache2);
			return new WaitLoadSprite(cache2);
		}
	}
}