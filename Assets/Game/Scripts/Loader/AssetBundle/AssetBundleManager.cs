using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
	internal sealed class AssetBundleManager
	{
		private AssetBundleLoader loader;
		private AssetBundleManifest manifest;
		private float lastCheckSweepTime = 0;
		private int unloadedCountInTime = 0;
		string[] activeVariants = new string[0];
		LinkedList<AssetBundleItem> loadingAssetBundles = new LinkedList<AssetBundleItem>();

		Dictionary<string, AssetBundleItem> assetBundles =
			new Dictionary<string, AssetBundleItem>(StringComparer.Ordinal);

		Queue<AssetBundleItem> waitLoadQueue = new Queue<AssetBundleItem>();
		private int maxLoadingCount = 5;
		private Func<string, AssetBundleItem, bool> sweepChecker;

		internal int MaxLoadingCount
		{
			set => maxLoadingCount = value;
		}

		public string[] ActiveVariants
		{
			get => activeVariants;
			set => activeVariants = value;
		}

		public AssetBundleManifest Manifest
		{
			get => manifest;
			set => manifest = value;
		}

		internal AssetBundleManager(AssetBundleLoader loader)
		{
			this.loader = loader;
			sweepChecker = (bundle, item) =>
			{
				if (unloadedCountInTime > 1) return false;
				var flag = item.Sweep();
				if (flag) unloadedCountInTime++;
				return flag;
			};
		}

		internal AssetBundleItem GetAssetBundle(string bundleName)
		{
			if (!assetBundles.TryGetValue(bundleName, out var item1)) return null;
			if (!string.IsNullOrEmpty(item1.Error)) return item1;
			if (item1.AssetBundle == null) return null;
			var dependencies = item1.Dependencies;
			if (dependencies != null)
			{
				foreach (var item2 in dependencies)
				{
					if (item2 != null)
					{
						if (!string.IsNullOrEmpty(item2.Error))
						{
							item1.Error = item2.Error;
							return item1;
						}
						if (item2.AssetBundle == null) return null;
					}
				}
			}
			return item1;
		}

		internal bool LoadLocalManifest(string bundleName)
		{
			var wait = loader.LoadLocal(bundleName, true);
			if (wait == null) return false;
			var item = new AssetBundleItem(wait);
			item.Retain();
			assetBundles.Add(bundleName, item);
			loadingAssetBundles.AddLast(item);
			return true;
		}

		internal bool LoadRemoteManifest(string bundleName)
		{
			var wait = loader.LoadRemote(bundleName);
			if (wait == null) return false;
			var item = new AssetBundleItem(wait);
			item.Retain();
			assetBundles.Add(bundleName, item);
			loadingAssetBundles.AddLast(item);
			return true;
		}

		internal AssetBundle LoadAssetBundleSync(string bundleName)
		{
			return LoadAssetBundleSyncInternal(bundleName)?.AssetBundle;
		}

		internal bool LoadAssetBundle(string bundleName, bool isSync)
		{
			return LoadAssetBundleInternal(bundleName, isSync) != null;
		}

		internal bool UnloadAssetBundle(string bundleName)
		{
			if (!assetBundles.TryGetValue(bundleName, out var item)) return false;
			if (item.AssetBundle != null) item.Release();
			return true;
		}

		public void UnloadAllUnusedAssetBundle()
		{
			assetBundles.RemoveAll((bundle, item) =>
			{
				if (item.RefCount > 0) return false;
				item.Destroy(true);
				return true;
			});
		}

		internal void DestroyAssetBundle(string bundleName, bool unloadAllLoadedObjects = false)
		{
			if (!assetBundles.TryGetValue(bundleName, out var item)) return;
			item.Destroy(unloadAllLoadedObjects);
			assetBundles.Remove(bundleName);
		}

		internal bool HasLoadingBundles()
		{
			return loadingAssetBundles.Any(loadingAssetBundle => !loadingAssetBundle.CheckLoading());
		}

		internal void UnloadAll()
		{
			foreach (var kv in assetBundles)
			{
				var item = kv.Value;
				if (item.AssetBundle != null) item.AssetBundle.Unload(false);
			}
			loadingAssetBundles.Clear();
			waitLoadQueue.Clear();
			assetBundles.Clear();
		}

		internal string[] GetBundlesWithoutCached(string bundleName)
		{
			Assert.IsNotNull(manifest);
			var strSet = new HashSet<string>(StringComparer.Ordinal);
			GetBundlesWithoutCachedImpl(bundleName, strSet);
			return strSet.ToArray();
		}

		internal void Update()
		{
			LinkedListNode<AssetBundleItem> next;
			for (var node = loadingAssetBundles.First; node != null; node = next)
			{
				next = node.Next;
				if (node.Value.CheckLoading()) loadingAssetBundles.Remove(node);
			}
			if (Time.time - lastCheckSweepTime >= 0.2f)
			{
				lastCheckSweepTime = Time.time;
				unloadedCountInTime = 0;
				assetBundles.RemoveAll(sweepChecker);
			}
			QueueLoad();
		}

		internal void DrawAssetBundles() { }

		private void GetBundlesWithoutCachedImpl(string bundleName, HashSet<string> bundles)
		{
			if (!bundles.Contains(bundleName))
			{
				var hash = manifest.GetAssetBundleHash(bundleName);
				if (!loader.IsLocalCached(bundleName, hash))
					bundles.Add(bundleName);
			}
			foreach (var allDep in manifest.GetAllDependencies(bundleName))
			{
				if (bundles.Contains(allDep)) continue;
				var hash = manifest.GetAssetBundleHash(allDep);
				if (!loader.IsLocalCached(allDep, hash))
					bundles.Add(allDep);
			}
		}

		private AssetBundleItem LoadAssetBundleSyncInternal(string bundleName)
		{
			if (assetBundles.TryGetValue(bundleName, out var item1))
			{
				item1.Retain();
				return item1;
			}
			Assert.IsNotNull(manifest);
			var hash = manifest.GetAssetBundleHash(bundleName);
			if (!hash.isValid) return null;
			var ab = loader.LoadLocalSync(bundleName, hash);
			if (ab == null) return null;
			var item2 = new AssetBundleItem(ab, bundleName);
			assetBundles.Add(bundleName, item2);
			var allDep = manifest.GetAllDependencies(bundleName);
			var items = new AssetBundleItem[allDep.Length];
			for (int index = 0; index < allDep.Length; index++)
				items[index] = LoadAssetBundleSyncInternal(allDep[index]);
			item2.Dependencies = items;
			item2.Retain();
			return item2;
		}

		private AssetBundleItem LoadAssetBundleInternal(string bundleName, bool isSync,
			bool isFromDependLoad = false)
		{
			if (assetBundles.TryGetValue(bundleName, out var item1))
			{
				item1.Retain();
				return item1;
			}
			Assert.IsNotNull(manifest);
			var hash = manifest.GetAssetBundleHash(bundleName);
			if (!hash.isValid) return null;
			AssetBundleItem item2;
			if (isSync)
			{
				var wait = loader.Load(bundleName, hash, isSync);
				if (wait == null || !string.IsNullOrEmpty(wait.Error))
					return null;
				item2 = new AssetBundleItem(wait, bundleName);
				assetBundles.Add(bundleName, item2);
				loadingAssetBundles.AddLast(item2);
			}
			else
			{
				item2 = new AssetBundleItem(bundleName);
				assetBundles.Add(bundleName, item2);
				waitLoadQueue.Enqueue(item2);
			}
			var allDep = manifest.GetAllDependencies(bundleName);
			var items = new AssetBundleItem[allDep.Length];
			for (int index = 0; index < allDep.Length; index++)
				items[index] = LoadAssetBundleInternal(allDep[index], isSync, true);
			item2.Dependencies = items;
			item2.Retain();
			return item2;
		}

		private void QueueLoad()
		{
			if (waitLoadQueue.Count <= 0) return;
			var num = maxLoadingCount + waitLoadQueue.Count / 10 - loadingAssetBundles.Count;
			if (num <= 0) return;
			for (int index = 0; index < num && waitLoadQueue.Count > 0; index++)
			{
				var item = waitLoadQueue.Dequeue();
				var bundleName = item.BundleName;
				var hash = manifest.GetAssetBundleHash(bundleName);
				var wait = loader.Load(bundleName, hash, false);
				if (wait != null && string.IsNullOrEmpty(wait.Error))
				{
					item.SetWaitLoadAssetBundle(wait);
					loadingAssetBundles.AddLast(item);
				}
			}
		}
	}
}