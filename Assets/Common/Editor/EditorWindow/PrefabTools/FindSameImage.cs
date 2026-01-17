using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Common.Editor
{
	public class FindSameImage : PageBase
	{
		public PrefabTools win { get; }

		public FindSameImage(PrefabTools window)
		{
			win = window;
		}

		private class ImageData
		{
			public string guid;
			public string path;
			public Texture2D tex;

			public ImageData(string guid, string path)
			{
				this.guid = guid;
				this.path = path;
				tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
			}
		}

		private class ImageRepeatData
		{
			public List<ImageData> imageList = new List<ImageData>();
		}

		private Dictionary<uint, ImageRepeatData> mImageMap = new Dictionary<uint, ImageRepeatData>();
		private Dictionary<uint, ImageRepeatData> mSearchMap = new Dictionary<uint, ImageRepeatData>();
		private uint mCurSelectMD5;
		private int mTotalCount;

		private Vector2 mScrollPos = Vector2.zero;
		private string mSearchStr = string.Empty;

		public void OnGUI()
		{
			if (GUILayout.Button("查找相同图片")) FindSameTexture();
			DrawOperate();
			DrawResult();
		}

		private void FindSameTexture()
		{
			var paths = win.GetPrefabPaths();
			if (paths.Length == 0 || string.IsNullOrEmpty(paths[0]))
			{
				win.ShowNotification(new GUIContent("请设置路径"));
				return;
			}

			mImageMap.Clear();

			var allTexture = AssetDatabase.FindAssets("t:texture", paths);
			for (int i = 0, len = allTexture.Length; i < len; i++)
			{
				var guid = allTexture[i];
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var md5 = MD5.GetMD5FromFile(path);
				if (!mImageMap.TryGetValue(md5, out var repeatData))
				{
					repeatData = new ImageRepeatData();
					mImageMap.Add(md5, repeatData);
				}
				repeatData.imageList.Add(new ImageData(guid, path));
				EditorUtility.DisplayProgressBar("查找相同图片...", path, (float) i / len);
			}
			EditorUtility.ClearProgressBar();
		}

		private void DrawOperate()
		{
			if (mImageMap.Count <= 0) return;
			EditorGUILayout.BeginHorizontal();
			{
				mSearchStr = EditorGUILayout.TextField("搜索路径:", mSearchStr);
				if (GUILayout.Button("开始搜索"))
				{
					FindInResult();
					mScrollPos = Vector2.zero;
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		private void FindInResult()
		{
			if (string.IsNullOrEmpty(mSearchStr)) return;
			mSearchMap.Clear();
			foreach (var kv in mImageMap)
			{
				var imageList = kv.Value.imageList;
				foreach (var data in imageList)
				{
					if (!data.path.Contains(mSearchStr)) continue;
					mSearchMap.Add(kv.Key, kv.Value);
					break;
				}
			}
		}

		private void DrawResult()
		{
			var imageMap = string.IsNullOrEmpty(mSearchStr) ? mImageMap : mSearchMap;
			if (imageMap.Count <= 0) return;

			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("重复总数量:" + mTotalCount);
			mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos);
			{
				var index = 0;
				mTotalCount = 0;
				foreach (var kv in imageMap)
				{
					if (kv.Value.imageList.Count <= 1) continue;
					mTotalCount += kv.Value.imageList.Count - 1;
					if (GUILayout.Button($"{index} {kv.Key}")) mCurSelectMD5 = kv.Key;
					if (mCurSelectMD5 == kv.Key)
					{
						EditorGUI.indentLevel++;
						foreach (var data in kv.Value.imageList)
						{
							EditorGUILayout.BeginHorizontal();
							GUI.color = Color.green;
							if (GUILayout.Button(data.path)) Selection.activeObject = data.tex;
							GUI.color = Color.white;
							if (GUILayout.Button("删除", GUILayout.MaxWidth(PrefabTools.BTN_WIDTH)))
							{
								if (IOHelper.DeleteFile(data.path))
								{
									kv.Value.imageList.Remove(data);
									AssetDatabase.Refresh();
								}
							}
							var height = 100;
							var width = Mathf.Clamp(data.tex.width * height / data.tex.height, 50, 300);
							EditorGUILayout.ObjectField(data.tex, typeof(Texture), false,
								GUILayout.Width(width), GUILayout.Height(height));
							EditorGUILayout.EndHorizontal();
						}
						EditorGUI.indentLevel--;
					}
					index++;
				}
			}
			EditorGUILayout.EndScrollView();
		}
	}
}