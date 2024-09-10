using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public static class ReferenceDict
{
	private static WeakDictionary<int, SpriteReference> spriteDict = new WeakDictionary<int, SpriteReference>();
	private static WeakDictionary<int, PrefabReference> prefabDict = new WeakDictionary<int, PrefabReference>();
	private static List<GameObject> needReleasePrefab = new List<GameObject>();
	private static List<Sprite> needReleaseSprites = new List<Sprite>();
	private static float lastGcTime = 0;
	private static float gcInterval = 5f;

	public static SpriteReference AddSpriteReference(Image image, Sprite sprite)
	{
		var id = image.GetInstanceID();
		if (!spriteDict.TryGetValue(id, out var spriteRef))
		{
			spriteRef = new SpriteReference();
			spriteDict[id] = spriteRef;
		}
		spriteRef.SetSprite(sprite);
		return spriteRef;
	}

	public static PrefabReference AddPrefabReference(GameObject go, GameObject prefab)
	{
		var id = go.GetInstanceID();
		if (!prefabDict.TryGetValue(id, out var prefabRef))
		{
			prefabRef = new PrefabReference();
			prefabDict[id] = prefabRef;
		}
		prefabRef.SetPrefab(prefab);
		return prefabRef;
	}

	public static void ReleaseSprite(Sprite sprite)
	{
		lock (needReleaseSprites)
			needReleaseSprites.Add(sprite);
	}

	public static void ReleasePrefab(GameObject prefab)
	{
		lock (needReleasePrefab)
			needReleasePrefab.Add(prefab);
	}

	public static void Update()
	{
		lock (needReleaseSprites)
		{
			foreach (var sprite in needReleaseSprites)
				Singleton<SpritePool>.Instance.Free(sprite);
			needReleaseSprites.Clear();
		}
		lock (needReleasePrefab)
		{
			foreach (var prefab in needReleasePrefab)
				Singleton<PrefabPool>.Instance.Free(prefab);
			needReleasePrefab.Clear();
		}
		var time = Time.time;
		if (time - lastGcTime < gcInterval) return;
		lastGcTime = time;
		spriteDict.GC();
		prefabDict.GC();
	}
}