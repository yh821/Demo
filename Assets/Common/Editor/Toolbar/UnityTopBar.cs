using System.Collections.Generic;
using System.IO;
using Common;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

[InitializeOnLoad]
public class UnityTopBar
{
	public class OpenSceneHistory
	{
		public List<string> history;

		public OpenSceneHistory()
		{
			history = new List<string>();
		}

		public string[] GetHistory()
		{
			var content = new string[history.Count];
			for (int i = 0, len = history.Count; i < len; i++)
			{
				content[i] = Path.GetFileNameWithoutExtension(history[i]);
			}
			return content;
		}
	}

	public const string ProjName = "UnityTools";

	private const string OptionFile = "UserSettings/OpenSceneHistory.json";
	private const int MaxHistory = 20;
	private const int BTN_WIDTH = 50;
	private const int BTN_HEIGHT = 22;
	private const int POPUP_WIDTH = 220;

	private static GUIStyle title_style;

	private static OpenSceneHistory option;
	private static OpenSceneHistory Option =>
		option ??= File.Exists(OptionFile)
			? IOHelper.ReadJson<OpenSceneHistory>(OptionFile)
			: new OpenSceneHistory();

	private static string[] history;
	private static int openSceneIndex = 0;
	private static int lastOpenSceneIndex = 0;
	private static int UpVersion = 0;

