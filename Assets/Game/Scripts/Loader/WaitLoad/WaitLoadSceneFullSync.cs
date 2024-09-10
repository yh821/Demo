using System.IO;
using UnityEngine.SceneManagement;

namespace Game
{
	internal sealed class WaitLoadSceneFullSync : WaitLoadScene
	{
		private AssetBundleManager assetBundleManager;
		private string bundleName;
		private string sceneName;
		private LoadSceneMode loadMode;
		private bool complete;

		internal WaitLoadSceneFullSync(AssetBundleManager assetBundleManager, string bundleName, string sceneName,
			LoadSceneMode loadMode)
		{
			this.assetBundleManager = assetBundleManager;
			this.bundleName = bundleName;
			this.sceneName = Path.GetFileNameWithoutExtension(sceneName);
			this.loadMode = loadMode;
			complete = false;
		}

		public override bool keepWaiting => !complete && string.IsNullOrEmpty(Error);

		public override float Progress => 1;

		internal override bool Update()
		{
			var item = assetBundleManager.GetAssetBundle(bundleName);
			if (item == null) return true;
			if (!string.IsNullOrEmpty(item.Error))
			{
				Error = item.Error;
				return false;
			}
			SceneManager.LoadScene(sceneName, loadMode);
			complete = true;
			return false;
		}
	}
}