using UnityEngine;

namespace Game
{
	public abstract class WaitLoadObject : WaitLoadAsset
	{
		public abstract Object GetObject();

		public T GetObject<T>() where T : Object
		{
			return GetObject() as T;
		}
	}
}