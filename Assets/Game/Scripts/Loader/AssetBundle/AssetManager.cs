using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Game
{
	public static class AssetManager
	{
		public delegate void UpdateDelegate(float progress, int downloadSpeed, int bytesDownloaded, int contentLength);

		private static AssetBundleCache bundleCache = new AssetBundleCache();
		private static AssetBundleDownloader bundleDownloader = new AssetBundleDownloader();
		private static AssetBundleLoader bundleLoader = new AssetBundleLoader(bundleCache, bundleDownloader);
		private static AssetBundleManager bundleManager = new AssetBundleManager(bundleLoader);
		private static AssetBundleFileInfo fileInfo = new AssetBundleFileInfo();
		private static List<WaitLoadAsset> progressWaitings = new List<WaitLoadAsset>();
		private static LinkedListNode<Action> updateHandle;
		private static bool _isUseObb;

		static AssetManager()
		{
			Simulator = new AssetSimulator();
		}

		public static void SetIsUseObb(bool isUseObb)
		{
			_isUseObb = isUseObb;
		}

		public static bool GetIsUseObb()
		{
			return _isUseObb;
		}

		public static event Action<string> DownloadStartEvent
		{
			add => bundleDownloader.DownloadStartEvent += value;
			remove => bundleDownloader.DownloadStartEvent -= value;
		}

		public static event Action<string> DownloadFinishEvent
		{
			add => bundleDownloader.DownloadFinishEvent += value;
			remove => bundleDownloader.DownloadFinishEvent -= value;
		}

		public static AssetSimulator Simulator { get; private set; }

		public static string CachePath => bundleCache.CachePath;

		public static string DownloadingUrl
		{
			get => bundleLoader.DownloadUrl;
			set => bundleLoader.DownloadUrl = value;
		}

		public static string AssetVersion
		{
			get => bundleLoader.AssetVersion;
			set => bundleLoader.AssetVersion = value;
		}

		public static bool IgnoreHashCheck
		{
			get => bundleLoader.IgnoreHashCheck;
			set => bundleLoader.IgnoreHashCheck = value;
		}

		public static string[] ActiveVariants
		{
			get => bundleManager.ActiveVariants;
			set => bundleManager.ActiveVariants = value;
		}

		public static AssetBundleManifest Manifest => bundleManager.Manifest;

		public static bool HasManifest => Simulator.SimulateAssetBundle || Manifest != null;

		public static void DrawAssetBundle()
		{
			bundleManager.DrawAssetBundles();
		}

		public static void ClearCache()
		{
			bundleCache.ClearCache();
		}

		public static IEnumerator Dispose()
		{
			bundleLoader.Dispose();
			bundleManager.Manifest = null;
			progressWaitings.Clear();
			return new WaitUntil(() =>
			{
				if (bundleManager.HasLoadingBundles()) return false;
				if (updateHandle != null)
				{
					Scheduler.RemoveFrameListener(updateHandle);
					updateHandle = null;
				}
				bundleManager.UnloadAll();
				return true;
			});
		}

		public static string LoadVersion()
		{
			return bundleCache.LoadVersion();
		}

		public static void SaveVersion(string version)
		{
			bundleCache.SaveVersion(version);
		}

		public static WaitLoadAsset LoadLocalManifest(string manifestAssetBundleName)
		{
			updateHandle = Scheduler.AddFrameListener(Update);
			if (Simulator.SimulateAssetBundle) return Simulator.LoadDelay();
			if (!bundleManager.LoadLocalManifest(manifestAssetBundleName)) return null;
			var wait = new WaitLoadManifest(bundleManager, manifestAssetBundleName, "AssetBundleManifest",
				typeof(AssetBundleManifest));
			progressWaitings.Add(wait);
			return wait;
		}

		public static WaitLoadAsset LoadRemoteManifest(string manifestAssetBundleName)
		{
			if (Simulator.SimulateAssetBundle) return Simulator.LoadDelay();
			if (!bundleManager.LoadRemoteManifest(manifestAssetBundleName)) return null;
			var wait = new WaitLoadManifest(bundleManager, manifestAssetBundleName, "AssetBundleManifest",
				typeof(AssetBundleManifest));
			progressWaitings.Add(wait);
			return wait;
		}

		public static bool ExistedInSStreaming(string filePath)
		{
			return bundleCache.ExistedInStreaming(filePath);
		}

		public static bool IsVersionCached(string bundleName)
		{
			var hash = Manifest.GetAssetBundleHash(bundleName);
			return bundleCache.IsVersionCached(bundleName, hash);
		}

		public static bool IsVersionCached(string bundleName, Hash128 hash)
		{
			return bundleCache.IsVersionCached(bundleName, hash);
		}

		public static WaitLoadFileInfo LoadFileInfo()
		{
			var url = DownloadingUrl;
			Assert.IsFalse(string.IsNullOrEmpty(url));
			return fileInfo.Load(url + "/file_info.txt");
		}

		public static WaitUpdateAssetBundle UpdateBundle(string bundleName)
		{
			var hash = Manifest.GetAssetBundleHash(bundleName);
			return bundleLoader.UpdateBundle(bundleName, hash);
		}

		public static WaitUpdateAssetBundle UpdateBundle(string bundleName, Hash128 hash)
		{
			return bundleLoader.UpdateBundle(bundleName, hash);
		}

		public static AssetBundle LoadBundleLocal(string bundleName)
		{
			return bundleManager.LoadAssetBundleSync(bundleName);
		}

		public static UnityEngine.Object LoadObjectLocal(AssetID assetId, Type assetType)
		{
			return LoadObjectLocal(assetId.BundleName, assetId.AssetName, assetType);
		}

		public static UnityEngine.Object LoadObjectLocal(string bundleName, string assetName, Type assetType)
		{
			if (Simulator.SimulateAssetBundle) return Simulator.LoadObjectLocal(bundleName, assetName, assetType);
			var ab = bundleManager.LoadAssetBundleSync(bundleName);
			return ab == null ? null : ab.LoadAsset(assetName, assetType);
		}

		public static WaitLoadObject LoadObject(AssetID assetId, Type assetType)
		{
			return LoadObject(assetId.BundleName, assetId.AssetName, assetType);
		}

		public static WaitLoadObject LoadObject(string bundleName, string assetName, Type assetType)
		{
			if (Simulator.SimulateAssetBundle) return Simulator.LoadObject(bundleName, assetName, assetType);
			if (!bundleManager.LoadAssetBundle(bundleName, false))
				return new WaitLoadObjectFull("Load asset bundle {0}:{1} failed!", bundleName, assetName);
			var wait = new WaitLoadObjectFull(bundleManager, bundleName, assetName, assetType);
			progressWaitings.Add(wait);
			return wait;
		}

		public static void LoadObject(AssetID assetId, Type assetType, Action<UnityEngine.Object> complete)
		{
			LoadObject(assetId.BundleName, assetId.AssetName, assetType, complete);
		}

		public static void LoadObject(string bundleName, string assetName, Type assetType,
			Action<UnityEngine.Object> complete)
		{
			if (assetType == typeof(GameObject))
				Debug.LogError("Please do not use the method to load GameObject!");
			else
				Scheduler.RunCoroutine(LoadObjectImpl(bundleName, assetName, assetType, complete));
		}

		public static WaitLoadObject LoadObjectSync(AssetID assetId, Type assetType)
		{
			return LoadObjectSync(assetId.BundleName, assetId.AssetName, assetType);
		}

		public static WaitLoadObject LoadObjectSync(string bundleName, string assetName, Type assetType)
		{
			if (Simulator.SimulateAssetBundle) return Simulator.LoadObjectSync(bundleName, assetName, assetType);
			if (!bundleManager.LoadAssetBundle(bundleName, true))
				return new WaitLoadObjectFull("Load asset bundle {0}:{1} failed!", bundleName, assetName);
			var wait = new WaitLoadObjectFullSync(bundleManager, bundleName, assetName, assetType);
			progressWaitings.Add(wait);
			return wait;
		}

		public static void LoadObjectSync(AssetID assetId, Type assetType, Action<UnityEngine.Object> complete)
		{
			LoadObjectSync(assetId.BundleName, assetId.AssetName, assetType, complete);
		}

		public static void LoadObjectSync(string bundleName, string assetName, Type assetType,
			Action<UnityEngine.Object> complete)
		{
			if (assetType == typeof(GameObject))
				Debug.LogError("Please do not use the method to load GameObject!");
			else
				Scheduler.RunCoroutine(LoadObjectSyncImpl(bundleName, assetName, assetType, complete));
		}

		public static WaitLoadScene LoadScene(AssetID assetId, LoadSceneMode loadMode)
		{
			return LoadScene(assetId.BundleName, assetId.AssetName, loadMode);
		}

		public static WaitLoadScene LoadScene(string bundleName, string sceneName, LoadSceneMode loadMode)
		{
			if (Simulator.SimulateAssetBundle) return Simulator.LoadScene(bundleName, sceneName, loadMode);
			if (!bundleManager.LoadAssetBundle(bundleName, false))
				return new WaitLoadSceneFull("Load scene {0}:{1} failed!", bundleName, sceneName);
			var wait = new WaitLoadSceneFull(bundleManager, bundleName, sceneName, loadMode);
			progressWaitings.Add(wait);
			return wait;
		}

		public static void LoadScene(AssetID assetId, LoadSceneMode loadMode, Action complete)
		{
			LoadScene(assetId.BundleName, assetId.AssetName, loadMode, complete);
		}

		public static void LoadScene(string bundleName, string sceneName, LoadSceneMode loadMode, Action complete,
			Action<float> progress = null)
		{
			Scheduler.RunCoroutine(LoadSceneImpl(bundleName, sceneName, loadMode, complete, progress));
		}

		public static WaitLoadScene LoadSceneSync(AssetID assetId, LoadSceneMode loadMode)
		{
			return LoadSceneSync(assetId.BundleName, assetId.AssetName, loadMode);
		}

		public static WaitLoadScene LoadSceneSync(string bundleName, string sceneName, LoadSceneMode loadMode)
		{
			if (Simulator.SimulateAssetBundle) return Simulator.LoadSceneSync(bundleName, sceneName, loadMode);
			if (!bundleManager.LoadAssetBundle(bundleName, true))
				return new WaitLoadSceneFull("Load scene {0}:{1} failed!", bundleName, sceneName);
			var wait = new WaitLoadSceneFullSync(bundleManager, bundleName, sceneName, loadMode);
			progressWaitings.Add(wait);
			return wait;
		}

		public static void LoadSceneSync(AssetID assetId, LoadSceneMode loadMode, Action complete)
		{
			LoadSceneSync(assetId.BundleName, assetId.AssetName, loadMode, complete);
		}

		public static void LoadSceneSync(string bundleName, string sceneName, LoadSceneMode loadMode, Action complete)
		{
			Scheduler.RunCoroutine(LoadSceneSyncImpl(bundleName, sceneName, loadMode, complete));
		}

		public static string[] GetAssetNamesInBundle(string bundleName, bool isFullName = false)
		{
			if (Simulator.SimulateAssetBundle) return new string[0];
			var ab = LoadBundleLocal(bundleName);
			if (ab == null) return new string[0];
			var assetNames = ab.GetAllAssetNames();
			if (!isFullName)
			{
				for (int i = 0; i < assetNames.Length; i++)
				{
					var index = assetNames[i].LastIndexOf("/");
					if (index >= 0)
						assetNames[i] = assetNames[i].Substring(index + 1);
				}
			}
			return assetNames;
		}

		public static string[] GetDependBundles(string bundleName)
		{
			return Simulator.SimulateAssetBundle ? new string[0] : Manifest.GetAllDependencies(bundleName);
		}

		public static string[] GetBundlesWithoutCache(string bundleName)
		{
			return Simulator.SimulateAssetBundle ? new string[0] : bundleManager.GetBundlesWithoutCached(bundleName);
		}

		public static bool UnloadAssetBundle(string bundleName)
		{
			return bundleManager.UnloadAssetBundle(bundleName);
		}

		public static void UnloadAllUnusedAssetBundle()
		{
			bundleManager.UnloadAllUnusedAssetBundle();
		}

		public static void Update()
		{
			bundleLoader.Update();
			bundleManager.Update();
			progressWaitings.RemoveAll(waiting => !waiting.Update());
		}

		public static void SetAssetBundleLoadMaxCount(int loadCount, int writeCount)
		{
			bundleManager.MaxLoadingCount = loadCount;
			AssetBundleLoadTask.SetMaxWriteCountLimit(writeCount);
		}

		public static void LoadRemoteManifest(string manifestAssetBundleName, Action<string> complete)
		{
			Scheduler.RunCoroutine(LoadRemoteManifestImpl(manifestAssetBundleName, complete));
		}

		public static void LoadFileInfo(Action<string, AssetBundleFileInfo> complete)
		{
			Scheduler.RunCoroutine(LoadFileInfoImpl(complete));
		}

		public static void UpdateBundle(string bundleName, UpdateDelegate update, Action<string> complete)
		{
			Scheduler.RunCoroutine(UpdateBundleImpl(bundleName, update, complete));
		}

		public static void UpdateBundle(string bundleName, Hash128 hash, UpdateDelegate update, Action<string> complete)
		{
			Scheduler.RunCoroutine(UpdateBundleImpl(bundleName, hash, update, complete));
		}


		private static IEnumerator LoadFileInfoImpl(Action<string, AssetBundleFileInfo> complete)
		{
			var wait = LoadFileInfo();
			yield return wait;
			complete(wait.Error, wait.FileInfo);
		}

		private static IEnumerator LoadRemoteManifestImpl(string manifestAssetBundleName, Action<string> complete)
		{
			var wait = LoadRemoteManifest(manifestAssetBundleName);
			yield return wait;
			complete(wait.Error);
		}

		private static IEnumerator UpdateBundleImpl(string bundleName, UpdateDelegate update, Action<string> complete)
		{
			var wait = UpdateBundle(bundleName);
			if (string.IsNullOrEmpty(wait.Error))
			{
				var handle = Scheduler.AddFrameListener(() =>
					update(wait.Progress, wait.DownloadSpeed, wait.BytesDownloaded, wait.ContentLength));
				yield return wait;
				Scheduler.RemoveFrameListener(handle);
				complete(wait.Error);
			}
			else complete(wait.Error);
		}

		private static IEnumerator UpdateBundleImpl(string bundleName, Hash128 hash, UpdateDelegate update,
			Action<string> complete)
		{
			var wait = UpdateBundle(bundleName, hash);
			if (string.IsNullOrEmpty(wait.Error))
			{
				var handle = Scheduler.AddFrameListener(() =>
					update(wait.Progress, wait.DownloadSpeed, wait.BytesDownloaded, wait.ContentLength));
				yield return wait;
				Scheduler.RemoveFrameListener(handle);
				complete(wait.Error);
			}
			else complete(wait.Error);
		}

		private static IEnumerator LoadObjectImpl(string bundleName, string assetName, Type assetType,
			Action<UnityEngine.Object> complete)
		{
			var wait = LoadObject(bundleName, assetName, assetType);
			yield return wait;
			complete(wait.GetObject());
		}

		private static IEnumerator LoadObjectSyncImpl(string bundleName, string assetName, Type assetType,
			Action<UnityEngine.Object> complete)
		{
			var wait = LoadObjectSync(bundleName, assetName, assetType);
			yield return wait;
			complete(wait.GetObject());
		}

		private static IEnumerator LoadSceneImpl(string bundleName, string sceneName, LoadSceneMode loadMode,
			Action complete, Action<float> progress)
		{
			var wait = LoadScene(bundleName, sceneName, loadMode);
			if (progress != null)
			{
				var handle = Scheduler.AddFrameListener(() => progress(wait.Progress));
				yield return wait;
				Scheduler.RemoveFrameListener(handle);
			}
			else yield return wait;
			complete();
		}

		private static IEnumerator LoadSceneSyncImpl(string bundleName, string sceneName, LoadSceneMode loadMode,
			Action complete)
		{
			var wait = LoadSceneSync(bundleName, sceneName, loadMode);
			yield return wait;
			complete();
		}
	}
}