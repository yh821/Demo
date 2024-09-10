using UnityEngine;

namespace Game
{
	public sealed class WaitUpdateAssetBundle : CustomYieldInstruction
	{
		private AssetBundleLoadTask task;
		private string error;
		private WaitForSecondsRealtime simulateFailedWait;

		internal WaitUpdateAssetBundle() { }

		internal WaitUpdateAssetBundle(AssetBundleLoadTask task)
		{
			this.task = task;
		}

		internal WaitUpdateAssetBundle(string error)
		{
			this.error = error;
		}

		internal WaitUpdateAssetBundle(WaitForSecondsRealtime simulateFailedWait)
		{
			this.simulateFailedWait = simulateFailedWait;
		}

		public int BytesDownloaded => simulateFailedWait != null || task == null ? 0 : task.BytesDownloaded;
		public int ContentLength => simulateFailedWait != null || task == null ? 0 : task.ContentLength;
		public float Progress => simulateFailedWait != null || task == null ? 0 : task.Progress;
		public int DownloadSpeed => simulateFailedWait != null || task == null ? 0 : task.DownloadSpeed;

		public string Error
		{
			get
			{
				if (simulateFailedWait != null)
					return simulateFailedWait.keepWaiting ? string.Empty : "Simulate update failed!";
				if (!string.IsNullOrEmpty(error))
					return error;
				return task == null ? string.Empty : task.Error;
			}
		}

		public override bool keepWaiting =>
			simulateFailedWait?.keepWaiting ?? Error == null && task != null && task.Updating;
	}
}