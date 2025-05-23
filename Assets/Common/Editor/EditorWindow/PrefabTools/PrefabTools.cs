using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Common.Editor
{
	public interface PageBase
	{
		PrefabTools win { get; }
		void OnGUI();
	}

	public class PrefabTools : EditorWindow
	{
		public const int SPACE = 10;
		public const int BTN_WITCH = 100;
		public const int SLT_WITCH = 160;
		public const int WIN_WIDTH = 1080;
		public const int TGO_WIDTH = 14;

		private static string[] TAB =
		{
			"查找图片",
			"查找字体",
			"查找文本",
			// "查找组件",
			// "查找错误",
			"GUID引用",
		};

		private const int tab_column = 4;

		[MenuItem("Tools/预制工具 %&s", false, 900)]
		public static PrefabTools OpenWindow()
		{
			var winWidth = Mathf.Min(tab_column * (BTN_WITCH + 3) + 3, WIN_WIDTH);
			_win = GetWindow<PrefabTools>("预制工具");
			_win.minSize = new Vector2(winWidth, 400);
			_win.maxSize = new Vector2(WIN_WIDTH, 980);
			_win.Init();
			return _win;
		}

		private static PrefabTools _win;

		public int Tab { get; set; }

		public const string PathListKey = "FindPrefabImage.PrefabPathList";
		public const string DefaultPath = "Assets/Game/UIs";
		private SerializedObject mPathSerializedObject;
		private ReorderableList mPathReorderableList;
		public List<Object> PrefabPathList = null;

		private Transform mTempParent = null;

		public Transform TempParent
		{
			get
			{
				if (mTempParent == null)
				{
					var go = GameObject.Find("GameRoot/BaseView/Root");
					if (go != null)
						mTempParent = go.transform;
				}

				return mTempParent;
			}
		}

		public FindPrefabFont findFont { get; private set; }
		public FindPrefabChar findChar { get; private set; }
		public FindPrefabImage findImage { get; private set; }
		public FindRefByGuid findRefByGuid { get; private set; }

		private void Init()
		{
			findFont = new FindPrefabFont(this);
			findChar = new FindPrefabChar(this);
			findImage = new FindPrefabImage(this);
			findRefByGuid = new FindRefByGuid(this);

			InitPrefabPaths();
		}

		public void SetFindImage(Sprite sprite)
		{
			findImage.InitFindImage(sprite);
		}

		private void OnLostFocus()
		{
			SavePrefabPaths();
		}

		public void InitPrefabPaths()
		{
			var pathStr = EditorPrefs.GetString(PathListKey, DefaultPath);
			PrefabPathList = new List<Object>();
			var paths = pathStr.Split('|');
			if (string.IsNullOrEmpty(paths[0]))
				paths[0] = DefaultPath;
			foreach (var path in paths)
			{
				var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
				PrefabPathList.Add(obj);
			}

			mPathSerializedObject = new SerializedObject(this);
			var property = mPathSerializedObject.FindProperty("PrefabPathList");
			mPathReorderableList = new ReorderableList(mPathSerializedObject, property)
			{
				drawHeaderCallback = rect => GUI.Label(rect, "搜索预制路径:"),
				elementHeight = EditorGUIUtility.singleLineHeight,
				drawElementCallback = (rect, index, selected, focused) =>
				{
					var element = property.GetArrayElementAtIndex(index);
					EditorGUI.ObjectField(rect, element, GUIContent.none);
				}
			};
		}

		public void SavePrefabPaths()
		{
			mPathSerializedObject.ApplyModifiedProperties();
			var paths = GetPrefabPaths();
			EditorPrefs.SetString(PathListKey, string.Join("|", paths));
		}

		public string[] GetPrefabPaths()
		{
			var paths = new List<string>();
			foreach (var obj in PrefabPathList)
			{
				var path = AssetDatabase.GetAssetPath(obj);
				if (string.IsNullOrEmpty(path))
					continue;
				paths.Add(path);
			}

			return paths.ToArray();
		}

		private void OnGUI()
		{
			var oldColor = GUI.color;
			for (int i = 0, len = TAB.Length; i < len; i += tab_column)
			{
				GUILayout.BeginHorizontal();
				for (int j = 0; j < tab_column; j++)
				{
					var index = i + j;
					if (index < len)
					{
						GUI.color = Tab == index ? Color.grey : Color.white;
						if (GUILayout.Button(TAB[index], GUILayout.MaxWidth(BTN_WITCH)))
							Tab = index;
					}
				}

				GUILayout.EndHorizontal();
			}

			GUI.color = oldColor;

			EditorGUIUtility.labelWidth = 64;
			mPathReorderableList?.DoLayoutList();

			switch (Tab)
			{
				case 0:
					findImage.OnGUI();
					break;
				case 1:
					findFont.OnGUI();
					break;
				case 2:
					findChar.OnGUI();
					break;
				case 3:
					findRefByGuid.OnGUI();
					break;
			}
		}


		public GameObject GetSceneObject(string name)
		{
			if (TempParent != null)
			{
				var t = TempParent.Find(name);
				if (t != null)
					return t.gameObject;
			}

			return GameObject.Find(name);
		}

		public static UnityEditor.SceneManagement.PrefabStage GetPrefabStage(GameObject prefab, string path)
		{
			var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
			if (stage == null || stage.prefabAssetPath != path)
			{
				AssetDatabase.OpenAsset(prefab);
				stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
			}

			return stage;
		}
	}
}