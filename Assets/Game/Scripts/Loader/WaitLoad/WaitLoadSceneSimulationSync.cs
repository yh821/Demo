using System.IO;
using UnityEditor;
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
#if UNITY_EDITOR
				var parameters = new LoadSceneParameters {loadSceneMode = loadMode};
				UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(abNames[0], parameters);
#endif
			}
		}

		internal override bool Update()
		{
			return false;
		}
	}
}