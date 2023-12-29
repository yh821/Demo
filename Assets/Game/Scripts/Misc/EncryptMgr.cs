using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using LuaInterface;
using UnityEngine;

public class EncryptMgr
{
	private static byte[] encryptKey;
	public static bool IsEncryptAsset { get; private set; } = false;
	public static bool IsEncryptPath { get; private set; } = false;

	public static void InitEncryptKey()
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		var key = string.Empty;
#else
		var key = GameRoot.ProjCode;
#endif
		if (string.IsNullOrEmpty(key))
		{
			IsEncryptAsset = false;
			return;
		}

		encryptKey = CalcEncryptKey(key);
		IsEncryptAsset = true;
		IsEncryptPath = true;
	}

	private static byte[] CalcEncryptKey(string key)
	{
		var args = key.Split(',');
		var keys = new byte[args.Length];
		for (var i = 0; i < args.Length; i++)
			keys[i] = Convert.ToByte(args[i]);
		return keys;
	}

#if UNITY_EDITOR
	[NoToLua]
	public static void EncryptSteamFiles()
	{
		var path = Path.Combine(Application.streamingAssetsPath, "file_list.txt");
		var key = CalcEncryptKey(GameRoot.ProjCode);
		var lines = File.ReadAllLines(path);
		EncryptSteam fileSteam = null;
		var streamingAssetsPath = Application.streamingAssetsPath;
		foreach (var line in lines)
		{
			var abPath = Path.Combine(streamingAssetsPath, line);
			var abData = File.ReadAllBytes(abPath);
			fileSteam = new EncryptSteam(abPath, FileMode.Create);
			fileSteam.SetEncryptKey(key);
			fileSteam.Write(abData, 0, abData.Length);
			fileSteam.Close();
			fileSteam.Dispose();
		}

		var data = File.ReadAllBytes(path);
		fileSteam = new EncryptSteam(path, FileMode.Create);
		fileSteam.SetEncryptKey(key);
		fileSteam.Write(data, 0, data.Length);
		fileSteam.Close();
		fileSteam.Dispose();
	}
#endif

	private static Dictionary<string, string> encryptPathMapO2N = new Dictionary<string, string>();
	private static Dictionary<string, string> encryptPathMapN2O = new Dictionary<string, string>();

	public static string GetEncryptPath(string path)
	{
		if (!IsEncryptPath || !IsEncryptAsset) return path;
		if (encryptPathMapO2N.ContainsKey(path)) return encryptPathMapO2N[path];

		var args = path.Split('/');
		var sb = new StringBuilder();
		for (int i = 0; i < args.Length; i++)
		{
			var name = args[i];
			if (string.IsNullOrEmpty(name)) continue;
			if (i != 0) sb.Append("/");
			var bytes = Encoding.Default.GetBytes(name);
			for (int j = 0; j < bytes.Length; j++)
				bytes[j] ^= encryptKey[j % encryptKey.Length];

			var k = 0;
			var total = 0;
			while (k + 4 <= bytes.Length)
			{
				var num = bytes[k + 3] & 0xFF;
				num |= (bytes[k + 2] << 8) & 0xFF00;
				num |= (bytes[k + 1] << 16) & 0xFF0000;
				num |= (bytes[k] << 24) & 0xFF0000;
				k += 4;
				total += num;
			}

			var newName = total == 0 ? name : WordLibrary.GetEnWord(total);
			sb.Append(newName).Append('_').Append(name);
		}

		var newPath = sb.ToString();
		encryptPathMapO2N.Add(path, newPath);
		if (encryptPathMapN2O.ContainsKey(newPath))
			Debug.LogError($"GetEncryptPath calc new path error: {path}, {newPath}, {encryptPathMapN2O[newPath]}");
		else
			encryptPathMapN2O.Add(newPath, path);
		return newPath;
	}

	public static string ReadEncryptFile(string path)
	{
		if (!IsEncryptAsset || !File.Exists(path)) return string.Empty;
		var size = Convert.ToInt32(new FileInfo(path).Length);
		var fileSteam = new EncryptSteam(path, FileMode.Open, FileAccess.Read, FileShare.None, size, false);
		var buffer = new byte[size];
		var length = fileSteam.Read(buffer, 0, buffer.Length);
		fileSteam.Close();
		fileSteam.Dispose();
		return Encoding.UTF8.GetString(buffer, 0, length);
	}

	public static bool DecryptAssetBundle(string path, string targetPath)
	{
		if (!IsEncryptAsset || !File.Exists(path)) return false;
		if (File.Exists(targetPath)) return true;
		var dirName = Path.GetDirectoryName(targetPath);
		if (!Directory.Exists(dirName))
			Directory.CreateDirectory(dirName);
		var data = File.ReadAllBytes(path);
		var fileSteam = new EncryptSteam(targetPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None,
			data.Length, false);
		fileSteam.SetEncryptKey(encryptKey);
		fileSteam.Write(data, 0, data.Length);
		fileSteam.Close();
		fileSteam.Dispose();
		return true;
	}

	public static string DecryptAgentAssets(string path)
	{
		if (!IsEncryptAsset || !File.Exists(path) || !path.StartsWith(Application.streamingAssetsPath))
			return string.Empty;
		var relativePath = Regex.Replace(path, Application.streamingAssetsPath, "");
		var targetPath = $"{Application.persistentDataPath}/AgentAssets/{relativePath}";
		if (File.Exists(targetPath)) return targetPath;
		var dirName = Path.GetDirectoryName(targetPath);
		if (!Directory.Exists(dirName))
			Directory.CreateDirectory(dirName);
		var data = File.ReadAllBytes(path);
		var fileSteam = new EncryptSteam(targetPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None,
			data.Length, false);
		fileSteam.SetEncryptKey(encryptKey);
		fileSteam.Write(data, 0, data.Length);
		fileSteam.Close();
		fileSteam.Dispose();
		return targetPath;
	}
}