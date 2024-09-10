using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
	public sealed class GameObjectCache
	{
		public struct CacheItem
		{
			public GameObject CacheObject;
			public float LastFreeTime;
		}

		List<CacheItem> instances = new List<CacheItem>();
		private float defaultReleaseAfterFree = 30f;
		private int defaultInstancePoolCount = 16;
		private bool isFromAsset = false;
		private GameObject prefab;
		private Dictionary<GameObject, GameObjectCache> lookup;
		private PoolStrategy poolStrategy;
		private int spawnCount;
		private float releaseAfterFree;
		private int instancePoolCount;
		private Predicate<CacheItem> sweepChecker;

		internal GameObjectCache(Dictionary<GameObject, GameObjectCache> lookup)
		{
			sweepChecker = instance =>
			{
				if (instance.CacheObject != null)
				{
					if (Time.realtimeSinceStartup <= instance.LastFreeTime + releaseAfterFree)
						return false;
					UnityEngine.Object.Destroy(instance.CacheObject);
					return true;
				}
				Debug.LogErrorFormat("The instance of prefab '{0}' in the pool has been destroy.", prefab.name);
				return true;
			};
			this.lookup = lookup;
		}

		public string Error { get; private set; }
		public bool Loading { get; set; }

		public int SpawnCount => spawnCount;
		public int CacheCount => instances.Count;
		public float ReleaseAfterFree => releaseAfterFree;

		public int InstancePoolCount => instancePoolCount;

		public List<CacheItem> Instances => instances;
		internal bool HasPrefab => prefab != null;

		public float DefaultReleaseAfterFree
		{
			get => defaultReleaseAfterFree;
			set
			{
				defaultReleaseAfterFree = value;
				releaseAfterFree =
					poolStrategy != null ? poolStrategy.InstanceReleaseAfterFree : defaultReleaseAfterFree;
			}
		}

		public int DefaultInstancePoolCount
		{
			get => defaultInstancePoolCount;
			set
			{
				defaultInstancePoolCount = value;
				instancePoolCount = poolStrategy != null ? poolStrategy.InstancePoolCount : defaultInstancePoolCount;
			}
		}

		internal void SetPrefab(GameObject prefab)
		{
			this.prefab = prefab;
			poolStrategy = prefab.GetComponent<PoolStrategy>();
			if (poolStrategy != null)
			{
				releaseAfterFree = poolStrategy.InstanceReleaseAfterFree;
				instancePoolCount = poolStrategy.InstancePoolCount;
			}
			else
			{
				releaseAfterFree = DefaultReleaseAfterFree;
				instancePoolCount = DefaultInstancePoolCount;
			}
		}

		internal void LoadPrefab(AssetID assetId)
		{
			Loading = true;
			LoadPrefabImpl(assetId);
		}

		internal void ClearPrefab()
		{
			if (prefab == null) return;
			if (isFromAsset) Singleton<PrefabPool>.Instance.Free(prefab);
			prefab = null;
		}

		internal GameObject Spawn(Transform parent)
		{
			spawnCount++;
			while (instances.Count > 0)
			{
				var index = instances.Count - 1;
				var instance = instances[index];
				instances.RemoveAt(index);
				var cacheObject = instance.CacheObject;
				if (cacheObject != null)
				{
					cacheObject.SetActive(true);
					cacheObject.transform.SetParent(parent);
					cacheObject.transform.localPosition = prefab.transform.localPosition;
					cacheObject.transform.localRotation = prefab.transform.localRotation;
					cacheObject.transform.localScale = prefab.transform.localScale;
					lookup.Add(cacheObject, this);
					return cacheObject;
				}
				Debug.LogErrorFormat("The instance of prefab '{0}' in the pool has been destroy.", prefab.name);
			}
			var key = Singleton<PrefabPool>.Instance.Instantiate(prefab);
			key.transform.SetParent(parent, false);
			lookup.Add(key, this);
			return key;
		}

		internal void Spawn(InstantiateQueue queue, int priority, Action<GameObject> callback)
		{
			spawnCount++;
			if (instances.Count > 0)
			{
				var index = instances.Count - 1;
				var instance = instances[index];
				instances.RemoveAt(index);
				var go = instance.CacheObject;
				Assert.IsNotNull(go);
				go.SetActive(true);
				go.transform.localPosition = prefab.transform.localPosition;
				go.transform.localRotation = prefab.transform.localRotation;
				go.transform.localScale = prefab.transform.localScale;
				lookup.Add(go, this);
				callback(go);
			}
			else
			{
				queue.Instantiate(prefab, priority, go =>
				{
					lookup.Add(go, this);
					callback(go);
				});
			}
		}

		internal void PutBack(GameObject instance, bool destroy)
		{
			spawnCount--;
			if (destroy || instances.Count >= instancePoolCount)
				UnityEngine.Object.Destroy(instance);
			else
			{
				instances.Add(new CacheItem
				{
					CacheObject = instance,
					LastFreeTime = Time.realtimeSinceStartup
				});
			}
		}

		internal void SweepCache()
		{
			instances.RemoveAll(sweepChecker);
		}

		internal void ClearCache()
		{
			foreach (var instance in instances)
				UnityEngine.Object.Destroy(instance.CacheObject);
			instances.Clear();
		}

		private void LoadPrefabImpl(AssetID assetId)
		{
			Singleton<PrefabPool>.Instance.Load(assetId, go =>
			{
				if (go == null)
					Error = $"This asset: {assetId} is not a GameObject";
				else
				{
					SetPrefab(go);
					isFromAsset = true;
				}
			});
		}
	}
}