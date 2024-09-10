using UnityEngine;

namespace Game
{
	public sealed class WaitLoadAssetBundle : CustomYieldInstruction
	{
		private AssetBundleLoadTask task;
		private AssetBundle assetBundle;
		private WaitForSecondsRealtime simulateFailedWait;

		internal WaitLoadAssetBundle(AssetBundleLoadTask task)
		{
			this.task = task;
		}

		internal WaitLoadAssetBundle(AssetBundle assetBundle)
		{
			this.assetBundle = assetBundle;
		}

		internal WaitLoadAssetBundle(WaitForSecondsRealtime simulateFailedWait)
		{
			this.simulateFailedWait = simulateFailedWait;
		}

		internal string Error
		{
			get
			{
				if (simulateFailedWait != null)
					return simulateFailedWait.keepWaiting ? string.Empty : "Simulate update failed!";
				return task == null ? string.Empty : task.Error;
			}
		}

		public override bool keepWaiting => simulateFailedWait?.keepWaiting ?? string.IsNullOrEmpty(Error) && AssetBundle == null;

		internal AssetBundle AssetBundle
		{
			get
			{
				if (simulateFailedWait != null) return null;
				return assetBundle == null ? task.AssetBundle : assetBundle;
			}
		}
	}
}