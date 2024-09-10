using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class UIEffect : MaskableGraphic, IOverriderOrder
{
	private static readonly ObjectPool<CullStateChangedEvent> CullStateChangeEventPool =
		new ObjectPool<CullStateChangedEvent>(l => l.RemoveAllListeners());

	public bool isIgnoreTimeScale;
	public bool isOffZTest;
	public int orderOffset;

	private readonly List<CanvasGroup> canvasGroupsList = new List<CanvasGroup>();
	private readonly Dictionary<Renderer, int> renderDic = new Dictionary<Renderer, int>();
	private float canvasGroupAlpha = 1f;
	private float deltaTime;
	private Canvas groupCanvas;
	private bool isCliped;
	private bool start;

	private List<Material> materialList;
	private ParticleSystem[] particleSystems;
	private Camera rootCamera;
	private Transform rootCanvasTransform;
	private List<RendererItem> sortOrderList;
	private float timeAtLastFrame;

	protected override void Awake()
	{
		base.Awake();
		var renderers = ListPool<Renderer>.Get();
		GetComponentsInChildren(true, renderers);
		foreach (var render in renderers)
		{
			SetUILayer(render);
			renderDic.Add(render, render.sortingOrder);
		}
		ListPool<Renderer>.Release(renderers);
	}

#if UNITY_EDITOR
	protected override void Reset()
	{
		base.Reset();
		raycastTarget = false;
	}
#endif

	private void Update()
	{
		UpdateEffectInTimeScaleZero();
	}

	protected override void Start()
	{
		base.Start();
		onCullStateChanged ??= CullStateChangeEventPool.Get();
		onCullStateChanged.AddListener(CullChanged);
		if (Application.isPlaying)
			ResetRootCanvas();
		ResetRootCamera();
		//延迟第一次OnEnable到Start后
		//OnEnable会访问其他实例，避免时序问题
		start = true;
		OnEnable();
	}

	protected override void OnEnable()
	{
		if (!start) return;
		base.OnEnable();
		ResetCanvasGroupList();
		UpdateAlpha();
		TrySetZTestOff();
		if (groupCanvas)
			OverrideOrderGroupMgr.Instance.SetGroupCanvasDirty(groupCanvas);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (Application.isPlaying)
		{
			foreach (var dic in renderDic)
			{
				if (dic.Key)
					dic.Key.sortingOrder = dic.Value;
			}
		}
		ResumeClipState();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Application.isPlaying)
		{
			OverrideOrderGroupMgr.Instance.RemoveFromGroup(groupCanvas, this);
			groupCanvas = null;
			rootCanvasTransform = null;
		}
		if (onCullStateChanged != null)
		{
			CullStateChangeEventPool.Release(onCullStateChanged);
			onCullStateChanged = null;
		}
		canvasGroupsList.Clear();
	}

	protected override void OnCanvasGroupChanged()
	{
		base.OnCanvasGroupChanged();
		UpdateAlpha();
	}

	protected override void OnTransformParentChanged()
	{
		base.OnTransformParentChanged();
		if (Application.isPlaying)
		{
			ResumeClipState();
			ResetRootCanvas();
			OverrideOrderGroupMgr.Instance.SetGroupCanvasDirty(groupCanvas);
			ResetCanvasGroupList();
		}
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
	}

	public override void SetClipRect(Rect clipRect, bool validRect)
	{
		if (!rootCamera) return;
		if (rootCanvasTransform == null) return;
		base.SetClipRect(clipRect, validRect);

		if (validRect)
		{
			isCliped = true;
			var materials = GetCloneMaterialList();
			var minWorldPos = rootCanvasTransform.TransformPoint(new Vector2(clipRect.x, clipRect.y));
			var maxWorldPos = rootCanvasTransform.TransformPoint(new Vector2(x: clipRect.right, y: clipRect.bottom));
			var clipVector = UI3DModel.GetClipRect(minWorldPos, maxWorldPos);
			foreach (var material1 in materials)
			{
				if (material1)
				{
					material1.EnableKeyword(GameConst.ENABLE_POST_EFFECT);
					material1.SetFloat("_PostEffectType", 64);
					material1.SetVector("_ClipRect", clipVector);
				}
			}
		}
		else ResumeClipState();
	}

	GameObject IOverriderOrder.GetTarget()
	{
		return GetTarget();
	}

	public GameObject GetTarget()
	{
		return this != null ? gameObject : null;
	}

	public void SetOverrideOrder(int order, int orderInterval, int maxOrder, out int incOrder)
	{
		incOrder = 0;
		if (sortOrderList == null)
		{
			sortOrderList = new List<RendererItem>(renderDic.Count);
			var iter = renderDic.GetEnumerator();
			while (iter.MoveNext())
			{
				sortOrderList.Add(new RendererItem(iter.Current.Key, iter.Current.Value));
				sortOrderList.Sort(CompareRenderer);
			}
			iter.Dispose();
		}

		if (sortOrderList.Count <= 0) return;

		var curOrder = sortOrderList[0].order;
		var childMax = maxOrder - 1;
		order = Mathf.Min(childMax, order);
		foreach (var item in sortOrderList)
		{
			if (item.render == null) continue;
			if (curOrder != item.order)
			{
				curOrder = item.order;
				order = Mathf.Min(childMax, order + 1);
				item.render.sortingOrder = order + orderOffset;
				incOrder++;
			}
			else
			{
				item.render.sortingOrder = order + orderOffset;
			}
		}
	}

	private void UpdateEffectInTimeScaleZero()
	{
		if (!isIgnoreTimeScale) return;
		if (particleSystems == null) particleSystems = GetComponentsInChildren<ParticleSystem>(true);

		deltaTime = Time.realtimeSinceStartup - timeAtLastFrame;
		timeAtLastFrame = Time.realtimeSinceStartup;
		if (Mathf.Abs(Time.timeScale) < 1e-6)
		{
			foreach (var system in particleSystems)
			{
				system.Simulate(deltaTime, false, false);
				system.Play();
			}
		}
	}

	private void SetUILayer(Renderer render)
	{
		if (render != null) render.gameObject.layer = GameLayers.UIEffect;
	}

	private List<Material> GetCloneMaterialList()
	{
		if (materialList == null) materialList = new List<Material>();
		if (materialList.Count <= 0)
		{
			var iter = renderDic.GetEnumerator();
			while (iter.MoveNext())
			{
				var mat = MaterialMgr.Instance.GetClonedMaterial(iter.Current.Key);
				materialList.Add(mat);
			}
			iter.Dispose();
		}
		return materialList;
	}

	private void ResumeClipState()
	{
		if (!isCliped) return;
		isCliped = false;
		if (materialList == null) return;
		foreach (var material1 in materialList)
		{
			if (material1) material1.DisableKeyword(GameConst.ENABLE_POST_EFFECT);
		}
	}

	private void ResetCanvasGroupList()
	{
		canvasGroupsList.Clear();
		GetComponentsInParent(false, canvasGroupsList);
	}

	private void ResetRootCanvas()
	{
		OverrideOrderGroupMgr.Instance.RemoveFromGroup(groupCanvas, this);
		groupCanvas = null;
		rootCanvasTransform = null;

		var canvasScalers = ListPool<CanvasScaler>.Get();
		GetComponentsInParent(true, canvasScalers);
		if (canvasScalers.Count > 0)
		{
			var canvasScaler = canvasScalers[0];
			groupCanvas = OverrideOrderGroupMgr.Instance.AddToGroup(this);
			rootCanvasTransform = canvasScaler.transform;
		}
		ListPool<CanvasScaler>.Release(canvasScalers);
	}

	private void ResetRootCamera()
	{
		var canvasList = ListPool<Canvas>.Get();
		GetComponentsInParent(true, canvasList);
		if (canvasList.Count > 0)
		{
			var rootCanvas = canvasList[0];
			rootCamera = rootCanvas.worldCamera;
		}
		else rootCamera = null;
		ListPool<Canvas>.Release(canvasList);
	}

	private void UpdateAlpha()
	{
		var alpha = 1f;
		foreach (var canvasGroup in canvasGroupsList)
			if (canvasGroup != null)
				alpha *= canvasGroup.alpha;
		if (canvasGroupAlpha != alpha)
		{
			canvasGroupAlpha = alpha;
			materialList ??= GetCloneMaterialList();
			foreach (var material1 in materialList)
			{
				if (material1) material1.SetFloat("_AlphaMultiplier", canvasGroupAlpha);
			}
		}
	}

	private void TrySetZTestOff()
	{
		if (!isOffZTest) return;
		materialList ??= GetCloneMaterialList();
		foreach (var material1 in materialList)
		{
			if (material1) material1.SetFloat("_ZTest", (float) ZTestMode.Always);
		}
	}

	private void CullChanged(bool cull)
	{
		var list = ListPool<ParticleSystem>.Get();
		GetComponentsInChildren(list);
		foreach (var system in list)
		{
			system.GetComponent<Renderer>().enabled = !cull;
		}
		ListPool<ParticleSystem>.Release(list);

		var iter = renderDic.GetEnumerator();
		while (iter.MoveNext()) iter.Current.Key.enabled = !cull;
		iter.Dispose();
	}

	private int CompareRenderer(RendererItem x, RendererItem y)
	{
		if (x.order > y.order) return 1;
		if (x.order < y.order) return -1;
		return 0;
	}

	private enum ZTestMode
	{
		Disabled,
		Never,
		Less,
		Equal,
		LessEqual,
		Greater,
		NotEqual,
		GreaterEqual,
		Always
	}

	private struct RendererItem
	{
		public readonly Renderer render;
		public readonly int order;

		public RendererItem(Renderer render, int order)
		{
			this.render = render;
			order += 6000;
			this.order = order;
		}
	}
}