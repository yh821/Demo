using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
	public sealed class AssetSimulator
	{
		private const string SimulateAssetBundleModeKey = "SimulateAssetBundleMode";
		private const string SimulateAssetBundleFailedKey = "SimulateAssetBundleFailed";

		private int simulateAssetBundleMode = -1;
		private int simulateAssetBundle = -1;
		private float simulateLoadTimeMin = 0;
		private float simulateLoadTimeMax = 1;
		private int simulateAssetBundleFailed = -1;

		public int SimulateLoadModeInEditor
		{
			get
			{
				if (simulateAssetBundleMode == -1)
					simulateAssetBundleMode = EditorPrefs.GetInt(SimulateAssetBundleModeKey, 1);
				return simulateAssetBundleMode;
			}
			set
			{
				if (simulateAssetBundleMode == value) return;
				simulateAssetBundleMode = value;
				EditorPrefs.SetInt(SimulateAssetBundleModeKey, value);
			}
		}

		public bool SimulateAssetBundle
		{
			get
			{
				if (simulateAssetBundle < 0) FlushSimulate();
				return simulateAssetBundle > 0;
			}
		}

		public bool SimulateAssetBundleFailed
		{
			get
			{
				if (simulateAssetBundleFailed < 0)
					simulateAssetBundleFailed = EditorPrefs.GetBool(SimulateAssetBundleFailedKey, false) ? 1 : 0;
				return simulateAssetBundleFailed > 0;
			}
			set
			{
				if (simulateAssetBundleFailed > 0 == value) return;
				simulateAssetBundleFailed = value ? 1 : 0;
				EditorPrefs.SetBool(SimulateAssetBundleFailedKey, value);
			}
		}

		public float SimulateLoadTimeMin
		{
			get
			{
				if (simulateLoadTimeMin < 0) FlushSimulate();
				return simulateLoadTimeMin;
			}
		}

		public float SimulateLoadTimeMax
		{
			get
			{
				if (simulateLoadTimeMax < 0) FlushSimulate();
				return simulateLoadTimeMax;
			}
		}

		internal WaitLoadObject LoadDelay()
		{
			return new WaitLoadObjectSimulation(null, Random.Range(SimulateLoadTimeMin, SimulateLoadTimeMax));
		}

		internal WaitForSecondsRealtime SimulateAssetBundleFailedRequest()
		{
			return !SimulateAssetBundleFailed || Random.Range(0, 100) > 50
				? null
				: new WaitForSecondsRealtime(Random.Range(1, 5));
		}

		internal Object LoadObjectLocal(string bundleName, string assetName, System.Type assetType)
		{
			var abNames = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName,
				Path.GetFileNameWithoutExtension(assetName));
			string assetPath = null;
			if (Path.HasExtension(assetName))
			{
				var name1 = Path.GetFileNameWithoutExtension(assetName);
				var ext1 = Path.GetExtension(assetName);
				foreach (var path in abNames)
				{
					var name2 = Path.GetFileNameWithoutExtension(path);
					var ext2 = Path.GetExtension(path);
					if (name1 == name2 && ext1 == ext2)
					{
						assetPath = path;
						break;
					}
				}
			}
			else if (abNames.Length > 0)
			{
				var name = Path.GetFileNameWithoutExtension(assetName);
				foreach (var path in abNames)
				{
					if (Path.GetFileNameWithoutExtension(path) == name)
					{
						assetPath = path;
						break;
					}
				}
			}
			return string.IsNullOrEmpty(assetPath) ? null : AssetDatabase.LoadAssetAtPath(assetPath, assetType);
		}

		internal WaitLoadObject LoadObject(string bundleName, string assetName, System.Type assetType)
		{
			var simulateObject = LoadObjectLocal(bundleName, assetName, assetType);
			var simulateTime = Random.Range(SimulateLoadTimeMin, SimulateLoadTimeMax);
			if (simulateObject != null) return new WaitLoadObjectSimulation(simulateObject, simulateTime);
			return new WaitLoadObjectSimulation("Load object {0}:{1} failed!", bundleName, assetName);
		}

		internal WaitLoadObject LoadObjectSync(string bundleName, string assetName, System.Type assetType)
		{
			var simulateObject = LoadObjectLocal(bundleName, assetName, assetType);
			if (simulateObject != null) return new WaitLoadObjectSimulation(simulateObject, 0);
			return new WaitLoadObjectSimulation("Load object {0}:{1} failed!", bundleName, assetName);
		}

		internal WaitLoadScene LoadScene(string bundleName, string sceneName, LoadSceneMode loadMode)
		{
			var simulateTime = Random.Range(SimulateLoadTimeMin, SimulateLoadTimeMax);
			return new WaitLoadSceneSimulation(bundleName, sceneName, loadMode, simulateTime);
		}

		internal WaitLoadScene LoadSceneSync(string bundleName, string sceneName, LoadSceneMode loadMode)
		{
			return new WaitLoadSceneSimulationSync(bundleName, sceneName, loadMode);
		}

		private void FlushSimulate()
		{
			switch (SimulateLoadModeInEditor)
			{
				case 0:
					simulateAssetBundle = 0;
					simulateLoadTimeMin = 0;
					simulateLoadTimeMax = 0;
					break;
				case 1:
					simulateAssetBundle = 1;
					simulateLoadTimeMin = 0;
					simulateLoadTimeMax = 0;
					break;
				case 2:
					simulateAssetBundle = 1;
					simulateLoadTimeMin = 0;
					simulateLoadTimeMax = 0.1f;
					break;
				case 3:
					simulateAssetBundle = 1;
					simulateLoadTimeMin = 0;
					simulateLoadTimeMax = 0.5f;
					break;
				case 4:
					simulateAssetBundle = 1;
					simulateLoadTimeMin = 0.5f;
					simulateLoadTimeMax = 0.5f;
					break;
			}
		}
	}
}