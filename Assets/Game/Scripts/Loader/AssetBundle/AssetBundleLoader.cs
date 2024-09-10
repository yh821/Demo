using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
	internal sealed class AssetBundleLoader
	{
		private LinkedList<AssetBundleLoadTask> tasks = new LinkedList<AssetBundleLoadTask>();
		private AssetBundleCache cache;
		private AssetBundleDownloader downloader;

		internal string DownloadUrl { get; set; }
		internal string AssetVersion { get; set; }
		internal bool IgnoreHashCheck { get; set; }

		internal AssetBundleLoader(AssetBundleCache cache, AssetBundleDownloader downloader)
		{
			this.cache = cache;
			this.downloader = downloader;
		}

		internal WaitUpdateAssetBundle UpdateBundle(string bundleName, Hash128 hash)
		{
			if (!hash.isValid) return new WaitUpdateAssetBundle("Bundle hash is invalid.");
			if (cache.IsVersionCached(bundleName, hash)) return new WaitUpdateAssetBundle();
			foreach (var task in tasks.Where(task => task.BundleName == bundleName && task.Hash == hash))
				return new WaitUpdateAssetBundle(task);
			var simulateFailed = AssetManager.Simulator.SimulateAssetBundleFailedRequest();
			if (simulateFailed != null)
				return new WaitUpdateAssetBundle(simulateFailed);
			var task1 = new AssetBundleLoadTask(true, downloader,
				GetRemotePath(bundleName, hash.ToString()), cache, bundleName, hash);
			tasks.AddLast(task1);
			downloader.Start(task1);
			return new WaitUpdateAssetBundle(task1);
		}

		internal AssetBundle LoadLocalSync(string bundleName)
		{
			return cache.LoadFromCacheSync(bundleName);
		}

		internal AssetBundle LoadLocalSync(string bundleName, Hash128 hash)
		{
			return cache.LoadFromCacheSync(bundleName, hash, IgnoreHashCheck);
		}

		internal WaitLoadAssetBundle LoadLocal(string bundleName, bool isSync)
		{
			if (isSync) return new WaitLoadAssetBundle(cache.LoadFromCacheSync(bundleName));
			var request = cache.LoadFromCache(bundleName);
			if (request == null) return null;
			var task = new AssetBundleLoadTask(cache, request, bundleName);
			tasks.AddLast(task);
			return new WaitLoadAssetBundle(task);
		}

		internal WaitLoadAssetBundle LoadLocal(string bundleName, Hash128 hash, bool isSync)
		{
			if (isSync) return new WaitLoadAssetBundle(cache.LoadFromCacheSync(bundleName, hash, IgnoreHashCheck));
			var request = cache.LoadFromCache(bundleName, hash, IgnoreHashCheck);
			if (request == null) return null;
			var task = new AssetBundleLoadTask(cache, request, bundleName, hash);
			tasks.AddLast(task);
			return new WaitLoadAssetBundle(task);
		}

		internal WaitLoadAssetBundle Load(string bundleName, Hash128 hash, bool isSync)
		{
			if (IgnoreHashCheck || cache.IsVersionCached(bundleName, hash))
				return LoadLocal(bundleName, hash, isSync);
			foreach (var task in tasks)
			{
				if (task.BundleName == bundleName && task.Hash == hash)
				{
					task.UpdateOnly = false;
					return new WaitLoadAssetBundle(task);
				}
			}
			var simulateFailed = AssetManager.Simulator.SimulateAssetBundleFailedRequest();
			if (simulateFailed != null)
				return new WaitLoadAssetBundle(simulateFailed);
			var task1 = new AssetBundleLoadTask(false, downloader,
				GetRemotePath(bundleName, hash.ToString()), cache, bundleName, hash);
			tasks.AddLast(task1);
			downloader.Start(task1);
			return new WaitLoadAssetBundle(task1);
		}

		internal WaitLoadAssetBundle LoadRemote(string bundleName)
		{
			foreach (var task in tasks)
			{
				if (string.Equals(task.BundleName, bundleName))
				{
					task.UpdateOnly = false;
					return new WaitLoadAssetBundle(task);
				}
			}
			var task1 = new AssetBundleLoadTask(false, downloader,
				GetRemotePath(bundleName, AssetVersion), cache, bundleName);
			tasks.AddLast(task1);
			downloader.Start(task1);
			return new WaitLoadAssetBundle(task1);
		}

		internal bool IsLocalCached(string bundleName, Hash128 hash)
		{
			return cache.IsVersionCached(bundleName, hash);
		}

		internal void Dispose()
		{
			foreach (var task in tasks)
			{
				if (task.Updating)
					task.UpdateOnly = true;
			}
		}

		internal void Update()
		{
			LinkedListNode<AssetBundleLoadTask> next;
			for (var node = tasks.First; node != null; node = next)
			{
				next = node.Next;
				if (!node.Value.Update())
					tasks.Remove(node);
			}
			AssetBundleLoadTask.ClearWriteCountLimit();
		}

		private string GetRemotePath(string bundleName, string variant)
		{
			return string.IsNullOrEmpty(DownloadUrl) ? string.Empty : $"{DownloadUrl}/{bundleName}?v={variant}";
		}
	}
}