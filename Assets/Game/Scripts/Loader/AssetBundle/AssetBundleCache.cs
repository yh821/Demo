using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Game
{
	internal sealed class AssetBundleCache
	{
		private HashSet<string> streamingFiles = new HashSet<string>(StringComparer.Ordinal);
		private string cachePath;

		internal AssetBundleCache()
		{
			var str1 = StreamingAssets.ReadAllText("file_list.txt");
			var array = new char[1] {'\n'};
			foreach (var str2 in str1.Split(array))
				streamingFiles.Add(str2);
		}

		internal string CachePath
		{
			get
			{
				if (string.IsNullOrEmpty(cachePath))
				{
					cachePath = Path.Combine(Application.persistentDataPath, "AssetCache");
					if (!Directory.Exists(cachePath))
						Directory.CreateDirectory(cachePath);
				}
				return cachePath;
			}
		}

		public string LoadVersion()
		{
			var path = GetCachePath("version.txt");
			return File.Exists(path)
				? File.ReadAllText(path)
				: StreamingAssets.ReadAllText("AssetBundle/version.txt");
		}

		public void SaveVersion(string version)
		{
			File.WriteAllText(Path.Combine(CachePath, "version.txt"), version);
		}

		internal bool IsVersionCached(string bundleName, Hash128 hash)
		{
			return !string.IsNullOrEmpty(GetLocalPath(bundleName, hash));
		}

		internal void ClearCache()
		{
			if (!Directory.Exists(CachePath)) return;
			Directory.Delete(CachePath, true);
		}

		internal void DeleteCache(string bundleName)
		{
			var path = GetCachePath(bundleName);
			if (!File.Exists(path)) return;
			File.Delete(path);
		}

		internal void DeleteCache(string bundleName, Hash128 hash)
		{
			var path = GetCachePath(bundleName, hash);
			if (!File.Exists(path)) return;
			File.Delete(path);
		}

		internal AssetBundle LoadFromCacheSync(string bundleName)
		{
			var localPath = GetLocalPath(bundleName);
			return string.IsNullOrEmpty(localPath) ? null : AssetBundle.LoadFromFile(localPath);
		}

		internal AssetBundle LoadFromCacheSync(string bundleName, Hash128 hash, bool ignoreHashCheck)
		{
			var path = ignoreHashCheck ? GetLocalPathIgnoreHash(bundleName, hash) : GetLocalPath(bundleName, hash);
			return string.IsNullOrEmpty(path) ? null : AssetBundle.LoadFromFile(path);
		}

		internal AssetBundleCreateRequest LoadFromCache(string bundleName)
		{
			var localPath = GetLocalPath(bundleName);
			return string.IsNullOrEmpty(localPath) ? null : AssetBundle.LoadFromFileAsync(localPath);
		}

		internal AssetBundleCreateRequest LoadFromCache(string bundleName, Hash128 hash, bool ignoreHashCheck)
		{
			var localPath = ignoreHashCheck ? GetLocalPathIgnoreHash(bundleName, hash) : GetLocalPath(bundleName, hash);
			return string.IsNullOrEmpty(localPath) ? null : AssetBundle.LoadFromFileAsync(localPath);
		}

		internal bool ExistedInStreaming(string filePath)
		{
			filePath = filePath.Replace("\\", "/");
			return streamingFiles.Contains(filePath);
		}

		public string GetCachePath(string bundleName)
		{
			return Path.Combine(CachePath, bundleName);
		}

		public string GetCachePath(string bundleName, Hash128 hash)
		{
			return Path.Combine(CachePath, bundleName) + "-" + hash;
		}

		private string GetLocalPath(string bundleName)
		{
			var path = GetCachePath(bundleName);
			if (File.Exists(path)) return path;
			var str = Path.Combine("AssetBundle", bundleName);
			return ExistedInStreaming(str) ? Path.Combine(Application.streamingAssetsPath, str) : string.Empty;
		}

		private string GetLocalPath(string bundleName, Hash128 hash)
		{
			var path = GetCachePath(bundleName);
			if (File.Exists(path)) return path;
			var str = Path.Combine("AssetBundle", bundleName + "-" + hash);
			return ExistedInStreaming(str) ? Path.Combine(Application.streamingAssetsPath, str) : string.Empty;
		}

		private string GetLocalPathIgnoreHash(string bundleName, Hash128 hash)
		{
			var localPath = GetLocalPath(bundleName, hash);
			if (!string.IsNullOrEmpty(localPath)) return localPath;
			var path = GetCachePath(bundleName);
			var fileName = Path.GetFileName(bundleName);
			var dirName = Path.GetDirectoryName(path);
			if (Directory.Exists(dirName))
			{
				var files = Directory.GetFiles(dirName, $"{fileName}-*", SearchOption.TopDirectoryOnly);
				if (files.Length > 0U)
					return files[0];
			}
			var streamingFileStartWith = FindStreamingFileStartWith(Path.Combine("AssetBundle", bundleName + "-"));
			return string.IsNullOrEmpty(streamingFileStartWith)
				? string.Empty
				: Path.Combine(Application.streamingAssetsPath, streamingFileStartWith);
		}

		private string FindStreamingFileStartWith(string filePrefix)
		{
			filePrefix = filePrefix.Replace("\\", "/");
			foreach (var file in streamingFiles.Where(file => file.StartsWith(filePrefix)))
				return file;
			return string.Empty;
		}
	}
}