using System;
using LuaInterface;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	[RequireComponent(typeof(Canvas))]
	[RequireComponent(typeof(GraphicRaycaster))]
	public class UIOverrideOrder : MonoBehaviour, IOverriderOrder, IClippable
	{
		[SerializeField] private bool isSupportClip;

		private Canvas groupCanvas;
		private Canvas canvas;
		private int baseSortingOrder;
		private RectMask2D lastMask;

		private RectTransform mRectTransform;

		public RectTransform rectTransform =>
			mRectTransform ? mRectTransform : mRectTransform = GetComponent<RectTransform>();

		private void Awake()
		{
			canvas = GetComponent<Canvas>();
			if (canvas)
				baseSortingOrder = canvas.sortingOrder;
		}

		private void Start()
		{
			groupCanvas = OverrideOrderGroupMgr.Instance.AddToGroup(this);
		}

		private void OnEnable()
		{
			if (groupCanvas)
				OverrideOrderGroupMgr.Instance.SetGroupCanvasDirty(groupCanvas);
			TryAddClip();
		}

		private void OnDisable()
		{
			if (canvas && Application.isPlaying)
				canvas.sortingOrder = baseSortingOrder;
			TryRemoveClip();
		}

		private void OnDestroy()
		{
			OverrideOrderGroupMgr.Instance.RemoveFromGroup(groupCanvas, this);
			groupCanvas = null;
		}

		protected void OnTransformChildrenChanged()
		{
			if (Application.isPlaying)
			{
				ResetRootCanvas();
				OverrideOrderGroupMgr.Instance.SetGroupCanvasDirty(groupCanvas);
			}
			TryAddClip();
		}

		private void TryAddClip()
		{
			if (!isSupportClip) return;
			var mask = GetComponentInParent<RectMask2D>();
			if (lastMask == mask) return;
			if (lastMask != null)
			{
				lastMask.RemoveClippable(this);
				lastMask = null;
			}
			if (mask != null)
			{
				mask.AddClippable(this);
				lastMask = mask;
			}
		}

		private void TryRemoveClip()
		{
			if (!isSupportClip) return;
			SetClipRect(new Rect(), false);
			if (lastMask != null)
			{
				lastMask.RemoveClippable(this);
				lastMask = null;
			}
		}

		public GameObject GetTarget()
		{
			return this == null ? null : gameObject;
		}

		public void SetOverrideOrder(int order, int orderInterval, int maxOrder, out int incOrder)
		{
			incOrder = 0;
			if (canvas)
			{
				canvas.overrideSorting = true;
				if (order > maxOrder)
					order = maxOrder;
				canvas.sortingOrder = order;
				incOrder = 1;
			}
		}

		public void ResetRootCanvas()
		{
			OverrideOrderGroupMgr.Instance.RemoveFromGroup(groupCanvas, this);
			groupCanvas = null;
			var canvasScalers = ListPool<CanvasScaler>.Get();
			if (canvasScalers.Count > 0)
				groupCanvas = OverrideOrderGroupMgr.Instance.AddToGroup(this);
			ListPool<CanvasScaler>.Release(canvasScalers);
		}

		[NoToLua]
		public void RecalculateClipping() { }

		[NoToLua]
		public void Cull(Rect clipRect, bool validRect) { }

		public void SetClipRect(Rect value, bool validRect)
		{
			if (!isSupportClip) return;
			var graphics = GetComponentsInChildren<MaskableGraphic>();
			foreach (var graphic in graphics)
			{
				graphic.SetClipRect(value, validRect);
			}
		}

		[NoToLua]
		public void SetClipSoftness(Vector2 clipSoftness) { }

		// [NoToLua]
		// public void SetClipSoftnessMask(Vector2 clipSoftnessMask) { }
	}
}