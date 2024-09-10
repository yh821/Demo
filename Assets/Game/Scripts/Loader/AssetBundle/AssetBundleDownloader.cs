using System;
using System.Collections.Generic;

namespace Game
{
	internal sealed class AssetBundleDownloader
	{
		private int workerCount = 4;
		private Queue<AssetBundleLoadTask> pendingQueue = new Queue<AssetBundleLoadTask>();
		private LinkedList<AssetBundleLoadTask> workingList = new LinkedList<AssetBundleLoadTask>();

		internal event Action<string> DownloadStartEvent;
		internal event Action<string> DownloadFinishEvent;

		public int WorkerCount
		{
			get => workerCount;
			set => workerCount = value;
		}

		internal void Start(AssetBundleLoadTask task)
		{
			if (workingList.Count < workerCount)
				StartTask(task);
			else
				pendingQueue.Enqueue(task);
		}

		internal void Complete(AssetBundleLoadTask task, bool isTryReDownload = false)
		{
			workingList.Remove(task);
			DownloadFinishEvent?.Invoke(task.Url);
			if (isTryReDownload)
				StartTask(task);
			else
			{
				while (workingList.Count < workerCount && pendingQueue.Count > 0)
					StartTask(pendingQueue.Dequeue());
			}
		}

		private void StartTask(AssetBundleLoadTask task)
		{
			task.StartDownload();
			workingList.AddLast(task);
			DownloadStartEvent?.Invoke(task.Url);
		}
	}
}