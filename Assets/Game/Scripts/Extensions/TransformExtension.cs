using UnityEngine;

public static class TransformExtension
{
	public static void SetPosition(this Transform transform, float x, float y, float z)
	{
		transform.position = new Vector3(x, y, z);
	}

	public static void SetLocalPosition(this Transform transform, float x, float y, float z)
	{
		transform.localPosition = new Vector3(x, y, z);
	}

	public static void SetLocalScale(this Transform transform, float x, float y, float z)
	{
		transform.localScale = new Vector3(x, y, z);
	}
}