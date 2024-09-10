using UnityEngine;

namespace Game
{
	public class SpriteReference
	{
		private bool isRetained;
		private Sprite sprite;

		~SpriteReference()
		{
			TryRelease(true);
		}

		public void SetSprite(Sprite sprite)
		{
			if (this.sprite == sprite) return;
			if (this.sprite != null) TryRelease();
			this.sprite = sprite;
			TryRetain();
		}

		private void TryRetain()
		{
			if (sprite == null || isRetained) return;
			isRetained = Singleton<SpritePool>.Instance.Retain(sprite);
		}

		private void TryRelease(bool finalized = false)
		{
			if (!isRetained || sprite == null) return;
			isRetained = false;
			if (finalized) ReferenceDict.ReleaseSprite(sprite);
			else Singleton<SpritePool>.Instance.Free(sprite);
			sprite = null;
		}
	}
}