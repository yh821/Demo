using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
	public class WaitLoadSceneSimulation : WaitLoadScene
	{
		private AsyncOperation asyncOpt;
		private float simulateTime;

		public override float Progress => asyncOpt?.progress ?? 0;

		internal WaitLoadSceneSimulation(string bundleName, string sceneName, LoadSceneMode loadMode,
			float simulateTime)
		{
			this.simulateTime = simulateTime;
			var abNames = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName,
				Path.GetFileNameWithoutExtension(sceneName));
			if (abNames.Length == 0)
				Error = $"There is no scene with name \"{sceneName}\" in \"{bundleName}\"";
			else
			{
#if UNITY_EDITOR
				var parameters = new LoadSceneParameters {loadSceneMode = loadMode};
				asyncOpt = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(abNames[0],
					parameters);
#endif
			}
		}

		public override bool keepWaiting
		{
			get
			{
				simulateTime -= Time.unscaledDeltaTime;
				return simulateTime > 0 || asyncOpt != null && !asyncOpt.isDone;
			}
		}

		internal override bool Update()
		{
			return false;
		}
	}
}