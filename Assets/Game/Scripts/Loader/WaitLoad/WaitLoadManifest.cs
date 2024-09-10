using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
	internal sealed class WaitLoadManifest : WaitLoadObjectFull
	{
		internal WaitLoadManifest(AssetBundleManager abMgr, string bundleName, string assetName, Type assetType)
			: base(abMgr, bundleName, assetName, assetType) { }

		internal override bool Update()
		{
			base.Update();
			if (!string.IsNullOrEmpty(Error))
			{
				AssetBundleManager.DestroyAssetBundle(BundleName);
				return false;
			}
			if (Request == null || !Request.isDone) return true;
			var manifest = GetObject<AssetBundleManifest>();
			AssetBundleManager.DestroyAssetBundle(BundleName);
			Assert.IsNotNull(manifest);
			AssetBundleManager.Manifest = manifest;
			return false;
		}
	}
}