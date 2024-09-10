using UnityEngine;

namespace Game
{
	internal class WaitLoadObjectFull : WaitLoadObject
	{
		private string bundleName;
		private string assetName;
		private System.Type assetType;

		internal AssetBundleManager AssetBundleManager { get; }
		protected AssetBundleRequest Request { get; private set; }
		protected string BundleName => bundleName;

		internal WaitLoadObjectFull(AssetBundleManager abMgr, string bundleName, string assetName,
			System.Type assetType)
		{
			AssetBundleManager = abMgr;
			this.bundleName = bundleName;
			this.assetName = assetName;
			this.assetType = assetType;
		}

		internal WaitLoadObjectFull(string format, params object[] args)
		{
			Error = string.Format(format, args);
		}

		public override bool keepWaiting =>
			(Request != null || string.IsNullOrEmpty(Error)) && (Request == null || !Request.isDone);

		internal override bool Update()
		{
			var item = AssetBundleManager.GetAssetBundle(bundleName);
			if (item == null) return true;
			if (!string.IsNullOrEmpty(item.Error))
			{
				Error = item.Error;
				return false;
			}
			Request = item.AssetBundle.LoadAssetAsync(assetName, assetType);
			return false;
		}

		public override Object GetObject()
		{
			return Request != null && Request.isDone ? Request.asset : null;
		}
	}
}