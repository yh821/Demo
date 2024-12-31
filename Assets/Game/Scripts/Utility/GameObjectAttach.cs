using System;
using UnityEditor;
using UnityEngine;

namespace Game
{
	[ExecuteInEditMode]
	public class GameObjectAttach : MonoBehaviour, IGameObjectAttach
	{
		public float delayTime = 0;
		public int attachLayer = -1;

		[SerializeField] private bool autoSyncLayer = false;
		private bool isDisableEffect = false;

		// private SRPEffect srpEffect;
		private int effectQualityBisa = 0;

		[SerializeField] private AssetID asset;

		public AssetID Asset
		{
			get => asset;
			set
			{
				if (!asset.Equals(value)) asset = value;
			}
		}

		public string BundleName
		{
			get => asset.BundleName;
			set => asset.BundleName = value;
		}

		public string AssetName
		{
			get => asset.AssetName;
			set => asset.AssetName = value;
		}

#if UNITY_EDITOR
		public string AssetGuid
		{
			get => asset.AssetGuid;
			set => asset.AssetGuid = value;
		}

		private GameObject previewGameObj;
#endif

		public bool Loaded { get; private set; }

		private void OnDestroy()
		{
			if (EventDispatcher.Instance != null)
				EventDispatcher.Instance.OnGameObjAttachDestroy(this);
		}

		private bool mDisable;
		private bool mIsUi;

		private void OnDisable()
		{
#if UNITY_EDITOR
			DestroyAttachObj();
#endif
			if (EventDispatcher.Instance != null)
				EventDispatcher.Instance.OnGameObjAttachDisable(this);

			Loaded = false;
			mDisable = true;
		}

		private void OnEnable()
		{
			UpdateAttachObj();
			mDisable = false;
		}

		private void UpdateAttachObj()
		{
#if UNITY_EDITOR
			// if (GameRoot.Instance != null &&
			//     UnityEditor.SceneManagement.EditorSceneManager.IsPreviewSceneObject(transform)) return;
			CreateAttachObj();
#endif
			if (isDisableEffect) return;
			mIsUi = transform.GetComponentInParent<Canvas>();
			//临时处理UI特效
			Scheduler.Delay(() =>
			{
				if (!this || !enabled || isDisableEffect || mDisable) return;
				if (EventDispatcher.Instance == null) return;
				if (gameObject.activeSelf) EventDispatcher.Instance.OnGameObjAttachEnable(this);
			}, delayTime);
		}

		public void OnLoadComplete(GameObject effect)
		{
			Loaded = true;
			if (autoSyncLayer && attachLayer < 0)
				attachLayer = gameObject.layer;
			if (attachLayer >= 0)
			{
				var renderers = ListPool<Renderer>.Get();
				gameObject.GetComponentsInChildren(true, renderers);
				foreach (var render in renderers)
					render.gameObject.layer = attachLayer;
				ListPool<Renderer>.Release(renderers);
			}
			attachLayer = -1;

			SetEffectQualityBias(effectQualityBisa);
		}

		private void SetEffectQualityBias(int bias)
		{
			effectQualityBisa = bias;
		}

		public void SetIsDisableEffect(bool disable)
		{
			isDisableEffect = disable;
			if (EventDispatcher.Instance != null)
			{
				if (disable) EventDispatcher.Instance.OnGameObjAttachDisable(this);
				else UpdateAttachObj();
			}
		}

		public void SetIsSceneOptimize(bool disable, int effectQualityBias = 0)
		{
			SetEffectQualityBias(effectQualityBias);
		}

		public bool IsSceneOptimize()
		{
			return isDisableEffect;
		}

#if UNITY_EDITOR

		private bool dirty = false;

		private void OnValidate()
		{
			dirty = true;
		}

		private void Update()
		{
			if (Application.isPlaying) return;
			if (dirty)
			{
				dirty = false;
				CreateAttachObj();
			}
		}

		private void DestroyAttachObj()
		{
			if (!Application.isPlaying)
			{
				var previewObj = gameObject.GetComponent<PreviewObject>();
				if (previewObj) previewObj.ClearPreview();
				if (previewGameObj != null)
				{
					Destroy(previewGameObj);
					previewGameObj = null;
				}
			}
		}

		private void CreateAttachObj()
		{
			DestroyAttachObj();
			var isPlayingEditMode = false;
#if UNITY_EDITOR
			// isPlayingEditMode = Application.isPlaying &&
			//                     UnityEditor.SceneManagement.EditorSceneManager.IsPreviewSceneObject(transform);
#endif
			if (GameRoot.Instance == null && !isPlayingEditMode)
			{
				if (!(string.IsNullOrEmpty(BundleName) || string.IsNullOrEmpty(AssetName)))
				{
					var asset = EditorResourceMgr.LoadGameObject(BundleName, AssetName);
					if (asset != null)
					{
						var go = Instantiate(asset);
						if (Application.isPlaying)
						{
							go.transform.SetParent(transform, false);
							previewGameObj = go;
						}
						else
						{
							var previewObj = gameObject.GetComponent<PreviewObject>() ??
							                 gameObject.AddComponent<PreviewObject>();
							previewObj.SimulateInEditMode = true;
							previewObj.SetPreview(go);
						}
					}
				}
			}
		}

		public void RefreshAssetBundleName()
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(AssetGuid);
			var importer = AssetImporter.GetAtPath(assetPath);
			if (importer != null)
			{
				BundleName = importer.assetBundleName;
				AssetName = assetPath.Substring(assetPath.LastIndexOf('/') + 1);
			}
		}

		public bool IsGameObjectMissing()
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(AssetGuid);
			var importer = AssetImporter.GetAtPath(assetPath);
			if (importer == null)
				return true;
			if (BundleName != importer.assetBundleName ||
			    AssetName != assetPath.Substring(assetPath.LastIndexOf('/') + 1))
				return true;
			return false;
		}
#endif
	}
}