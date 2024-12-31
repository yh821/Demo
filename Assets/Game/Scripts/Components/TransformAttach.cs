using UnityEngine;

namespace Game
{
	public class TransformAttach : MonoBehaviour
	{
		public Transform target;
		public Vector3 offset;
		public Vector3 rotation;

		private void LateUpdate()
		{
			if (target == null) return;
			var rot = Quaternion.Euler(rotation);
			transform.rotation = target.rotation * rot;
			transform.position = target.position + target.rotation * offset;
		}
	}
}