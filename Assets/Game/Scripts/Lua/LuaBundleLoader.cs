using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using LuaInterface;
using UnityEngine;
using Game;

public sealed class LuaBundleLoader : LuaFileUtils
{
#if UNITY_IOS || UNITY_STANDALONE
	private string AssetPrefix = "Assets/Game/LuaBundle/";
#else
    private string AssetPrefix = "Assets/Game/LuaBundleJit64/";
#endif

	private Dictionary<string, string> lookup = new Dictionary<string, string>(StringComparer.Ordinal);
	private Dictionary<string, AssetBundle> assetBundleMap = new Dictionary<string, AssetBundle>();

	private Dictionary<string, string> resAliasPathMap = new Dictionary<string, string>();
	private HashSet<string> notSaveBundleMap = new HashSet<string>();

	private bool isLoadAliasResPath = false;

	public LuaBundleLoader()
	{
		instance = this;
		beZip = false;

#if UNITY_ANDROID
		if (!GameRoot.IsAndroid64())
			AssetPrefix = "Assets/Game/LuaBundleJit32/";
#endif
	}

	public void PruneLuaBundles()
	{
		foreach (var key in assetBundleMap.Keys)
		{
			var bundle = assetBundleMap[key];
			bundle.Unload(false);
		}

		assetBundleMap.Clear();
	}

	public void AddLuaBundle(string luaFile, string luaBundle)
	{
		if (lookup.ContainsKey(luaFile))
			lookup.Add(luaFile.ToLower(), luaBundle);
	}

	public void SetupLuaLoader(LuaState luaState)
	{
		Debugger.Log("Start setup lua lookup");
		var initCode = @"
		local LUA_ASSET_BUNDLE_PREFIX
		local LUA_ASSET_PREFIX

		if UNITY_IOS or UNITY_STANDALONE then
		    LUA_ASSET_BUNDLE_PREFIX = 'lua/'
		    LUA_ASSET_PREFIX = 'Assets/Game/LuaBundle/'
		else
		    if GameRoot.IsAndroid64() then
		        LUA_ASSET_BUNDLE_PREFIX = 'luajit64/'
		        LUA_ASSET_PREFIX = 'Assets/Game/LuaBundleJit64/'
		    else
		        LUA_ASSET_BUNDLE_PREFIX = 'luajit32/'
		        LUA_ASSET_PREFIX = 'Assets/Game/LuaBundleJit32/'
		    end
		end

		local SysFile = System.IO.File
		local UnityApplication = UnityEngine.Application
		local UnityAppStreamingAssetsPath = UnityApplication.streamingAssetsPath
		local UnityAppPersistentDataPath = UnityApplication.persistentDataPath

		if not UNITY_EDITOR then
		    local cacheDir = EncryptMgr.GetEncryptPath('BundleCache')
		    local cachePath = string.format('%s/%s', UnityAppPersistentDataPath, cacheDir)

		    local luaAssetBundle = 'LuaAssetBundle/LuaAssetBundle.lua'
		    local cacheLuaAssetBundle = EncryptMgr.GetEncryptPath(luaAssetBundle)
		    local luaAssetBundleData

		    local cacheFullPath = string.format('%s/%s', cachePath, cacheLuaAssetBundle)
		    if SysFile.Exists(cacheFullPath) then
		        luaAssetBundleData = SysFile.ReadAllText(cacheFullPath)
		    else
		        local aliasPath = GameRoot.GetAliasResPath('AssetBundle/' .. luaAssetBundle)
		        if EncryptMgr.IsEncryptAsset then
		            luaAssetBundleData = EncryptMgr.ReadEncryptFile(string.format('%s/%s', UnityAppStreamingAssetsPath, aliasPath))
		        else
		            luaAssetBundleData = StreamingAssets.ReadAllText(aliasPath)
		        end
		    end

		    local pattern = LUA_ASSET_BUNDLE_PREFIX .. '.+'
		    local luaBundleInfos = loadstring(luaAssetBundleData)().bundleInfos

		    for bundleName, bundleInfo in pairs(luaBundleInfos) do
		        if string.match(bundleName, pattern) then
		            local hash = bundleInfo.hash
		            local relativePath = string.format('LuaAssetBundle/%s-%s', bundleName, hash)
		            relativePath = EncryptMgr.GetEncryptPath(relativePath)
		            local path = string.format('%s/%s', cachePath, relativePath)
		            if not SysFile.Exists(path) then
		                path = string.format('%s/AssetBundle/', UnityAppStreamingAssetsPath, relativePath)
		            end
		            for i, file in ipairs(bundleInfo.deps) do
		                AddLuaBundle(file, path)
		            end
		        end
		    end
		end

		AddLuaBundle = nil
		";
		luaState.DoString(initCode);
		Debugger.Log($"setup lua lookup complete, lua count:{lookup.Count}");
	}

