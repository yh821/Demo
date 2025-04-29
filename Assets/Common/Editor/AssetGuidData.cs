using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Common
{
	public class AssetGuidData
	{
		public static AssetGuidData Instance
		{
			get
			{
				if (mInstance == null) mInstance = IOHelper.ReadJson<AssetGuidData>(DataPath);
				return mInstance ??= new AssetGuidData();
			}
		}

		private static AssetGuidData mInstance;


		public class GuidRefData
		{
			public string mainGuid;
			public List<string> refGuides;
		}

		public List<GuidRefData> guidRefList;
		public Dictionary<string, long> guidTimeDict;

		public AssetGuidData()
		{
			guidRefList = new List<GuidRefData>();
			guidTimeDict = new Dictionary<string, long>();
		}


		private static readonly Regex GuidRegex = new Regex(@"\{fileID: \d+, guid: (\w+), type: \d+\}");
		private const string DataPath = "UserSettings/AssetGuidData.json";
		private const string LastDataPath = "UserSettings/AssetGuidData_last.json";

		private readonly Dictionary<string, GuidRefData> guidRefDict = new Dictionary<string, GuidRefData>();

		public void Clear()
		{
			guidRefDict.Clear();
			guidTimeDict.Clear();
			guidRefList.Clear();
		}

		private void InitGuidRefDict()
		{
			if (guidRefDict.Count > 0) return;
			foreach (var data in guidRefList)
			{
				if (!guidRefDict.ContainsKey(data.mainGuid)) guidRefDict.Add(data.mainGuid, data);
			}
		}

		private void ReadAssetGuid(string path)
		{
			if (string.IsNullOrEmpty(path)) return;
			var mainGuid = AssetDatabase.AssetPathToGUID(path);
			if (string.IsNullOrEmpty(mainGuid)) return;
			var info = new FileInfo(path);
			var lastTime = info.LastWriteTime.Ticks;
			var time = GetWriteTime(mainGuid);
			if (time >= lastTime) return;
			SetWriteTime(mainGuid, lastTime);
			var content = File.ReadAllText(path);
			var guidMatches = GuidRegex.Matches(content);
			foreach (Match mc in guidMatches) AddAsset(mc.Groups[1].Value, mainGuid);
		}

		private void ReadAllAssetGuid(params string[] searchPatterns)
		{
			if (searchPatterns.Length <= 0) return;
			var dataPath = Application.dataPath;
			var fileList = new List<string>();
			for (int i = 0, len = searchPatterns.Length; i < len; i++)
			{
				var pattern = searchPatterns[i];
				var cancel =
					EditorUtility.DisplayCancelableProgressBar($"正在收集...({i}/{len})", pattern, (float) i / len);
				if (cancel) break;
				var files = Directory.GetFiles(dataPath, pattern, SearchOption.AllDirectories);
				fileList.AddRange(files);
			}
			for (int i = 0, len = fileList.Count; i < len; i++)
			{
				var path = fileList[i].Replace(dataPath, "Assets").Replace("\\", "/");
				var cancel = EditorUtility.DisplayCancelableProgressBar($"正在读取...({i}/{len})", path, (float) i / len);
				if (cancel) break;
				ReadAssetGuid(path);
			}
			EditorUtility.ClearProgressBar();
		}

		public void ReadAllAssets()
		{
			InitGuidRefDict();
			ReadAllAssetGuid("*.prefab", "*.controller", "*.overrideController");
		}

		public void Save()
		{
			guidRefList = new List<GuidRefData>();
			foreach (var data in guidRefDict.Values)
				guidRefList.Add(data);
			guidRefList.Sort((a, b) => string.CompareOrdinal(a.mainGuid, b.mainGuid));
			IOHelper.CopyFile(DataPath, LastDataPath);
			IOHelper.SaveJson(DataPath, Instance);
		}

		public List<Object> GetAssetList(string mainGuid)
		{
			InitGuidRefDict();
			if (!guidRefDict.TryGetValue(mainGuid, out var data)) return null;
			var objList = new List<Object>();
			foreach (var guid in data.refGuides)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				if (string.IsNullOrEmpty(path)) continue;
				var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
				objList.Add(asset);
			}
			return objList;
		}

		public bool IsHaveAssetRef(string mainGuid)
		{
			InitGuidRefDict();
			if (!guidRefDict.TryGetValue(mainGuid, out var data)) return false;
			foreach (var guid in data.refGuides)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				if (string.IsNullOrEmpty(path)) continue;
				var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
				if (asset != null) return true;
			}
			return false;
		}

		private void AddAsset(string refGuid, string mainGuid)
		{
			if (refGuid.StartsWith("000000000000000")) return; //忽略unity默认资源
			var refPath = AssetDatabase.GUIDToAssetPath(refGuid);
			if (string.IsNullOrEmpty(refPath)) return; //missing资源
			if (refPath.EndsWith(".cs") || refPath.EndsWith(".dll")) return; //忽略脚本和库
			if (guidRefDict.TryGetValue(refGuid, out var data))
			{
				if (!data.refGuides.Contains(mainGuid)) data.refGuides.Add(mainGuid);
			}
			else
			{
				data = new GuidRefData {mainGuid = refGuid, refGuides = new List<string> {mainGuid}};
				guidRefDict.Add(refGuid, data);
			}
		}

		private long GetWriteTime(string guid)
		{
			return guidTimeDict.TryGetValue(guid, out var time) ? time : 0;
		}

		private void SetWriteTime(string guid, long time)
		{
			if (guidTimeDict.ContainsKey(guid)) guidTimeDict[guid] = time;
			else guidTimeDict.Add(guid, time);
		}
	}
}