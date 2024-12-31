using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	[RequireComponent(typeof(RectTransform))]
	public class SafeAreaAdapter : MonoBehaviour
	{
		public static Rect ReferenceSafeArea => referenceSafeArea;
		private static Rect referenceSafeArea;

		private RectTransform rectTransform;
		private bool hasSafeArea = false;
		private Action hasSafeAreaCallBack;

		private Vector2 referenceResolution; //设计分辨率
		private float lastCheckSafeAreaTime = 0;
		private Rect SafeArea;
		private Rect LastSafeArea;
		private int resolutionWidth;
		private int resolutionHeight;

#if UNITY_EDITOR || UNITY_STANDALONE
		private const float SafeAreaSize = 64;
		private const float Rotio = SafeAreaSize / GameConst.ReferenceWidth;
#endif

		private void Awake()
		{
			rectTransform = GetComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;

			if (referenceResolution == Vector2.zero)
			{
				var scaler = GetComponentInParent<CanvasScaler>();
				if (scaler != null) referenceResolution = scaler.referenceResolution;
			}
			AdjustLayout();
		}

		private void Update()
		{
			TryGetSafeAreaInfo();
			if (CheckSafeAreaChange())
			{
				AdjustLayout();
				hasSafeArea = true;
				hasSafeAreaCallBack?.Invoke();
			}
		}

		private bool CheckSafeAreaChange()
		{
			if (rectTransform.offsetMin.x < 0) return false;
			if (resolutionWidth <= 0 || resolutionHeight <= 0) return false;
			if (SafeArea == LastSafeArea) return false;

			LastSafeArea = SafeArea;

			//设计分辨率/实际分辨率
			var factorX = referenceResolution.x / resolutionWidth;
			var factorY = referenceResolution.y / resolutionHeight;

			//设计安全区 = 实际安全区 * 设实比
			referenceSafeArea.x = Mathf.Floor(SafeArea.x * factorX);
			referenceSafeArea.y = Mathf.Floor(SafeArea.y * factorY);
			referenceSafeArea.width = Mathf.Floor(SafeArea.width * factorX);
			referenceSafeArea.height = Mathf.Floor(SafeArea.height * factorY);

			if (rectTransform.offsetMin.x == referenceSafeArea.xMin
			    && rectTransform.offsetMin.y == referenceSafeArea.yMin
			    && rectTransform.offsetMax.x == referenceSafeArea.xMax - referenceResolution.x
			    && rectTransform.offsetMax.y == referenceSafeArea.yMax - referenceResolution.y)
			{
				return false;
			}

			return true;
		}

		private void TryGetSafeAreaInfo()
		{
			if (lastCheckSafeAreaTime == 0 || lastCheckSafeAreaTime + 1 <= Time.time)
			{
				lastCheckSafeAreaTime = Time.time;

#if UNITY_EDITOR || UNITY_STANDALONE
				// var type = (AspectRatioController.DeviceType) PlayerPrefs.GetInt(AspectRatioController.DeviceKey, 0);
				// switch (type)
				// {
				// 	case AspectRatioController.DeviceType.iPhoneL:
				// 		break;
				// 	case AspectRatioController.DeviceType.iPhoneR:
				// 		break;
				// 	default:
				SafeArea = new Rect(0, 0, Screen.width, Screen.height);
				// 		break;
				// }
				resolutionWidth = Screen.width;
				resolutionHeight = Screen.height;
#else
				DeviceTool.GetScreenSafeArea(out SafeArea, out resolutionWidth, out resolutionHeight);
				resolutionWidth = resolutionWidth == 0 ? Screen.width : resolutionWidth;
				resolutionHeight = resolutionHeight == 0 ? Screen.height : resolutionHeight;
#endif
			}
		}

		private void AdjustLayout()
		{
			if (referenceSafeArea.width == 0 || referenceSafeArea.height == 0) return;
			rectTransform.offsetMin = new Vector2(referenceSafeArea.x, referenceSafeArea.y);
			rectTransform.offsetMax = new Vector2(referenceSafeArea.xMax - referenceResolution.x,
				referenceSafeArea.yMax - referenceResolution.y);
		}

		public void SetSafeAreaChangeCallBack(Action action)
		{
			hasSafeAreaCallBack = action;
			if (hasSafeArea)
				hasSafeAreaCallBack?.Invoke();
		}

		public static SafeAreaAdapter Bind(GameObject go)
		{
			return go.GetOrAddComponent<SafeAreaAdapter>();
		}
	}
}