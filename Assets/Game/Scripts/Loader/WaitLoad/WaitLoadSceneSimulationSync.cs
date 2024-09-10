using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
	public class WaitLoadSceneSimulationSync : WaitLoadScene
	{
		public override float Progress => 1;

		public override bool keepWaiting => false;

		internal WaitLoadSceneSimulationSync(string bundleName, string levelName, LoadSceneMode loadMode)
		{
			var abNames = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName,
				Path.GetFileNameWithoutExtension(levelName));
			if (abNames.Length == 0)
				Error = $"There is no scene with name \"{levelName}\" in \"{bundleName}\"";
			else
			{
				var parameters = new LoadSceneParameters {loadSceneMode = loadMode};
				EditorSceneManager.LoadSceneInPlayMode(abNames[0], parameters);
			}
		}

		internal override bool Update()
		{
			return false;
		}
	}
}