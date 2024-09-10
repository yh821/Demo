using System;
using UnityEngine;

namespace Game
{
	public struct SpriteLoadItem
	{
		public AssetID assetId;
		public Action<Sprite> complete;

		public SpriteLoadItem(AssetID assetId, Action<Sprite> complete)
		{
			this.assetId = assetId;
			this.complete = complete;
		}
	}
}