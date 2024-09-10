using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace Game
{
	internal sealed class AssetBundleLoadTask
	{
		private static int maxWriteCountInTime = 5;
		private static int writeCountInTime = 0;

		private const float SampleSpeedInitInterval = 0.05f;
		private const float SampleSpeedUpdateInterval = 0.5f;

		private int maxReDownloadCount = 6;
		private int tryReDownloadCount = 0;
		private AssetBundleDownloader downloader;
		private string url;
		private UnityWebRequest www;
		private bool sendRequest;
		private string cachePath;
		private AssetBundleCache cache;
		private AssetBundleCreateRequest request;
		private string bundleName;
		private Hash128 hash;
		private ulong bytesDownloaded;
		private float progress;
		private ulong sampleSpeed;
		private float sampleTimeLeft;
		private float sampleAccumTime;
		private ulong sampleDownloadBytes;

		internal bool UpdateOnly { get; set; }
		internal AssetBundle AssetBundle { get; private set; }
		internal string Error { get; private set; }
		internal string BundleName => bundleName;
		internal Hash128 Hash => hash;
		internal string Url => url;
		internal int BytesDownloaded => (int) bytesDownloaded;
		internal int ContentLength => 0;
		internal float Progress => progress;
		internal int DownloadSpeed => (int) sampleSpeed;
		internal bool Updating => www != null;

		internal AssetBundleLoadTask(
			bool updateOnly,
			AssetBundleDownloader downloader,
			string url,
			AssetBundleCache cache,
			string bundleName,
			Hash128 hash)
		{
			UpdateOnly = updateOnly;
			this.downloader = downloader;
			this.url = url;
			this.cache = cache;
			this.bundleName = bundleName;
			this.hash = hash;
			ResetSampleSpeed(0.05f);
		}

		internal AssetBundleLoadTask(
			bool updateOnly,
			AssetBundleDownloader downloader,
			string url,
			AssetBundleCache cache,
			string bundleName)
		{
			UpdateOnly = updateOnly;
			this.downloader = downloader;
			this.url = url;
			this.cache = cache;
			this.bundleName = bundleName;
			ResetSampleSpeed(0.05f);
		}

		internal AssetBundleLoadTask(
			AssetBundleCache cache,
			AssetBundleCreateRequest request,
			string bundleName,
			Hash128 hash)
		{
			UpdateOnly = false;
			this.cache = cache;
			this.request = request;
			this.bundleName = bundleName;
			this.hash = hash;
		}

		internal AssetBundleLoadTask(
			AssetBundleCache cache,
			AssetBundleCreateRequest request,
			string bundleName)
		{
			UpdateOnly = false;
			this.cache = cache;
			this.request = request;
			this.bundleName = bundleName;
		}

		public static void SetMaxWriteCountLimit(int value)
		{
			maxWriteCountInTime = value;
		}

		public static void ClearWriteCountLimit()
		{
			writeCountInTime = 0;
		}

		internal void StartDownload()
		{
			Assert.IsNull(www);
			cachePath = hash.isValid ? cache.GetCachePath(bundleName, hash) : cache.GetCachePath(bundleName);
			www = UnityWebRequest.Get(url);
			www.Send();
			sendRequest = true;
		}

		internal bool Update()
		{
			if (www != null)
				UpdateDownload();
			else
				UpdateRequest();
			return string.IsNullOrEmpty(Error) && (!sendRequest || (!sendRequest || Updating)) && AssetBundle == null;
		}

		private void UpdateDownload()
		{
			var deltaBytes = www.downloadedBytes - bytesDownloaded;
			bytesDownloaded = www.downloadedBytes;
			progress = www.downloadProgress;
			UpdateSampleSpeed(deltaBytes);
			if (www.result == UnityWebRequest.Result.Success)
			{
				if (!www.isDone) return;
				if (writeCountInTime < maxWriteCountInTime)
				{
					writeCountInTime++;
					try
					{
						var dirName = Path.GetDirectoryName(cachePath);
						if (!Directory.Exists(dirName))
							Directory.CreateDirectory(dirName);
						File.WriteAllBytes(cachePath, www.downloadHandler.data);
					}
					catch (Exception e)
					{
						Error = e.Message;
						www.Dispose();
						www = null;
						downloader.Complete(this, ++tryReDownloadCount <= maxReDownloadCount);
						return;
					}
					www.Dispose();
					www = null;
					UpdateComplete();
				}
				downloader.Complete(this, false);
			}
			else
			{
				Error = $"http load error: {url} {www.result} {www.error}";
				www.Dispose();
				www = null;
				downloader.Complete(this, ++tryReDownloadCount <= maxReDownloadCount);
			}
		}

		private void UpdateComplete()
		{
			if (UpdateOnly) return;
			if (cache != null)
			{
				if (hash.isValid)
				{
					Assert.IsNotNull(cache);
					request = cache.LoadFromCache(bundleName, hash, false);
					if (request == null)
						Error = $"The asset at {www.url} with has {hash} is not in the cache.";
				}
				else
				{
					Assert.IsNotNull(cache);
					request = cache.LoadFromCache(bundleName);
					if (request == null)
						Error = $"The asset at {www.url} is not in the cache.";
				}
			}
			else
			{
				var content = DownloadHandlerAssetBundle.GetContent(www);
				if (content != null)
					SetAssetBundle(content);
				else
					Error = $"The asset at {www.url} is not an asset bundle.";
			}
		}

		private void UpdateRequest()
		{
			if (request == null || !request.isDone) return;
			if (request.assetBundle != null)
				SetAssetBundle(request.assetBundle);
			else if (cache != null)
			{
				if (hash.isValid)
				{
					cache.DeleteCache(bundleName, hash);
					Error = $"The asset at {bundleName} with hash {hash} in cache is not an asset bundle.";
				}
				else
				{
					cache.DeleteCache(bundleName);
					Error = $"The asset at {bundleName} in cache is not an asset bundle.";
				}
			}
			else Error = $"The asset at {bundleName} with hash {hash} is not an asset bundle.";
		}

		private void SetAssetBundle(AssetBundle assetBundle)
		{
			AssetBundle = assetBundle;
			if (assetBundle.isStreamedSceneAssetBundle) return;
			foreach (var material in assetBundle.LoadAllAssets<Material>())
			{
				var shader = Shader.Find(material.shader.name);
				if (shader != null)
					material.shader = shader;
			}
		}

		private void ResetSampleSpeed(float interval)
		{
			sampleTimeLeft = interval;
			sampleAccumTime = 0;
			sampleDownloadBytes = 0UL;
		}

		private void UpdateSampleSpeed(ulong deltaBytes)
		{
			sampleTimeLeft -= Time.unscaledDeltaTime;
			sampleAccumTime += Time.unscaledDeltaTime;
			sampleDownloadBytes += deltaBytes;
			if (sampleTimeLeft > 0) return;
			sampleSpeed = (ulong) (sampleDownloadBytes / sampleAccumTime);
			ResetSampleSpeed(sampleSpeed == 0ul ? 0.05f : 0.5f);
		}
	}
}