using System;
using UnityEngine;

namespace Game
{
	public struct PrefabLoadItem
	{
		public AssetID assetId;
		public Action<GameObject> complete;

		public PrefabLoadItem(AssetID assetId, Action<GameObject> complete)
		{
			this.assetId = assetId;
			this.complete = complete;
		}
	}
}