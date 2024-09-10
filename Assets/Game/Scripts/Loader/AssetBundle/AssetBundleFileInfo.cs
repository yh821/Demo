using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace Game
{
	public sealed class AssetBundleFileInfo
	{
		Dictionary<string, int> sizeTable = new Dictionary<string, int>(StringComparer.Ordinal);
		private bool loaded;
		private WaitLoadFileInfo wait;

		public bool Loaded => loaded;
		public bool Loading => wait != null;

		public int GetSize(string name)
		{
			return sizeTable.TryGetValue(name, out var num) ? num : 0;
		}

		internal WaitLoadFileInfo Load(string url)
		{
			if (wait != null) return wait;
			if (Loaded) return new WaitLoadFileInfo(this);
			Debug.Log($"Start load file info: {url}");
			var request = UnityWebRequest.Get(url);
			var asyncOpt = request.Send();
			wait = new WaitLoadFileInfo(this, request, asyncOpt);
			return wait;
		}

		internal void LoadComplete()
		{
			Assert.IsNotNull(wait);
			wait = null;
		}

		internal bool Pares(string data)
		{
			sizeTable.Clear();
			var strs = data.Split(new [] {'\n', ' '}, StringSplitOptions.RemoveEmptyEntries);
			if (strs.Length % 2 > 0)
			{
				Debug.LogError($"[AssetBundleInfo] Pares length failed: {data}");
				return false;
			}
			for (int i = 0; i < strs.Length; i += 2)
			{
				var key = strs[i];
				if (!int.TryParse(strs[i + 1], out var result))
				{
					Debug.LogError($"[AssetBundleInfo] Pares token failed: {strs[i + 1]}");
					return false;
				}
				sizeTable.Add(key, result);
			}
			loaded = true;
			return true;
		}
	}
}