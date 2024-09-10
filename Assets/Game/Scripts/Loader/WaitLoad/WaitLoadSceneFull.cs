using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
	internal sealed class WaitLoadSceneFull : WaitLoadScene
	{
		private AssetBundleManager assetBundleManager;
		private string bundleName;
		private string sceneName;
		private LoadSceneMode loadMode;
		private AsyncOperation asyncOpt;

		internal WaitLoadSceneFull(AssetBundleManager assetBundleManager, string bundleName, string sceneName,
			LoadSceneMode loadMode)
		{
			this.assetBundleManager = assetBundleManager;
			this.bundleName = bundleName;
			this.sceneName = Path.GetFileNameWithoutExtension(sceneName);
			this.loadMode = loadMode;
		}

		internal WaitLoadSceneFull(string format, params object[] args)
		{
			Error = string.Format(format, args);
		}

		public override bool keepWaiting =>
			(asyncOpt != null || string.IsNullOrEmpty(Error)) && (asyncOpt == null || !asyncOpt.isDone);

		public override float Progress => asyncOpt?.progress ?? 0;

		internal override bool Update()
		{
			var item = assetBundleManager.GetAssetBundle(bundleName);
			if (item == null) return true;
			if (!string.IsNullOrEmpty(item.Error))
			{
				Error = item.Error;
				return false;
			}
			asyncOpt = SceneManager.LoadSceneAsync(sceneName, loadMode);
			return false;
		}
	}
}