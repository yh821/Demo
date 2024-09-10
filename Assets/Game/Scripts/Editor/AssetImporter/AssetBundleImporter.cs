using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class GamePath
{
	public const string BaseDir = "Assets/Game";
	public const string SceneDir = BaseDir + "/Scenes";
	public const string AudioDir = BaseDir + "/Audios";
	public const string EnvironmentsDir = BaseDir + "/Environments";

	public const string ActorDir = BaseDir + "/Actors";
	public const string RoleDir = ActorDir + "/Role";
	public const string MonsterDir = ActorDir + "/Monster";
	public const string PetDir = ActorDir + "/Pet";

	//UI
	public const string UIsDir = BaseDir + "/UIs";
	public const string ViewDir = UIsDir + "/View";
	public const string IconsDir = UIsDir + "/Icons";
	public const string TtfDir = UIsDir + "/TTF";
	public const string FontsDir = UIsDir + "/Fonts";
	public const string FontAtlasDir = UIsDir + "/FontAtlas";

	public const string ImageFlag = "/image";
	public const string RawImageFlag = "/rawimage";
	public const string NoPackFlag = "/nopack";
}

public static class AssetBundleImporter
{
	public const string IgnoreMark = "ignore_mark";

	private static readonly Regex LogicRegex = new Regex(@".+logic\d+\.unity");

	private static readonly char[] Seperator =
	{
		'/', '\\'
	};

	public static void MarkAssetBundle(string asset)
	{
		if (!asset.StartsWith(GamePath.BaseDir)) return;
		var ext = Path.GetExtension(asset);
		if (".cs".Equals(ext, StringComparison.OrdinalIgnoreCase)) return;

		var importer = AssetImporter.GetAtPath(asset);
		if (!importer) return;

		var bundleName = GetAssetBundleName(asset);
		bundleName = FixAssetBundleName(bundleName);
		if (!string.Equals(importer.assetBundleName, bundleName))
		{
			importer.assetBundleName = bundleName;
			importer.SaveAndReimport();
		}
	}

	private static string FixAssetBundleName(string bundleName)
	{
		bundleName = bundleName.Replace(" ", "");
		bundleName = bundleName.Replace("—", "-");
		bundleName = Regex.Replace(bundleName, "[\u4E00-\u9FA5]+", "");

		return bundleName;
	}

	private static string GetAssetBundleName(string asset)
	{
		if (!IsNeedMark(asset)) return string.Empty;
		var bundleName = string.Empty;
		if (string.IsNullOrEmpty(bundleName)) bundleName = TryGetFontName(asset);
		if (string.IsNullOrEmpty(bundleName)) bundleName = TryGetPrefabName(asset);
		if (string.IsNullOrEmpty(bundleName)) bundleName = TryGetSceneName(asset);
		if (string.IsNullOrEmpty(bundleName)) bundleName = TryGetAudioName(asset);
		if (string.IsNullOrEmpty(bundleName)) bundleName = TryGetUiName(asset);
		return bundleName.ToLower();
	}

