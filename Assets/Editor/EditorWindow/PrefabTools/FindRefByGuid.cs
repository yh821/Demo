using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Common.Editor
{
	public class FindRefByGuid : PageBase
	{
		public PrefabTools win { get; }

		public FindRefByGuid(PrefabTools window)
		{
			win = window;
		}

		private List<Object> mShowData;

		private Vector2 mScrollPos = Vector2.zero;
		private string mGuid;
		private string mLastGuid;
		private Object mObj;
		private Object mLastObj;
		private string mPath;
		private string mLastPath;
		private string mBundleName;

		public void OnGUI()
		{
			if (mGuid != mLastGuid)
			{
				mLastGuid = mGuid;
				mPath = AssetDatabase.GUIDToAssetPath(mGuid);
				mLastPath = mPath;
				mObj = AssetDatabase.LoadAssetAtPath<Object>(mPath);
				mLastObj = mObj;
				RefreshBundleName();
			}
			if (mObj != mLastObj)
			{
				mLastObj = mObj;
				mPath = AssetDatabase.GetAssetPath(mObj);
				mLastPath = mPath;
				mGuid = AssetDatabase.AssetPathToGUID(mPath);
				mLastGuid = mGuid;
				RefreshBundleName();
			}
			if (mPath != mLastPath)
			{
				mLastPath = mPath;
				mObj = AssetDatabase.LoadAssetAtPath<Object>(mPath);
				mLastObj = mObj;
				mGuid = AssetDatabase.AssetPathToGUID(mPath);
				mLastGuid = mGuid;
				RefreshBundleName();
			}

			mObj = EditorGUILayout.ObjectField(mObj, typeof(Object), false, GUILayout.Height(64));
			mGuid = EditorGUILayout.TextField("GUID:", mGuid);
			mPath = EditorGUILayout.TextField("PATH:", mPath);
			EditorGUILayout.TextField("BUNDLE:", mBundleName);

			GUILayout.Space(10);
			EditorGUILayout.BeginHorizontal();
			{
				GUI.color = Color.green;
				if (GUILayout.Button("搜索引用的资源"))
				{
					StartSearchAsset(mGuid);
				}
				GUI.color = Color.white;
				if (GUILayout.Button("刷新数据", GUILayout.Width(PrefabTools.BTN_WITCH)))
				{
					ReadAllGuidInAsset();
				}
				GUI.color = Color.grey;
				if (GUILayout.Button("删除数据", GUILayout.Width(PrefabTools.BTN_WITCH)))
				{
					AssetGuidData.Instance.Clear();
				}
				GUI.color = Color.white;
			}
			EditorGUILayout.EndHorizontal();
			if (mShowData == null) return;
			DrawObjectScroll(mShowData);
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				EditorGUILayout.LabelField($"资源总数:{mShowData.Count}");
			}
			EditorGUILayout.EndHorizontal();
		}

		private void RefreshBundleName()
		{
			var importer = AssetImporter.GetAtPath(mPath);
			mBundleName = importer != null ? importer.assetBundleName : "";
		}

		public void StartSearchAsset(string guid)
		{
			if (string.IsNullOrEmpty(guid))
			{
				win.ShowNotification(new GUIContent("GUID无效"));
				return;
			}
			mGuid = guid;
			mShowData = AssetGuidData.Instance.GetAssetList(guid);
		}

		private void DrawObjectScroll(List<Object> guidData)
		{
			mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos);
			foreach (var obj in guidData)
			{
				EditorGUILayout.ObjectField(obj, obj.GetType(), false);
			}
			EditorGUILayout.EndScrollView();
		}

		private void ReadAllGuidInAsset()
		{
			if (!EditorUtility.DisplayDialog("提示", "是否重新读取所有资源?(耗时较长)", "确定", "取消"))
				return;
			AssetGuidData.Instance.ReadAllAssets();
			AssetGuidData.Instance.Save();
		}

		[MenuItem("Assets/Tools/搜索被哪些资源引用 %w")]
		public static void SearchAssetRefByGuid()
		{
			var asset = Selection.activeObject;
			if (asset == null)
			{
				Debug.LogError("请选择对象");
				return;
			}
			var path = AssetDatabase.GetAssetPath(asset);
			var guid = AssetDatabase.AssetPathToGUID(path);
			var win = PrefabTools.OpenWindow();
			win.Tab = 3;
			win.findRefByGuid.StartSearchAsset(guid);
		}
	}
}