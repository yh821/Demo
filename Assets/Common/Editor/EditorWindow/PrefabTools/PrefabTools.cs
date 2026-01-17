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
		public const int BTN_WIDTH = 100;
		public const int WIN_WIDTH = 1600;
		public const int TGO_WIDTH = 14;

		[MenuItem("Tools/预制工具 %&s", false, 900)]
		public static PrefabTools OpenWindow()
		{
			var minWinWidth = Mathf.Min(3 * (BTN_WIDTH + 3) + 3, WIN_WIDTH);
			var maxWinWidth = Mathf.Min(12 * (BTN_WIDTH + 3) + 3, WIN_WIDTH);
			_win = GetWindow<PrefabTools>("预制工具");
			_win.minSize = new Vector2(minWinWidth, 400);
			_win.maxSize = new Vector2(maxWinWidth, 980);
			_win.Init();
			return _win;
		}

		private static PrefabTools _win;

		public int Tab { get; set; }
		private int tabColumn = 4;

		public const string PathListKey = "FindPrefabImage.PrefabPathList";
		public const string DefaultPath = "Assets/Game/UIs";
		private SerializedObject mPathSerializedObject;
		private ReorderableList mPathReorderableList;
		public List<Object> PrefabPathList = null;

		private readonly List<string> mToolNameList = new List<string>();
		private readonly List<PageBase> mToolList = new List<PageBase>();

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

		public FindPrefabImage findImage { get; private set; }
		public FindRefByGuid findRefByGuid { get; private set; }

		private void Init()
		{
			InitPrefabPaths();

			findImage = new FindPrefabImage(this);
			AddTool("查找图片", findImage);

			AddTool("查找字体", new FindPrefabFont(this));
			AddTool("查找文字", new FindPrefabChar(this));

			findRefByGuid = new FindRefByGuid(this);
			AddTool("GUID引用", findRefByGuid);

			AddTool("查找相同图片", new FindSameImage(this));
		}

		private void AddTool(string name, PageBase tool)
		{
			mToolNameList.Add(name);
			mToolList.Add(tool);
		}

		public void SetFindImage(Sprite sprite)
		{
			findImage.InitFindImage(sprite);
		}

		private void OnLostFocus()
		{
			SavePrefabPaths();
		}

		private void OnGUI()
		{
			var oldColor = GUI.color;
			tabColumn = Mathf.FloorToInt(position.width / (BTN_WIDTH + 3));
			for (int i = 0, len = mToolList.Count; i < len; i += tabColumn)
			{
				GUILayout.BeginHorizontal();
				for (int j = 0; j < tabColumn; j++)
				{
					var index = i + j;
					if (index < len)
					{
						GUI.color = Tab == index ? Color.grey : Color.white;
						if (GUILayout.Button(mToolNameList[index], GUILayout.MaxWidth(BTN_WIDTH)))
							Tab = index;
					}
				}
				GUILayout.EndHorizontal();
			}
			GUI.color = oldColor;

			EditorGUIUtility.labelWidth = 64;
			mPathReorderableList?.DoLayoutList();
			mToolList[Tab].OnGUI();
		}

		private void InitPrefabPaths()
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