using System;
using System.Collections.Generic;
using LuaInterface;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	public class UI3DModel : MaskableGraphic, IOverriderOrder, IClippable
	{
		[SerializeField] private bool isSupportMask;

		private readonly List<MaterialQueueData> materialQueueDatas = new List<MaterialQueueData>();

		//UiScene的模型不需要修改SortingOrder
		private bool noOverrideOrder;
		private int overrideLayer = -1;

		private readonly Dictionary<GameObject, AnimatorCullingMode> animatorCullingModeDic =
			new Dictionary<GameObject, AnimatorCullingMode>();

		private ClearMaskGraphic clearMaskGraphic;

		private Canvas clearZDepthCanvas;
		private Vector4 curClipRect = Vector4.zero;
		private readonly HashSet<GameObjectAttach> gameObjectAttaches = new HashSet<GameObjectAttach>();
		private Canvas groupCanvas;
		private bool isGrey;
		private bool isSupportClip;

		private float lastCheckChildTime;
		private RectMask2D lastMask;
		private Vector3 virtualLightDir;

		private readonly Dictionary<Renderer, int> renderOriginalOrderDic = new Dictionary<Renderer, int>();
		private Transform rootCanvasTransform;

		private Action onEnableCallback;
		private Action onDisableCallback;

		private RectTransform mRectTransform;

		[NoToLua]
		public new RectTransform rectTransform =>
			mRectTransform ? mRectTransform : mRectTransform = GetComponent<RectTransform>();

		private bool isStart;

		protected override void Awake()
		{
			base.Awake();
			base.raycastTarget = false;
			if (virtualLightDir == Vector3.zero)
			{
				var lightDir = GameObject.Find("GameRoot/UI3DModelLight");
				if (lightDir != null)
					virtualLightDir = lightDir.transform.forward;
			}
		}

#if UNITY_EDITOR
		protected override void Reset() { base.Reset();
#else
		protected void Reset() {
#endif
			Clear();
			animatorCullingModeDic.Clear();
			ResetRootCanvas();
			ResetAllRenders();
			ResetAllGoAttach();
			SetReOrderRender();
		}

		private void Update()
		{
			if (Time.time - lastCheckChildTime < 0.1) return;
			if (gameObjectAttaches.Count > 0) CheckGoAttach();
			TryRemoveInvalidRenders();
			TryRemoveInvalidGoAttach();
		}


		protected override void Start()
		{
			base.Start();
			isStart = true;
			OnEnable();
		}

		protected override void OnEnable()
		{
			if (!isStart) return;
			base.OnEnable();
			Reset();
			TryAddClip();
			TryAddClearMaskGraphic();
			if (groupCanvas)
				OverrideOrderGroupMgr.Instance.SetGroupCanvasDirty(groupCanvas);
			onEnableCallback?.Invoke();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			Clear();
			TryRemoveClip();
			TryRemoveClearMaskGraphic();
			if (Application.isPlaying)
			{
				foreach (var kv in renderOriginalOrderDic)
					if (kv.Key)
						kv.Key.sortingOrder = kv.Value;
			}
			onDisableCallback?.Invoke();
		}

		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			if (Application.isPlaying)
			{
				ResetRootCanvas();
				SetReOrderRender();
				TryAddClip();

				TryRemoveClearMaskGraphic();
				TryAddClearMaskGraphic();
			}
		}

		[NoToLua]
		public override void RecalculateClipping() { }

		[NoToLua]
		public override void Cull(Rect clipRect, bool validRect) { }

		[NoToLua]
		public override void SetClipSoftness(Vector2 clipSoftness) { }

		[NoToLua]
		public override void SetClipRect(Rect clipRect, bool validRect)
		{
			base.SetClipRect(clipRect, validRect);
			if (!isSupportClip) return;
			if (!rootCanvasTransform) return;
			if (!canvas) return;

			var uiCamera = canvas.worldCamera;
			if (!uiCamera) return;

			var newClipRect = Vector4.zero;
			if (validRect)
			{
				var minWorldPos = rootCanvasTransform.TransformPoint(new Vector3(clipRect.x, clipRect.y, 0));
				var maxWorldPos = rootCanvasTransform.TransformPoint(new Vector3(clipRect.right, clipRect.bottom, 0));
				newClipRect = GetClipRect(minWorldPos, maxWorldPos);
			}
			if (newClipRect != curClipRect)
			{
				curClipRect = newClipRect;
				RefreshCloneMaterials();
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
		}

		public GameObject GetTarget()
		{
			return this != null ? gameObject : null;
		}

		public void SetOverrideOrder(int order, int orderInterval, int maxOrder, out int incOrder)
		{
			incOrder = 0;
			if (noOverrideOrder) return;
			var list = ListPool<RendererItem>.Get();
			var iter = renderOriginalOrderDic.GetEnumerator();
			while (iter.MoveNext()) list.Add(new RendererItem(iter.Current.Key, iter.Current.Value));
			iter.Dispose();
			if (list.Count <= 0)
			{
				ListPool<RendererItem>.Release(list);
				return;
			}

			list.Sort(CompareRenderer);

			var curOrder = list[0].order;
			var childMax = maxOrder - 1;
			order = Mathf.Min(childMax, order);
			foreach (var item in list)
			{
				if (item.render == null) continue;
				if (curOrder != item.order)
				{
					curOrder = item.order;
					order = Mathf.Min(childMax, order + 1);
					item.render.sortingOrder = order;
					incOrder++;
				}
				else
				{
					item.render.sortingOrder = order;
				}
			}

			ListPool<RendererItem>.Release(list);
			RefreshClearZDepthOrder();
		}

		private void RemoveMaterialData(Renderer render)
		{
			foreach (var mat in render.sharedMaterials)
			{
				var index = materialQueueDatas.FindIndex(a => a.material == mat);
				if (index >= 0) materialQueueDatas.RemoveAt(index);
			}
		}

		private void Clear()
		{
			TryRemoveInvalidRenders();
			TryRemoveInvalidGoAttach();
			foreach (var attach in gameObjectAttaches)
			{
				if (attach)
				{
					attach.attachLayer = -1;
					// var srpEffect = attach.GetComponentInChildren<SRPEffect>();
					// if (srpEffect) srpEffect.ResumeOptimizeInUI();
				}
			}
			gameObjectAttaches.Clear();
			var iter = renderOriginalOrderDic.GetEnumerator();
			while (iter.MoveNext())
			{
				var kv = iter.Current;
				var render = kv.Key;
				if (render)
				{
					render.sortingOrder = kv.Value;
					RevertLayer(render);
					RemoveMaterialData(render);
					MaterialMgr.Instance.ResumeMaterialsKeywordsAndRenderQueue(render);
				}
			}
			iter.Dispose();
			renderOriginalOrderDic.Clear();

			OverrideOrderGroupMgr.Instance.RemoveFromGroup(groupCanvas, this);
			groupCanvas = null;
			rootCanvasTransform = null;
			lastCheckChildTime = 0;
			isGrey = false;
			curClipRect = Vector4.zero;
		}

		public void SetNoOverrideOrder(bool noOverride)
		{
			noOverrideOrder = noOverride;
		}

		public void SetIsGrey(bool isGrey)
		{
			if (this.isGrey == isGrey) return;
			this.isGrey = isGrey;
			RefreshCloneMaterials();
		}

		public void SetIsSupportClip(bool isSupportClip)
		{
			if (this.isSupportClip == isSupportClip) return;
			if (isSupportClip) TryAddClip();
			else TryRemoveClip();
			this.isSupportClip = isSupportClip;
		}

		public void SetIsSupportMask(bool isSupportMask)
		{
			if (this.isSupportMask == isSupportMask) return;

			TryRemoveClearMaskGraphic();
			this.isSupportMask = isSupportMask;
			TryAddClearMaskGraphic();

			SetMaterialDirty();
		}


		private void TryAddClearMaskGraphic()
		{
			if (!isSupportMask || !Application.isPlaying) return;
			var mask = transform.GetComponentInParent<Mask>();
			if (mask != null && mask.IsActive())
			{
				var newClearMaskGraphic = mask.GetComponentInChildren<ClearMaskGraphic>(true);
				if (clearMaskGraphic != newClearMaskGraphic)
				{
					Destroy(clearMaskGraphic.gameObject);
					clearMaskGraphic = null;
				}
				if (newClearMaskGraphic == null)
				{
					var obj = new GameObject("ClearMask");
					obj.transform.SetParent(mask.transform);
					clearMaskGraphic = obj.AddComponent<ClearMaskGraphic>();
					obj.AddComponent<UIOverrideOrder>();

					//重新计算Mask
					mask.enabled = false;
					mask.enabled = true;
				}
			}
		}

		private void TryRemoveClearMaskGraphic()
		{
			if (!isSupportMask || !Application.isPlaying || clearMaskGraphic == null ||
			    clearMaskGraphic.transform.parent == null) return;

			var mask = clearMaskGraphic.transform.parent.GetComponentInParent<Mask>();
			if (mask != null && mask.IsActive())
			{
				Destroy(clearMaskGraphic.gameObject);
				clearMaskGraphic = null;
				//重新计算Mask
				mask.enabled = false;
				mask.enabled = true;
			}
		}

		public void SetOverrideLayer(int layer)
		{
			if (overrideLayer == layer) return;
			overrideLayer = layer;
			var renders = ListPool<Renderer>.Get();
			gameObject.GetComponentsInChildren(true, renders);
			foreach (var render in renders)
				SetUI3DLayer(render);
			ListPool<Renderer>.Release(renders);
		}

		public void OnAddGameObject(GameObject go)
		{
			if (go == null) return;
			RefreshRendersInGameObject(go);
			RefreshGoAttachInGameObject(go);
			SetReOrderRender();

			if (!animatorCullingModeDic.ContainsKey(go))
			{
				var animator = gameObject.GetComponentInChildren<Animator>();
				if (animator != null) animatorCullingModeDic.Add(go, animator.cullingMode);
			}

			SetAnimatorCullingMode(go, AnimatorCullingMode.AlwaysAnimate);

			// var srpEffect = go.GetComponent<SRPEffect>();
			// if (srpEffect) srpEffect.DisableOptimizeInUI();

			// var srpActor = go.GetComponent<SRPActor>();
			// if (srpActor) srpActor.ForceUseHighMesh = true;
		}

		public void OnRemoveGameObject(GameObject go)
		{
			if (go == null) return;
			OnGameObjectRemoved(go);
		}

		private void SetUI3DLayer(Renderer render)
		{
			if (!render) return;
			if (overrideLayer >= 0)
			{
				render.gameObject.layer = overrideLayer;
				return;
			}
			render.gameObject.layer = GameLayers.UI3D;
		}

		private void RevertLayer(Renderer render)
		{
			if (render != null) render.gameObject.layer = GameLayers.Default;
		}

		private void SetReOrderRender()
		{
			OverrideOrderGroupMgr.Instance.SetGroupCanvasDirty(groupCanvas);
		}

		private void ResetAllGoAttach()
		{
			renderOriginalOrderDic.Clear();
			RefreshRendersInGameObject(gameObject);
		}

		private void RefreshRendersInGameObject(GameObject go)
		{
			if (!go) return;
			var renders = ListPool<Renderer>.Get();
			go.GetComponentsInChildren(true, renders);
			foreach (var render in renders)
			{
				SetUI3DLayer(render);
				if (!renderOriginalOrderDic.ContainsKey(render))
					renderOriginalOrderDic.Add(render, render.sortingOrder);
				var materials = MaterialMgr.Instance.GetClonedMaterials(render);
				foreach (var material1 in materials)
				{
					if (!material1 || !material1.shader) continue;
					var index = materialQueueDatas.FindIndex(a => a.material == material1);
					if (index < 0)
						materialQueueDatas.Add(new MaterialQueueData(material1));
				}
			}
			ListPool<Renderer>.Release(renders);

			if (noOverrideOrder) return;

			for (int i = materialQueueDatas.Count - 1; i >= 0; i--)
			{
				var data = materialQueueDatas[i];
				if (!data.material || !data.material.shader)
					materialQueueDatas.RemoveAt(i);
			}
			materialQueueDatas.Sort((a, b) => a.queue.CompareTo(b.queue));

			var value = 0;
			var lastQueue = 2999;
			for (int i = 0; i < materialQueueDatas.Count; i++)
			{
				var data = materialQueueDatas[i];
				if (data.queue != value)
				{
					lastQueue++;
					value = data.queue;
				}
				materialQueueDatas[i].material.renderQueue = lastQueue;
			}

			RefreshCloneMaterials();
		}

		private void RefreshCloneMaterials()
		{
			for (int i = 0; i < materialQueueDatas.Count; i++)
			{
				RefreshCloneMaterial(materialQueueDatas[i].material);
			}
		}

		private void RefreshCloneMaterial(Material material1)
		{
			var useClip = isSupportClip && curClipRect != Vector4.zero;
			var postEffect = 0;
			if (useClip) postEffect |= 64;
			if (isGrey) postEffect |= 2;
			material1.SetFloat("_PostEffectType", postEffect);
			if (useClip) material1.SetVector("_ClipRect", curClipRect);
			if (postEffect == 0) material1.DisableKeyword(GameConst.ENABLE_POST_EFFECT);
			else material1.EnableKeyword(GameConst.ENABLE_POST_EFFECT);
		}

		private void ResetAllRenders()
		{
			gameObjectAttaches.Clear();
			RefreshGoAttachInGameObject(gameObject);
		}

		private void RefreshGoAttachInGameObject(GameObject go)
		{
			if (!go) return;
			var attaches = ListPool<GameObjectAttach>.Get();
			go.GetComponentsInChildren(true, attaches);
			foreach (var attach in attaches)
			{
				attach.attachLayer = overrideLayer;
				gameObjectAttaches.Add(attach);
			}
			ListPool<GameObjectAttach>.Release(attaches);
		}

		private void SetAnimatorCullingMode(GameObject go, AnimatorCullingMode mode)
		{
			if (go == null) return;
			var animator = go.GetComponentInChildren<Animator>();
			if (animator != null) animator.cullingMode = mode;
		}

		private void ResetRootCanvas()
		{
			OverrideOrderGroupMgr.Instance.RemoveFromGroup(groupCanvas, this);
			groupCanvas = null;
			rootCanvasTransform = null;

			var scalers = ListPool<CanvasScaler>.Get();
			GetComponentsInChildren(true, scalers);
			if (scalers.Count > 0)
			{
				var scaler = scalers[0];
				groupCanvas = OverrideOrderGroupMgr.Instance.AddToGroup(this);
				rootCanvasTransform = scaler.transform;
			}
			ListPool<CanvasScaler>.Release(scalers);
		}

		private void RefreshClearZDepthOrder()
		{
			if (groupCanvas == null) return;
			var clearZDepth = groupCanvas.transform.Find("UIClearZDepth");
			if (clearZDepth != null)
			{
				clearZDepth.gameObject.SetActive(true);
				clearZDepthCanvas = clearZDepth.GetComponent<Canvas>();
				if (clearZDepthCanvas != null)
				{
					clearZDepthCanvas.overrideSorting = true;
					clearZDepthCanvas.sortingOrder = groupCanvas.sortingOrder +
						OverrideOrderGroupMgr.Instance.GroupCanvasOrderInterval - 1;
				}
			}
		}

		private void CheckGoAttach()
		{
			lastCheckChildTime = Time.time;
			List<GameObjectAttach> attaches = null;
			foreach (var attach in gameObjectAttaches)
			{
				if (attaches == null)
					attaches = ListPool<GameObjectAttach>.Get();
				attaches.Add(attach);
			}
			if (attaches != null)
			{
				var iter = attaches.GetEnumerator();
				while (iter.MoveNext())
				{
					gameObjectAttaches.Remove(iter.Current);
					// var srpEffect = iter.Current.GetComponentInChildren<SRPEffect>();
					// if (srpEffect != null) srpEffect.DisableOptimizeInUI();
				}
				iter.Dispose();
				ListPool<GameObjectAttach>.Release(attaches);
				RefreshRendersInGameObject(gameObject);
				SetReOrderRender();
			}
		}

		private void TryRemoveInvalidRenders()
		{
			List<Renderer> delList = null;
			var ui3DModels = ListPool<UI3DModel>.Get();
			var iter = renderOriginalOrderDic.GetEnumerator();
			while (iter.MoveNext())
			{
				var render = iter.Current.Key;
				if (!render) continue;
				ui3DModels.Clear();
				render.GetComponentsInChildren(true, ui3DModels);
				if (ui3DModels.Count <= 0)
				{
					render.sortingOrder = iter.Current.Value;
					RevertLayer(render);
					RemoveMaterialData(render);
					MaterialMgr.Instance.ResumeMaterialsKeywordsAndRenderQueue(render);
				}
				else if (ui3DModels[0] != this)
				{
					if (delList == null) delList = ListPool<Renderer>.Get();
					delList.Add(render);
				}
			}
			iter.Dispose();
			ListPool<UI3DModel>.Release(ui3DModels);

			if (delList != null)
			{
				foreach (var del in delList)
					renderOriginalOrderDic.Remove(del);
				ListPool<Renderer>.Release(delList);
			}
		}

		private void TryRemoveInvalidGoAttach()
		{
			List<GameObjectAttach> delList = null;
			foreach (var attach in gameObjectAttaches)
			{
				if (!attach || attach.GetComponentInParent<UI3DModel>() != this)
				{
					if (delList == null) delList = ListPool<GameObjectAttach>.Get();
					delList.Add(attach);
				}
			}
			if (delList != null)
			{
				foreach (var del in delList)
					gameObjectAttaches.Remove(del);
				ListPool<GameObjectAttach>.Release(delList);
			}
		}

		private void OnGameObjectRemoved(GameObject go)
		{
			var renders = ListPool<Renderer>.Get();
			go.GetComponentsInChildren(true, renders);
			foreach (var render in renders)
			{
				if (renderOriginalOrderDic.TryGetValue(render, out var order))
				{
					renderOriginalOrderDic.Remove(render);
					render.sortingOrder = order;
					RevertLayer(render);
					RemoveMaterialData(render);
					MaterialMgr.Instance.ResumeMaterialsKeywordsAndRenderQueue(render);
				}
			}
			ListPool<Renderer>.Release(renders);

			var attaches = ListPool<GameObjectAttach>.Get();
			go.GetComponentsInChildren(true, attaches);
			foreach (var attach in attaches)
			{
				attach.attachLayer = -1;
				gameObjectAttaches.Remove(attach);
			}
			ListPool<GameObjectAttach>.Release(attaches);

			if (animatorCullingModeDic.TryGetValue(go, out var mode))
			{
				animatorCullingModeDic.Remove(go);
				SetAnimatorCullingMode(go, mode);
			}

			// var srpEffect = go.GetComponent<SRPEffect>();
			// if (srpEffect != null) srpEffect.DisableOptimizeInUI();

			// var srpActor = go.GetComponent<SRPActor>();
			// if (srpActor != null) srpActor.ForceUseHighMesh = false;
		}

		private void TryAddClip()
		{
			if (!isSupportClip) return;
			var mask = GetComponentInParent<RectMask2D>();
			if (lastMask != mask)
			{
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
		}

		private void TryRemoveClip()
		{
			if (!isSupportClip) return;
			if (lastMask != null)
			{
				lastMask.RemoveClippable(this);
				lastMask = null;
			}
			curClipRect = Vector4.zero;
		}

		private int CompareRenderer(RendererItem x, RendererItem y)
		{
			if (x.order > y.order) return 1;
			if (x.order < y.order) return -1;
			return 0;
		}

		public void SetOnEnableCallBack(Action action)
		{
			onEnableCallback = action;
		}

		public void SetOnDisableCallBack(Action action)
		{
			onDisableCallback = action;
		}

		public static Vector4 GetClipRect(Vector3 minWorldPos, Vector3 maxWorldPos)
		{
			return new Vector4(minWorldPos.x, minWorldPos.y, maxWorldPos.x, maxWorldPos.y);
		}

		private struct MaterialQueueData
		{
			public readonly Material material;
			public readonly int queue;

			public MaterialQueueData(Material material)
			{
				this.material = material;
				queue = material.renderQueue;
			}
		}

		private struct RendererItem
		{
			public readonly Renderer render;
			public readonly int order;

			public RendererItem(Renderer render, int order)
			{
				this.render = render;
				//ParticleSystem强制显示在最上层
				if (render != null && render.GetComponent<ParticleSystem>()) order += 60000;
				this.order = order;
			}
		}
	}
}