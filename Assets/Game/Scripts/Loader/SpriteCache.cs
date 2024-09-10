using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public sealed class SpriteCache
	{
		private float lastFreeTime = -1f;
		private float defaultReleaseAfterFree = 30;
		private Sprite cacheObject;
		private int referenceCount;
		private AssetID assetId;
		private IDictionary<Sprite, SpriteCache> lookup;
		private float releaseAfterFree;
		private bool isSync;

		internal SpriteCache(AssetID assetId, IDictionary<Sprite, SpriteCache> lookup)
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
				releaseAfterFree = defaultReleaseAfterFree;
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
				Debug.LogErrorFormat("[SpriteCache] referenceCount is error {0} {1}", assetId.ToString(),
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

		internal Sprite GetObject()
		{
			return cacheObject;
		}

		private IEnumerator LoadObjectImpl(AssetID assetId)
		{
			WaitLoadObject waitObj = null;
			if (isSync) waitObj = AssetManager.LoadObjectSync(this.assetId, typeof(Sprite));
			else waitObj = AssetManager.LoadObject(assetId, typeof(Sprite));
			yield return waitObj;
			if (string.IsNullOrEmpty(waitObj.Error))
			{
				cacheObject = waitObj.GetObject() as Sprite;
				if (cacheObject == null)
					Error = $"This asset: {this.assetId} is not a Sprite!";
				else
				{
					releaseAfterFree = DefaultReleaseAfterFree;
					if (lookup.ContainsKey(cacheObject)) lookup[cacheObject] = this;
					else lookup.Add(cacheObject, this);
				}
			}
			else Error = waitObj.Error;
		}
	}
}