	private string GetLuaFileFullPath(string fileName)
	{
		if (!fileName.EndsWith(".lua")) fileName += ".lua";
		if (!fileName.EndsWith(".bytes")) fileName += ".bytes";
		var filePath = AssetPrefix + fileName;
		return filePath.ToLower();
	}

	public bool IsLuaFileExist(string fileName)
	{
#if UNITY_EDITOR
		var path = FindFile(fileName);
		return !string.IsNullOrEmpty(path) && File.Exists(path);
#else
		var filePath = GetLuaFileFullPath(fileName);
		return lookup.TryGetValue(filePath, out _);
#endif
	}

	public override byte[] ReadFile(string fileName)
	{
#if UNITY_EDITOR
		return base.ReadFile(fileName);
#else
		return ReadAssetBundleFile(fileName);
#endif
	}

	private byte[] ReadAssetBundleFile(string fileName)
	{
		var filePath = GetLuaFileFullPath(fileName).ToLower();
		if (!lookup.TryGetValue(filePath, out var bundlePath))
		{
			Debug.LogError($"load lua file failed: {filePath}, bundle is not existed.");
			return null;
		}

		//这个ab包用完立即销毁，不能缓存
		var notSave = notSaveBundleMap.Contains(bundlePath);
		if (!assetBundleMap.TryGetValue(bundlePath, out var assetBundle))
		{
			var realBundlePath = bundlePath;
			if (EncryptMgr.IsEncryptAsset)
			{
				var cacheDir = EncryptMgr.GetEncryptPath("/BundleCache/");
				if (bundlePath.IndexOf(cacheDir) < 0)
				{
					var relativeBundleName = Regex.Replace(bundlePath, Application.streamingAssetsPath, "");
					var relativePath = $"DecryptLuaCache/{relativeBundleName}";
					relativePath = EncryptMgr.GetEncryptPath(relativePath);
					var decryptTargetPath = $"{Application.persistentDataPath}/{relativePath}";
					if (EncryptMgr.DecryptAssetBundle(bundlePath, decryptTargetPath))
						realBundlePath = decryptTargetPath;
				}
			}

			assetBundle = AssetBundle.LoadFromFile(realBundlePath);
			if (assetBundle == null)
			{
				Debug.LogError($"AssetBundle.LoadFromFile is failed: {bundlePath}, {fileName}, {realBundlePath}");
				var cacheDirName = "/BundleCache/";
				if (EncryptMgr.IsEncryptAsset)
					cacheDirName = EncryptMgr.GetEncryptPath(cacheDirName);
				if (bundlePath.IndexOf(cacheDirName) > 0 && File.Exists(realBundlePath))
				{
					Debug.LogError($"Delete Lua {realBundlePath}");
					File.Delete(realBundlePath);
				}

				return null;
			}

			if (!notSave) assetBundleMap.Add(bundlePath, assetBundle);
		}

		if (assetBundle == null)
		{
			Debug.LogError($"load bundle is failed: {bundlePath}, {fileName}");
			return null;
		}

		var textAsset = assetBundle.LoadAsset<TextAsset>(filePath);
		if (textAsset == null)
		{
			if (notSave) assetBundle.Unload(true);
			Debug.LogError($"load lua file failed: {fileName}, can not load asset from bundle");
			return null;
		}

		var buffer = textAsset.bytes;
		Resources.UnloadAsset(textAsset);
		if (notSave) assetBundle.Unload(true);
		return buffer;
	}

	public void LoadAliasResPathMap()
	{
#if UNITY_IOS
#endif
	}

	public string GetAliasResPath(string path)
	{
		if (!isLoadAliasResPath) return path;
		return !resAliasPathMap.TryGetValue(path, out var aliasPath) ? path : aliasPath;
	}

	//客户端需要在线热更某个配置，可能会出现新旧ab包同时出现的情况，这里需要记录这些ab包
	public void OverrideLuaBundle(string bundleName, string fileName)
	{
		var filePath = GetLuaFileFullPath(fileName).ToLower();
		if (lookup.TryGetValue(filePath, out var oldBundleName))
		{
			lookup[filePath] = bundleName;
			if (!notSaveBundleMap.Contains(oldBundleName))
			{
				notSaveBundleMap.Add(oldBundleName);
				if (assetBundleMap.TryGetValue(oldBundleName, out var assetBundle))
				{
					assetBundle.Unload(true);
					assetBundleMap.Remove(oldBundleName);
				}
			}
		}
		else
		{
			lookup.Add(filePath, bundleName);
		}

		if (!notSaveBundleMap.Contains(bundleName))
		{
			notSaveBundleMap.Add(bundleName);
			if (assetBundleMap.TryGetValue(bundleName, out var assetBundle))
			{
				assetBundle.Unload(true);
				assetBundleMap.Remove(bundleName);
			}
		}
	}
}