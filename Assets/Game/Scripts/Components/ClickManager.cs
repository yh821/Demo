using HedgehogTeam.EasyTouch;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
	[RequireComponent(typeof(Camera))]
	public sealed class ClickManager : MonoBehaviour
	{
		private static RaycastHit[] clickableHits = new RaycastHit[20];
		private static RaycastHit[] sceneHits = new RaycastHit[20];

		private Rect reserveRect;
		private Camera lookCamera;

		public delegate void ClickGroundDelegate(RaycastHit hit);

		private ClickGroundDelegate clickGroundEvent;

		public static ClickManager Instance { get; private set; }

		public ClickGroundDelegate AddClickGroundListener(ClickGroundDelegate clickDelegate)
		{
			return clickGroundEvent += clickDelegate;
		}

		public void RemoveClickGroundListener(ClickGroundDelegate clickDelegate)
		{
			if (clickGroundEvent != null) clickGroundEvent -= clickDelegate;
		}

		public void SetReserveRect(RectTransform rectTransform)
		{
			reserveRect = Rect.MinMaxRect(rectTransform.offsetMin.x, rectTransform.offsetMin.y,
				rectTransform.offsetMax.x, rectTransform.offsetMax.y);
		}

		private void Awake()
		{
			Assert.IsNull(Instance);
			Instance = this;
			lookCamera = GetComponent<Camera>();
			EasyTouch.On_SimpleTap += HandleOnTap;
		}

		private void OnDestroy()
		{
			Instance = null;
		}

		private void HandleOnTap(Gesture gesture)
		{
			if (lookCamera == null) return;
			if (reserveRect.Contains(gesture.position)) return;
			var ray = lookCamera.ScreenPointToRay(new Vector3(gesture.position.x, gesture.position.y, 0));
			//部分机器出现transform上的collider不刷新的情况，这里强制执行一次刷新
			Physics.SyncTransforms();

			var count = Physics.RaycastNonAlloc(ray, clickableHits, Mathf.Infinity, 1 << GameLayers.Clickable);
			if (ProcessClickableHit(ray, count)) return;
			count = Physics.RaycastNonAlloc(ray, sceneHits, Mathf.Infinity, 1 << GameLayers.Walkable);
			if (ProcessSceneHit(ray, count)) return;
		}

		private bool ProcessClickableHit(Ray ray, in int count)
		{
			var distance = float.PositiveInfinity;
			ClickableObject owner = null;
			for (int i = 0; i < count; i++)
			{
				var hit = clickableHits[i];
				if (hit.distance >= distance) continue;
				var clickable = hit.collider.GetComponent<Clickable>();
				if (clickable == null) continue;
				var target = clickable.Owner;
				if (target == null) continue;
				owner = target;
				distance = hit.distance;
			}
			if (owner != null)
			{
				owner.TriggerClick();
				return true;
			}
			return false;
		}

		private bool ProcessSceneHit(Ray ray, in int count)
		{
			var distance = float.PositiveInfinity;
			var hasFind = false;
			var hitIndex = 0;
			for (int i = 0; i < count; i++)
			{
				var hit = sceneHits[i];
				if (hit.distance >= distance) continue;
				hitIndex = i;
				hasFind = true;
				distance = hit.distance;
			}
			if (hasFind && clickGroundEvent != null && hitIndex < sceneHits.Length)
			{
				clickGroundEvent(sceneHits[hitIndex]);
				return true;
			}
			return false;
		}
	}
}