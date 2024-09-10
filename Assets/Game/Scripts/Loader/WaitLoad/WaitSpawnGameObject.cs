using UnityEngine;

namespace Game
{
	public sealed class WaitSpawnGameObject : CustomYieldInstruction
	{
		private GameObjectCache cache;
		private GameObject instance;
		private InstantiateQueue instantiateQueue;
		private int instantiatePriority;
		private bool instantiating;

		public Sprite LoadedObject { get; private set; }
		public string Error { get; private set; }

		public GameObject Instance
		{
			get
			{
				if (!cache.HasPrefab) return null;
				if (instantiateQueue == null && instance == null)
					instance = cache.Spawn(null);
				cache.Loading = false;
				return instance;
			}
		}

		public WaitSpawnGameObject(GameObjectCache cache, InstantiateQueue instantiateQueue, int instantiatePriority)
		{
			this.cache = cache;
			this.instantiateQueue = instantiateQueue;
			this.instantiatePriority = instantiatePriority;
		}

		~WaitSpawnGameObject()
		{
			cache.Loading = false;
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
				if (!cache.HasPrefab) return true;
				if (instantiateQueue != null)
				{
					if (!instantiating)
					{
						instantiating = true;
						cache.Spawn(instantiateQueue, instantiatePriority, go => instance = go);
					}
					if (instance == null) return true;
				}
				return false;
			}
		}
	}
}