using System.IO;
using UnityEditor;
using UnityEngine;

namespace Common
{
	public static class EditorHelper
	{
		#region 实例化预设

		public const string UGUI_ROOT_NAME = "GameRoot/BaseView/Root";

		[MenuItem("Tools/实例化预设 #&s")]
		public static void InstantiatePrefab()
		{
			var select = Selection.activeObject;
			if (select != null && PrefabUtility.GetPrefabType(select) == PrefabType.Prefab)
			{
				var target = GetSceneObject(select.name);
				if (target == null)
				{
					target = PrefabUtility.InstantiatePrefab(select) as GameObject;
					var isHaveCanvas = target.GetComponentInChildren<UnityEngine.UI.CanvasScaler>() != null;
					if (!isHaveCanvas && TempParent != null)
						target.transform.SetParent(TempParent, false);
					target.name = select.name;
					Selection.activeObject = target;
				}
			}
		}

		private static Transform TempParent
		{
			get
			{
				if (mTempParent == null)
				{
					var go = GameObject.Find(UGUI_ROOT_NAME);
					if (go != null)
						mTempParent = go.transform;
				}

				return mTempParent;
			}
		}

		private static Transform mTempParent = null;

		private static GameObject GetSceneObject(string name)
		{
			if (TempParent != null)
			{
				var t = TempParent.Find(name);
				if (t != null)
					return t.gameObject;
			}

			return GameObject.Find(name);
		}

		#endregion

		#region 复制预制ab名

		[MenuItem("Assets/复制AssetBundleName")]
		public static void CopyAssetBundleName()
		{
			var path = AssetDatabase.GetAssetPath(Selection.activeObject);
			var importer = AssetImporter.GetAtPath(path);
			if (!importer || string.IsNullOrEmpty(importer.assetBundleName)) return;
			GUIUtility.systemCopyBuffer = importer.assetBundleName;
		}

		[MenuItem("Assets/复制AssetPath")]
		public static void CopyAssetPath()
		{
			var path = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (string.IsNullOrEmpty(path)) return;
			GUIUtility.systemCopyBuffer = path;
		}

		[MenuItem("Assets/复制GUID")]
		public static void CopyAssetGuid()
		{
			var path = AssetDatabase.GetAssetPath(Selection.activeObject);
			var guid = AssetDatabase.GUIDFromAssetPath(path);
			if (guid.Empty()) return;
			GUIUtility.systemCopyBuffer = guid.ToString();
		}

		#endregion

		#region 删除空文件夹

		[MenuItem("Assets/删除空文件夹")]
		public static void DeleteEmptyFolder()
		{
			var obj = Selection.activeObject;
			if (!obj) return;
			var folder = AssetDatabase.GetAssetPath(obj);
			if (!Directory.Exists(folder)) return;
			foreach (var path in Directory.GetFileSystemEntries(folder, "*", SearchOption.AllDirectories))
			{
				if (!Directory.Exists(path)) continue;
				var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
				if (files.Length == 0)
				{
					Directory.Delete(path, true);
					File.Delete(path + ".meta");
				}
			}
			AssetDatabase.Refresh();
		}

		#endregion

		#region 批量重命名节点

		[MenuItem("GameObject/Game Tool/批量重命名子节点(父节点名+index升序)")]
		public static void StartAsceRenameNode()
		{
			var go = Selection.activeGameObject;
			if (!go) return;
			var prefix = go.name;
			var count = go.transform.childCount;
			for (int i = 0; i < count; i++)
			{
				var child = go.transform.GetChild(i);
				if (child != null) child.gameObject.name = prefix + i;
			}
			EditorUtility.SetDirty(go);
		}


		[MenuItem("GameObject/Game Tool/批量重命名子节点(父节点名+index降序)")]
		public static void StartDescRenameNode()
		{
			var go = Selection.activeGameObject;
			if (!go) return;
			var prefix = go.name;
			var count = go.transform.childCount;
			for (int i = 0; i < count; i++)
			{
				var child = go.transform.GetChild(i);
				if (child != null) child.gameObject.name = prefix + (count - 1 - i);
			}
			EditorUtility.SetDirty(go);
		}

		#endregion
	}
}