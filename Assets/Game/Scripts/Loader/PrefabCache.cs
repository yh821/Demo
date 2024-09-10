using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public sealed class PrefabCache
	{
		private float lastFreeTime = -1f;
		private float defaultReleaseAfterFree = 30;
		private GameObject cacheObject;
		private int referenceCount;
		private AssetID assetId;
		private IDictionary<GameObject, PrefabCache> lookup;
		private PoolStrategy poolStrategy;
		private float releaseAfterFree;
		private bool isSync;

		internal PrefabCache(AssetID assetId, IDictionary<GameObject, PrefabCache> lookup)
		{
			this.assetId = assetId;
			this.lookup = lookup;
		}

		public AssetID AssetId => assetId;

		public int ReferenceCount => referenceCount;

		public float LastFreeTime => lastFreeTime;

		public float ReleaseAfterFree => releaseAfterFree;

		public bool IsSync
		{
			get => isSync;
			set => isSync = value;
		}

		public string Error { get; private set; }

		public float DefaultReleaseAfterFree
		{
			get => defaultReleaseAfterFree;
			set
			{
				defaultReleaseAfterFree = value;
				releaseAfterFree = poolStrategy != null ? poolStrategy.PrefabReleaseAfterFree : defaultReleaseAfterFree;
			}
		}

		internal void Retain()
		{
			referenceCount++;
		}

		internal void Release()
		{
			referenceCount--;
			lastFreeTime = Time.time;
			if (referenceCount < 0)
				Debug.LogErrorFormat("[PrefabCache] referenceCount is error {0} {1}", assetId.ToString(),
					referenceCount);
		}

		internal void LoadObject(AssetID assetId)
		{
			Scheduler.RunCoroutine(LoadObjectImpl(assetId));
		}

		internal bool HasLoaded()
		{
			return cacheObject != null;
		}

		internal GameObject GetObject()
		{
			return cacheObject;
		}

		private IEnumerator LoadObjectImpl(AssetID assetId)
		{
			WaitLoadObject waitObj = null;
			if (isSync) waitObj = AssetManager.LoadObjectSync(this.assetId, typeof(GameObject));
			else waitObj = AssetManager.LoadObject(assetId, typeof(GameObject));
			yield return waitObj;
			if (string.IsNullOrEmpty(waitObj.Error))
			{
				cacheObject = waitObj.GetObject() as GameObject;
				if (cacheObject == null)
					Error = $"This asset: {this.assetId} is not a GameObject!";
				else
				{
					poolStrategy = cacheObject.GetComponent<PoolStrategy>();
					releaseAfterFree = poolStrategy == null
						? DefaultReleaseAfterFree
						: poolStrategy.InstanceReleaseAfterFree;
					if (lookup.ContainsKey(cacheObject)) lookup[cacheObject] = this;
					else lookup.Add(cacheObject, this);
				}
			}
			else Error = waitObj.Error;
		}
	}
}