	static UnityTopBar()
	{
		ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
		ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);
		EditorSceneManager.sceneOpened += OnSceneOpened;
		// EditorSceneManager.newSceneCreated += OnSceneCreated;
		// PrefabStage.prefabStageOpened += OnPrefabStageOpened;
	}

	private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
	{
		MarkOpenScene(scene.path);
	}

	private static void OnSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode) { }

	private static void OnPrefabStageOpened(PrefabStage stage) { }

	private static void MarkOpenScene(string scenePath)
	{
		if (string.IsNullOrEmpty(scenePath))
		{
			openSceneIndex = 0;
			lastOpenSceneIndex = 0;
		}
		else
		{
			if (Option.history.Contains(scenePath))
				Option.history.Remove(scenePath);
			Option.history.Add(scenePath);
			if (Option.history.Count > MaxHistory)
				Option.history.RemoveAt(0);
			IOHelper.SaveJson(OptionFile, Option);
			openSceneIndex = Option.history.Count - 1;
			lastOpenSceneIndex = openSceneIndex;
		}
		history = Option.GetHistory();
	}

	private static void OnLeftToolbarGUI()
	{
		if (title_style == null) title_style = new GUIStyle("BoldLabel") {fontSize = 16};

		if (EditorApplication.isPlaying) GUILayout.Space(POPUP_WIDTH);
		else
		{
			if (history == null)
			{
				if (GUILayout.Button("场景打开记录", GUILayout.MaxWidth(POPUP_WIDTH), GUILayout.Height(BTN_HEIGHT)))
					MarkOpenScene(string.Empty);
			}
			else
			{
				openSceneIndex = EditorGUILayout.Popup(openSceneIndex, history,
					GUILayout.MaxWidth(POPUP_WIDTH), GUILayout.Height(BTN_HEIGHT));
				if (openSceneIndex != lastOpenSceneIndex)
				{
					lastOpenSceneIndex = openSceneIndex;
					var path = Option.history[openSceneIndex];
					if (!string.IsNullOrEmpty(path) && File.Exists(path))
						EditorSceneManager.OpenScene(path);
					else
					{
						Option.history.RemoveAt(openSceneIndex);
						IOHelper.SaveJson(OptionFile, Option);
						openSceneIndex = Option.history.Count - 1;
						lastOpenSceneIndex = openSceneIndex;
						history = Option.GetHistory();
						Debug.LogError("不存在场景: " + path);
					}
				}
			}
		}

		if (GUILayout.Button(EditorGUIUtility.IconContent("SettingsIcon", "|svn路径配置"), GUILayout.Height(22)))
		{
			SVNHelper.OpenPathOption();
		}

		if (GUILayout.Button(new GUIContent("提交", "提交unity工程svn"), GUILayout.Height(22)))
		{
			SVNHelper.SvnCommand(SVNHelper.Command.Commit, SVNHelper.GetCommitPaths());
		}

		GUI.color = new Color(0.33f, 0.75f, 1f);
		if (GUILayout.Button(new GUIContent("更新", "更新unity工程svn"), GUILayout.Height(22)))
		{
			if (EditorApplication.isPlaying)
				EditorApplication.isPaused = true;
			if (UpVersion > 0) SVNHelper.SvnUpdate(SVNHelper.ProjectPath, UpVersion);
			else SVNHelper.SvnCommand(SVNHelper.Command.Update, SVNHelper.GetUpdatePaths());
		}
		if (EditorPrefs.GetBool(SVNHelper.ShowUpVerKey))
			UpVersion = EditorGUILayout.IntField(UpVersion, GUILayout.MaxWidth(BTN_WIDTH));
		GUI.color = Color.white;
	}

	private static void OnRightToolbarGUI()
	{
		GUILayout.Label(ProjName, title_style);
		GUILayout.Space(10);
		if (EditorApplication.isPlaying) return;

		GUI.color = Color.green;
		if (GUILayout.Button(new GUIContent("主场景", "启动游戏场景"), GUILayout.Height(22)))
		{
			EditorSceneManager.OpenScene("Assets/Game/Scenes/main.unity");
		}
		GUI.color = Color.white;

		if (GUILayout.Button(new GUIContent("空场景", "新建空场景"), GUILayout.Height(22)))
		{
			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
			var camera = new GameObject("Camera", typeof(Camera));
			camera.transform.position = new Vector3(0, 1, -10);
		}

		GUILayout.Space(10);
		GUILayout.Label(SVNHelper.CustomProjName, title_style);
	}

	private static void SceneCheckList()
	{
		var content = new System.Text.StringBuilder();
		var scene = SceneManager.GetActiveScene();

		var missingPrefabList = new List<GameObject>();
		var roots = scene.GetRootGameObjects();
		foreach (var go in roots)
		{
			//删除第一层空节点
			if (go.transform.childCount == 0)
			{
				var com = go.GetComponentsInChildren<Component>();
				if (com.Length <= 1)
				{
					Debug.Log($"删除空节点: <color=yellow>{go.name}</color>");
					Object.DestroyImmediate(go);
					continue;
				}
			}

			//收集MissingPrefab的实例
			WalkNode(go.transform, missingPrefabList);
		}

		//清除MissingPrefab的实例
		foreach (var go in missingPrefabList)
		{
			Debug.Log($"删除MissingPrefab: <color=yellow>{EditorHelper.GetPath(go)}</color>");
			Object.DestroyImmediate(go);
		}

		//是否烘培
		if (Lightmapping.lightingDataAsset == null)
		{
			content.Append("缺少光照贴图, 检查是否已烘培!");
		}

		//寻路网格
		var triangulation = NavMesh.CalculateTriangulation();
		if (triangulation.vertices.Length <= 0)
		{
			content.Append("缺少寻路网格, 检查是否已烘培!");
		}

		//环境光
		var lightColor = RenderSettings.ambientLight;
		var grayLevel = lightColor.r * 0.299f + lightColor.g * 0.587f + lightColor.b * 0.114f;
		if (grayLevel < 0.75f)
		{
			RenderSettings.ambientLight = Color.white;
			content.Append("已将环境光改为白色!");
		}

		if (content.Length > 0)
		{
			Debug.LogError(content.ToString());
			EditorUtility.DisplayDialog("错误", content.ToString(), "确定");
		}
		else
		{
			Debug.Log("场景检查完成");
		}

		EditorSceneManager.SaveScene(scene);
	}

	private static void WalkNode(Transform parent, List<GameObject> missingList)
	{
		if (PrefabUtility.IsPrefabAssetMissing(parent.gameObject))
		{
			missingList.Add(parent.gameObject);
			return;
		}

		if (parent.childCount == 0)
			return;
		for (int i = 0, len = parent.childCount; i < len; i++)
		{
			WalkNode(parent.GetChild(i), missingList);
		}
	}
}