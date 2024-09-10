using System.Linq;
using UnityEngine;

public static class GameObjectExtension
{
	public static Component GetOrAddComponent(this GameObject go, System.Type type)
	{
		var component = go.GetComponent(type);
		if (component == null)
			component = go.AddComponent(type);
		return component;
	}

	public static T GetOrAddComponent<T>(this GameObject go) where T : Component
	{
		var component = go.GetComponent<T>();
		if (component == null)
			component = go.AddComponent<T>();
		return component;
	}

	public static void SetLayerRecursively(this GameObject go, int layer)
	{
		var list = ListPool<Transform>.Get();
		list.Clear();
		go.GetComponentsInChildren(true, list);
		foreach (var transform in list.Where(transform => transform))
			transform.gameObject.layer = layer;
		ListPool<Transform>.Release(list);
	}
}