	private static string TryGetUiName(string asset)
	{
		if (!asset.StartsWith(GamePath.UIsDir)) return string.Empty;
		var importer = AssetImporter.GetAtPath(asset);
		if (importer is TextureImporter || asset.EndsWith(AtlasImporter.AtlasFileSuffix))
		{
			if (asset.Contains(GamePath.RawImageFlag))
			{
				var assetName = Path.GetFileNameWithoutExtension(asset).ToLower();
				var bundleName = "uis/rawimages/" + assetName;
				var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetName);
				if (assetPaths.Length == 0) return bundleName;
				if (assetPaths.Length == 1 && assetPaths[0] == asset) return bundleName;
				var newName = assetName + "_1";
				AssetDatabase.RenameAsset(asset, newName);
				Debug.LogError($"发现相同名字的RawImage, {asset} 重命名为 {newName}");
				return string.Empty;
			}

			var uisPath = GetRelativeDirPath(asset, GamePath.UIsDir);
			if (uisPath == "icons/item")
			{
				if (int.TryParse(Path.GetFileNameWithoutExtension(asset), out var num1))
				{
					var num2 = num1 / 100; //每100区间的打一个ab包
					return $"uis/{uisPath}_{num2}_atlas";
				}
			}

			return $"uis/{uisPath}_atlas";
		}
		return string.Empty;
	}

	private static string TryGetFontName(string asset)
	{
		if (asset.StartsWith(GamePath.TtfDir)) return "uis/ttf_bundle";
		if (asset.StartsWith(GamePath.FontAtlasDir)) return "uis/fonts_bundle";
		if (asset.StartsWith(GamePath.FontsDir))
			return AssetDatabase.GetMainAssetTypeAtPath(asset) == typeof(Font) ? "uis/fonts_bundle" : IgnoreMark;
		return string.Empty;
	}

	private static string TryGetAudioName(string asset)
	{
		if (asset.StartsWith(GamePath.AudioDir) && Path.GetDirectoryName(asset) != GamePath.AudioDir)
			return GetRelativeDirPath(asset, GamePath.BaseDir);
		return string.Empty;
	}

	private static string TryGetSceneName(string asset)
	{
		asset = asset.ToLower();
		if (asset.EndsWith(".unity") && !LogicRegex.IsMatch(asset))
		{
			var bundleName = GetRelativeDirPath(asset, GamePath.BaseDir);
			var sceneMark = Regex.Replace(Path.GetFileNameWithoutExtension(asset), ".+_", "_");
			return bundleName + sceneMark;
		}

		return string.Empty;
	}

	private static string TryGetPrefabName(string asset)
	{
		if (asset.EndsWith(".prefab"))
		{
			if (asset.Contains("/Editor") || asset.Contains("/editor")) return string.Empty;
			if (asset.StartsWith(GamePath.RoleDir)) return GetActorBundleName("role", asset);
			if (asset.StartsWith(GamePath.PetDir)) return GetActorBundleName("pet", asset);
			if (asset.StartsWith(GamePath.MonsterDir)) return GetActorBundleName("monster", asset);
			if (asset.StartsWith(GamePath.ActorDir)) return GetAssetBundleName("actors", asset);
			if (!asset.StartsWith(GamePath.EnvironmentsDir))
				return GetRelativeDirPath(asset, GamePath.BaseDir) + "_prefab";
		}

		return string.Empty;
	}

	private static string GetAssetBundleName(string dir, string asset)
	{
		var paths = asset.Split(Seperator);
		var parentDir = paths[paths.Length - 2];
		if (string.CompareOrdinal(parentDir, dir) != 0)
			return $"{dir}/{parentDir}_prefab";
		return GetRelativeDirPath(asset, GamePath.BaseDir);
	}

	private static string GetActorBundleName(string dir, string asset)
	{
		var paths = asset.Split(Seperator);
		var parentDir = paths[paths.Length - 2];
		if (string.CompareOrdinal(parentDir, dir) != 0)
			return $"actors/{dir}/{parentDir}_prefab";
		return GetRelativeDirPath(asset, GamePath.BaseDir);
	}

	private static string GetViewBundleName(string dir, string asset)
	{
		var paths = asset.Split(Seperator);
		var parentDir = paths[paths.Length - 2];
		if (string.CompareOrdinal(parentDir, dir) != 0)
			return $"views/{dir}/{parentDir}_prefab";
		return GetRelativeDirPath(asset, GamePath.BaseDir);
	}

	private static string GetRelativeDirPath(string path, string basePath)
	{
		basePath = basePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
		var relativePath = path.Substring(basePath.Length + 1);
		return Path.GetDirectoryName(relativePath)?.ToLower().Replace("\\", "/");
	}

	private static bool IsNeedMark(string asset)
	{
		if (AssetDatabase.IsValidFolder(asset)) return false;
		if (asset.Contains("/Editor/") || asset.Contains("/editor/")) return false;
		return true;
	}
}