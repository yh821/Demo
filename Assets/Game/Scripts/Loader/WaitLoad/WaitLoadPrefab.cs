using UnityEngine;

namespace Game
{
	public sealed class WaitLoadPrefab : CustomYieldInstruction
	{
		private PrefabCache cache;

		public GameObject LoadedObject { get; private set; }
		public string Error { get; private set; }

		internal WaitLoadPrefab(PrefabCache cache)
		{
			this.cache = cache;
		}

		public override bool keepWaiting
		{
			get
			{
				if (!string.IsNullOrEmpty(cache.Error))
				{
					Error = cache.Error;
					return false;
				}
				if (!cache.HasLoaded()) return true;
				LoadedObject = cache.GetObject();
				return false;
			}
		}
	}
}