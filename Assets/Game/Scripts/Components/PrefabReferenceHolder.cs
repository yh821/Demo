using UnityEngine;

namespace Game
{
	public class PrefabReferenceHolder : MonoBehaviour
	{
		private PrefabReference prefabRef;

		public void SetPrefabReference(PrefabReference prefabRef)
		{
			this.prefabRef = prefabRef;
		}
	}
}