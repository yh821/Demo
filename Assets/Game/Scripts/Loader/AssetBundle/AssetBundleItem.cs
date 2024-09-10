using UnityEngine;

namespace Game
{
	internal sealed class AssetBundleItem
	{
		private int refCount = 0;
		private float lastFreeTime = 0;
		private const float ReleaseDelayTime = 1f;
		private WaitLoadAssetBundle wait;
		private string bundleName;


		internal AssetBundle AssetBundle { get; private set; }
		internal string Error { get; set; }
		internal string BundleName => bundleName;
		internal int RefCount => refCount;
		internal AssetBundleItem[] Dependencies { get; set; }

		internal AssetBundleItem(AssetBundle assetBundle, string bundleName = "")
		{
			AssetBundle = assetBundle;
			this.bundleName = bundleName;
		}

		internal AssetBundleItem(WaitLoadAssetBundle wait, string bundleName = "")
		{
			this.wait = wait;
			this.bundleName = bundleName;
		}

		internal AssetBundleItem(string bundleName = "")
		{
			this.bundleName = bundleName;
		}

		internal void SetWaitLoadAssetBundle(WaitLoadAssetBundle wait)
		{
			this.wait = wait;
		}

		internal bool CheckLoading()
		{
			if (wait == null || wait.keepWaiting) return false;
			if (!string.IsNullOrEmpty(wait.Error)) return false;
			else AssetBundle = wait.AssetBundle;
			return true;
		}

		internal void Retain()
		{
			refCount++;
		}

		internal void Release()
		{
			if (refCount > 0 && --refCount <= 0)
				lastFreeTime = Time.time;
		}

		internal void Destroy(bool unloadAllLoadedObjects = false)
		{
			if (AssetBundle != null)
				AssetBundle.Unload(unloadAllLoadedObjects);
			if (Dependencies == null) return;
			foreach (var dep in Dependencies)
				dep?.Release();
		}

		internal bool Sweep()
		{
			if (refCount > 0 || Time.time < lastFreeTime + 1) return false;
			Destroy(true);
			return true;
		}
	}
}