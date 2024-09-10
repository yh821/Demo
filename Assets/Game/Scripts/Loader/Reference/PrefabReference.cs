using UnityEngine;

namespace Game
{
	public class PrefabReference
	{
		private bool isRetained;
		private GameObject prefab;

		~PrefabReference()
		{
			TryRelease(true);
		}

		public void SetPrefab(GameObject prefab)
		{
			if (this.prefab == prefab) return;
			if (this.prefab != null) TryRelease();
			this.prefab = prefab;
			TryRetain();
		}

		private void TryRetain()
		{
			if (prefab == null || isRetained) return;
			isRetained = Singleton<PrefabPool>.Instance.Retain(prefab);
		}

		private void TryRelease(bool finalized = false)
		{
			if (!isRetained || prefab == null) return;
			isRetained = false;
			if (finalized) ReferenceDict.ReleasePrefab(prefab);
			else Singleton<PrefabPool>.Instance.Free(prefab);
			prefab = null;
		}
	}
}