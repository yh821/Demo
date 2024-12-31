using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	[RequireComponent(typeof(RawImage))]
	[ExecuteInEditMode]
	public class LoadRawImage : MonoBehaviour
	{
		public string BundleName;
		public string AssetName;

		public bool AutoFitNatvieSize;
		public bool AutoUpdateAspectRatio;

		private RawImage rawImage;

#if UNITY_EDITOR
		private bool isDirty = false;
#endif

		private void Awake()
		{
			rawImage = gameObject.GetComponent<RawImage>();
			rawImage.enabled = false;
		}

		private void OnEnable()
		{
			if (EventDispatcher.Instance != null)
				EventDispatcher.Instance.OnLoadRawImageEnable(this);
#if UNITY_EDITOR
			else
				UpdateAsset();
#endif
		}

		private void OnDisable()
		{
			if (EventDispatcher.Instance != null)
				EventDispatcher.Instance.OnLoadRawImageDisable(this);
			else
				rawImage.texture = null;
		}

		private void OnDestroy()
		{
			if (EventDispatcher.Instance != null)
				EventDispatcher.Instance.OnLoadRawImageDestroy(this);
			else
				rawImage.texture = null;
		}

		private void SetTexture(Texture2D texture)
		{
			DestroyRawImageTexture();
			rawImage.texture = texture;
			rawImage.enabled = true;

			if (AutoFitNatvieSize)
				rawImage.SetNativeSize();

			if (AutoUpdateAspectRatio)
			{
				var ratioFitter = rawImage.GetComponent<AspectRatioFitter>();
				if (ratioFitter != null)
					ratioFitter.aspectRatio = texture.width / (float) texture.height;
			}
		}

		private void DestroyRawImageTexture()
		{
			rawImage.texture = null;
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (rawImage != null && rawImage.texture != null)
				rawImage.texture = null;
			isDirty = true;
		}

		private void Update()
		{
			if (isDirty)
			{
				isDirty = false;
				UpdateAsset();
			}
		}

		private void UpdateAsset()
		{
			if (string.IsNullOrEmpty(BundleName) || string.IsNullOrEmpty(AssetName))
				return;
			var texture = EditorResourceMgr.LoadObject(BundleName, AssetName, typeof(Texture2D)) as Texture2D;
			if (texture == null)
				DestroyRawImageTexture();
			else
			{
				var cloneTexture = new Texture2D(texture.width, texture.height, texture.format, false);
				cloneTexture.LoadRawTextureData(texture.GetRawTextureData());
				cloneTexture.Apply();
				SetTexture(cloneTexture);
			}
		}
#endif
	}
}