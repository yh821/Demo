using UnityEngine;

namespace Game
{
	public sealed class PoolStrategy : MonoBehaviour
	{
		[SerializeField] [Header("Prefab cache strategy.")]
		private float prefabReleaseAfterFree = 30f;

		[SerializeField] [Header("Instance pool strategy.")]
		private float instanceReleaseAfterFree = 60f;

		[SerializeField] [Header("Instance pool strategy.")]
		private int instancePoolCount = 5;

		public float PrefabReleaseAfterFree => prefabReleaseAfterFree;

		public float InstanceReleaseAfterFree => instanceReleaseAfterFree;

		public int InstancePoolCount => instancePoolCount;
	}
}