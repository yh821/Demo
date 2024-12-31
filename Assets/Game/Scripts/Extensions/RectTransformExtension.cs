using UnityEngine;

public static class RectTransformExtension
{
	public static void SetSizeDelta(this RectTransform rect, float x, float y)
	{
		rect.sizeDelta = new Vector2(x, y);
	}

	public static void SetAnchoredPosition(this RectTransform rect, float x, float y)
	{
		rect.anchoredPosition = new Vector2(x, y);
	}

	public static void SetLocalScale(this RectTransform rect, float x, float y, float z)
	{
		rect.localScale = new Vector3(x, y, z);
	}

	public static void SetAnchorMin(this RectTransform rect, float x, float y)
	{
		rect.anchorMin = new Vector2(x, y);
	}

	public static void SetAnchorMax(this RectTransform rect, float x, float y)
	{
		rect.anchorMax = new Vector2(x, y);
	}

	public static void SetPivot(this RectTransform rect, float x, float y)
	{
		rect.pivot = new Vector2(x, y);
	}
}