using UnityEngine;

namespace Game
{
	internal class WaitLoadObjectFullSync : WaitLoadObject
	{
		private string bundleName;
		private string assetName;
		private System.Type assetType;
		private Object assetObj;

		internal AssetBundleManager AssetBundleManager { get; }

		internal WaitLoadObjectFullSync(AssetBundleManager abMgr, string bundleName, string assetName,
			System.Type assetType)
		{
			AssetBundleManager = abMgr;
			this.bundleName = bundleName;
			this.assetName = assetName;
			this.assetType = assetType;
		}

		public override bool keepWaiting => assetObj == null && string.IsNullOrEmpty(Error);

		internal override bool Update()
		{
			var item = AssetBundleManager.GetAssetBundle(bundleName);
			if (item == null) return true;
			if (!string.IsNullOrEmpty(item.Error))
			{
				Error = item.Error;
				return false;
			}
			assetObj = item.AssetBundle.LoadAsset(assetName, assetType);
			return false;
		}

		public override Object GetObject()
		{
			return assetObj;
		}
	}
}