using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
	public sealed class WaitLoadFileInfo : CustomYieldInstruction
	{
		private AssetBundleFileInfo fileInfo;
		private UnityWebRequest request;
		private AsyncOperation asyncOpt;

		internal WaitLoadFileInfo(AssetBundleFileInfo fileInfo)
		{
			this.fileInfo = fileInfo;
		}

		internal WaitLoadFileInfo(AssetBundleFileInfo fileInfo, UnityWebRequest request, AsyncOperation asyncOpt)
		{
			this.fileInfo = fileInfo;
			this.request = request;
			this.asyncOpt = asyncOpt;
		}

		public string Error { get; private set; }

		public AssetBundleFileInfo FileInfo => fileInfo;

		public override bool keepWaiting
		{
			get
			{
				if (fileInfo.Loaded) return false;
				if (!asyncOpt.isDone) return true;
				fileInfo.LoadComplete();
				if (request.result != UnityWebRequest.Result.Success)
				{
					Error = request.error;
					request.Dispose();
					request = null;
					return false;
				}
				var content = DownloadHandlerBuffer.GetContent(request);
				request.Dispose();
				request = null;
				if (!fileInfo.Pares(content)) Error = "Parse file info failed!";
				return false;
			}
		}
	}
}