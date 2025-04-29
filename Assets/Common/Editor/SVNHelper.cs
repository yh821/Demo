using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Common
{
	public class SVNHelper
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

		#region 命令枚举

		public enum Command
		{
			Log,
			CheckOut,
			Update,
			Commit,
			Add,
			Revert,
			CleanUp,
			Resolve, //解决
			Remove,
			Rename,
			Diff,
			Ignore,
			Lock,
			UnLock,
		}

		#endregion

		private static string projectPath = string.Empty;

		public static string ProjectPath
		{
			get
			{
				if (string.IsNullOrEmpty(projectPath))
					projectPath = Application.dataPath.Replace("/Assets", "");

				return projectPath;
			}
		}

		#region 前台命令

		/// <summary>
		/// 执行SVN命令
		/// </summary>
		/// <param name="command">命令</param>
		/// <param name="path">操作路径</param>
		/// <param name="closeonend">0:不自动关闭,1:如果没发生错误则自动关闭对话框,
		/// 2:如果没发生错误和冲突则自动关闭对话框,3:如果没有错误、冲突和合并，会自动关闭
		/// 详细描述可在TortoiseSVN/help/Automating TortoiseSVN查看</param>
		public static void SvnCommand(string command, string path, int closeonend = -1, bool isWaitForExit = false)
		{
			var cmd = $"/c tortoiseproc.exe /command:{command.ToLower()} /path:\"{path}\"";
			if (closeonend >= 0 && closeonend <= 3) cmd += $" /closeonend:{closeonend}";
			// var info = new ProcessStartInfo("cmd.exe", cmd) {WindowStyle = ProcessWindowStyle.Hidden};
			// var process = Process.Start(info);
			// process?.Close();
			CmdProcess(cmd, isWaitForExit);
		}

		public static void SvnCommand(Command command, string path, int closeonend = -1, bool isWaitForExit = false)
		{
			SvnCommand(command.ToString(), path, closeonend, isWaitForExit);
		}

		public static void SvnCommand(Command command, List<string> paths, int closeonend = -1,
			bool isWaitForExit = false)
		{
			var pathDict = new Dictionary<string, List<string>>();
			foreach (var path in paths)
			{
				if (path.StartsWith("#") || path.Length < 2 || path[1] != ':')
					continue;
				var disk = path.Substring(0, 2).ToLower();
				if (pathDict.TryGetValue(disk, out var pathList))
					pathList.Add(path);
				else
				{
					pathList = new List<string> {path};
					pathDict.Add(disk, pathList);
				}
			}

			foreach (var pathList in pathDict.Values)
			{
				var sb = new StringBuilder();
				for (int i = 0, len = pathList.Count; i < len; i++)
				{
					if (i == 0)
						sb.Append(pathList[i]);
					else
						sb.Append('*').Append(pathList[i]);
				}

				SvnCommand(command, sb.ToString(), closeonend, isWaitForExit);
			}
		}

		public static void SvnUpdate(string path, int version = 0, int closeonend = -1, bool isWaitForExit = false)
		{
			if (version > 0)
			{
				var cmd = $"{Command.Update.ToString()} /rev:{version}";
				SvnCommand(cmd, path, closeonend, isWaitForExit);
			}
			else SvnCommand(Command.Update, path, closeonend, isWaitForExit);
		}

		#endregion

		#region 后台命令

		public static string SvnProcess(string cmd, string filePath = "", bool isWaitForExit = true)
		{
			return StartProcess("svn", string.IsNullOrEmpty(filePath) ? cmd : $"{cmd} {filePath}", isWaitForExit);
		}

		public static string SvnProcess(Command command, string filePath = "", bool isWaitForExit = true)
		{
			var cmd = command.ToString().ToLower();
			return StartProcess("svn", string.IsNullOrEmpty(filePath) ? cmd : $"{cmd} {filePath}", isWaitForExit);
		}

		public static string CmdProcess(string command, bool isWaitForExit = true)
		{
			return StartProcess("cmd", command, isWaitForExit);
		}

		private static string StartProcess(string fileName, string arguments, bool isWaitForExit = true)
		{
			var process = new Process();
			process.StartInfo.FileName = fileName;
			process.StartInfo.Arguments = arguments;
			process.StartInfo.WorkingDirectory = ProjectPath;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;

			process.Start();
			if (isWaitForExit)
			{
				process.WaitForExit();
				var output = process.StandardOutput.ReadToEnd();
				process.Close();
				return output;
			}

			process.Close();
			return string.Empty;
		}

		public static string RunCmdExe(string arguments)
		{
			var process = new Process();
			process.StartInfo.FileName = "cmd";
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.Start();
			process.StandardInput.WriteLine(arguments);
			process.StandardInput.WriteLine("exit");
			process.WaitForExit();
			var output = process.StandardOutput.ReadToEnd();
			process.Close();
			return output;
		}

		#endregion

		#region 菜单选项

		[MenuItem("Tools/SVN/更新 %&e")]
		public static void UpdateFromSVN()
		{
			SvnCommand(Command.Update, ProjectPath, 0);
		}

		[MenuItem("Tools/SVN/提交 %&r")]
		public static void CommitToSVN()
		{
			SvnCommand(Command.Commit, ProjectPath);
		}

		[MenuItem("Tools/SVN/清理")]
		public static void CleanUpFromSVN()
		{
			SvnCommand(Command.CleanUp, ProjectPath);
		}

		[MenuItem("Tools/SVN/解决")]
		public static void ResolveFromSVN()
		{
			SvnCommand(Command.Resolve, ProjectPath);
		}

		#endregion

		#region 右键选项

		private static void ExecuteSelectionSvnCmd(Command cmd, int closeonend = -1)
		{
			ExecuteSelectionSvnCmd(Selection.activeObject, cmd, closeonend);
		}

		public static void ExecuteSelectionSvnCmd(Object obj, Command cmd, int closeonend = -1)
		{
			if (obj == null)
				return;

			string path = AssetDatabase.GetAssetOrScenePath(obj);
			if (string.IsNullOrEmpty(path))
				return;

			path = Path.GetFullPath(path);
			path = $"{path}*{path}.meta";
			SvnCommand(cmd, path, closeonend);
		}

		[MenuItem("Assets/SVN Command/Log")]
		public static void SvnLogCommand()
		{
			ExecuteSelectionSvnCmd(Command.Log);
		}

		[MenuItem("Assets/SVN Command/Revert")]
		public static void SvnRevertCommand()
		{
			ExecuteSelectionSvnCmd(Command.Revert, 3);
		}

		[MenuItem("Assets/SVN Command/Update")]
		public static void SvnUpdateCommand()
		{
			ExecuteSelectionSvnCmd(Command.Update);
		}

		[MenuItem("Assets/SVN Command/Commit")]
		public static void SvnCommitCommand()
		{
			ExecuteSelectionSvnCmd(Command.Commit);
		}

		[MenuItem("Assets/SVN Command/Add")]
		public static void SvnAddCommand()
		{
			ExecuteSelectionSvnCmd(Command.Add);
		}

		[MenuItem("Assets/SVN Command/Remove")]
		public static void SvnRemoveCommand()
		{
			ExecuteSelectionSvnCmd(Command.Remove);
		}

		#endregion

		#region SVN配置

		[MenuItem("Tools/SVN/路径配置")]
		public static void OpenPathOption()
		{
			var win = EditorWindow.GetWindow<SVNHelperEditor>("SVN路径配置");
			win.InitPathGui();
		}

		public class PathOption
		{
			public List<string> commitPaths;
			public List<string> updatePaths;
			public string customProjName;

			public PathOption()
			{
				commitPaths = new List<string>();
				updatePaths = new List<string>();
				customProjName = "";
			}
		}

		private const string OptionFile = "UserSettings/SvnPathOption.json";

		private static PathOption _option;
		public static PathOption Option => _option ??= LoadOrCreate(OptionFile);

		public const string ShowUpVerKey = "SVNHelper.ShowUpVerKey";
		public const string PauseOnUpKey = "SVNHelper.PauseOnUpKey";

		public static PathOption LoadOrCreate(string path)
		{
			PathOption option;
			if (File.Exists(path))
			{
				option = IOHelper.ReadJson<PathOption>(path);
			}
			else
			{
				option = new PathOption();
				IOHelper.SaveJson(OptionFile, option);
			}

			return option;
		}

		public static void SaveOption(List<string> commitPaths, List<string> updatePaths, string customProjName)
		{
			IOHelper.SaveJson(OptionFile, new PathOption
			{
				commitPaths = commitPaths, updatePaths = updatePaths, customProjName = customProjName
			});
		}

		public static List<string> GetCommitPaths()
		{
			var paths = Option.commitPaths;
			if (paths.Count <= 0)
				paths.Add(ProjectPath);
			return paths;
		}

		public static List<string> GetUpdatePaths()
		{
			var paths = Option.updatePaths;
			if (paths.Count <= 0)
				paths.Add(ProjectPath);
			return paths;
		}

		public static string CustomProjName
		{
			get => Option.customProjName;
			set => Option.customProjName = value;
		}

		#endregion
	}

	public class SVNHelperEditor : EditorWindow
	{
		public List<string> CommitPathList;
		public List<string> UpdatePathList;

		private SerializedObject mPathSerializedObject;
		private SerializedProperty mCommitPathSerializedProperty;
		private ReorderableList mCommitPathReorderableList;
		private SerializedProperty mUpdatePathSerializedProperty;
		private ReorderableList mUpdatePathReorderableList;

		private const int BTN_WIDTH = 32;
		private const int PADDING = 4;

		private bool isShowUpVer = false;
		private bool isPauseOnUp = false;
		private string customProjName = "";

		private void OnLostFocus()
		{
			SavePath();
		}

		private void OnGUI()
		{
			GUILayout.Label("工程目录处理:");
			EditorGUILayout.BeginHorizontal();
			{
				if (GUILayout.Button("日志"))
					SVNHelper.SvnCommand(SVNHelper.Command.Log, SVNHelper.ProjectPath);
				if (GUILayout.Button("解决"))
					SVNHelper.SvnCommand(SVNHelper.Command.Resolve, SVNHelper.ProjectPath);
				if (GUILayout.Button("还原"))
					SVNHelper.SvnCommand(SVNHelper.Command.Revert, SVNHelper.ProjectPath);
			}
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(20);
			mCommitPathReorderableList?.DoLayoutList();
			mUpdatePathReorderableList?.DoLayoutList();

			EditorGUI.BeginChangeCheck();
			isShowUpVer = GUILayout.Toggle(isShowUpVer, "更新指定版本");
			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetBool(SVNHelper.ShowUpVerKey, isShowUpVer);
			}

			EditorGUI.BeginChangeCheck();
			isPauseOnUp = GUILayout.Toggle(isPauseOnUp, "更新时暂停游戏");
			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetBool(SVNHelper.PauseOnUpKey, isPauseOnUp);
			}

			EditorGUI.BeginChangeCheck();
			customProjName = EditorGUILayout.TextField("自定义项目名:", customProjName);
			if (EditorGUI.EndChangeCheck())
			{
				SVNHelper.CustomProjName = customProjName;
			}
		}

		public void InitPathGui()
		{
			isShowUpVer = EditorPrefs.GetBool(SVNHelper.ShowUpVerKey);
			CommitPathList = SVNHelper.GetCommitPaths();
			UpdatePathList = SVNHelper.GetUpdatePaths();
			mPathSerializedObject = new SerializedObject(this);

			mCommitPathSerializedProperty = mPathSerializedObject.FindProperty("CommitPathList");
			mCommitPathReorderableList = new ReorderableList(mPathSerializedObject, mCommitPathSerializedProperty)
			{
				drawHeaderCallback = rect => GUI.Label(rect, "提交路径:"),
				drawElementCallback = (rect, index, selected, focused) =>
				{
					var element = mCommitPathSerializedProperty.GetArrayElementAtIndex(index);
					var rectX = rect.x;
					var path = element.stringValue;
					var enable = !path.StartsWith("#");
					var lastEnable = GUI.Toggle(new Rect(rectX, rect.y + 2, 16, rect.height - 4), enable, "");
					if (lastEnable != enable)
					{
						if (lastEnable)
							path = path.Substring(1);
						else
							path = "#" + path;
						SetPath(path, element);
					}

					rectX += 16 + PADDING;
					if (GUI.Button(new Rect(rectX, rect.y + 2, BTN_WIDTH, rect.height - 4),
						EditorGUIUtility.IconContent("Folder Icon", "选择文件夹")))
					{
						SetPath(EditorUtility.OpenFolderPanel("选择提交文件夹", element.stringValue, ""), element);
					}

					rectX += BTN_WIDTH + PADDING;
					if (GUI.Button(new Rect(rectX, rect.y + 2, BTN_WIDTH, rect.height - 4),
						EditorGUIUtility.IconContent("TextAsset Icon", "选择文件")))
					{
						SetPath(EditorUtility.OpenFilePanel("选择提交文件", element.stringValue, "*.*"), element);
					}

					rectX += BTN_WIDTH + PADDING;
					EditorGUI.LabelField(new Rect(rectX, rect.y, rect.width - rectX, rect.height), element.stringValue);
				}
			};

			mUpdatePathSerializedProperty = mPathSerializedObject.FindProperty("UpdatePathList");
			mUpdatePathReorderableList = new ReorderableList(mPathSerializedObject, mUpdatePathSerializedProperty)
			{
				drawHeaderCallback = rect => GUI.Label(rect, "更新路径:"),
				drawElementCallback = (rect, index, selected, focused) =>
				{
					var element = mUpdatePathSerializedProperty.GetArrayElementAtIndex(index);
					var rectX = rect.x;
					var path = element.stringValue;
					var enable = !path.StartsWith("#");
					var lastEnable = GUI.Toggle(new Rect(rectX, rect.y + 2, 16, rect.height - 4), enable, "");
					if (lastEnable != enable)
					{
						if (lastEnable)
							path = path.Substring(1);
						else
							path = "#" + path;
						SetPath(path, element);
					}

					rectX += 16 + PADDING;
					if (GUI.Button(new Rect(rectX, rect.y + 2, BTN_WIDTH, rect.height - 4),
						EditorGUIUtility.IconContent("Folder Icon", "选择文件夹")))
					{
						SetPath(EditorUtility.OpenFolderPanel("选择更新文件夹", element.stringValue, ""), element);
					}

					rectX += BTN_WIDTH + PADDING;
					if (GUI.Button(new Rect(rectX, rect.y + 2, BTN_WIDTH, rect.height - 4),
						EditorGUIUtility.IconContent("TextAsset Icon", "选择文件")))
					{
						SetPath(EditorUtility.OpenFilePanel("选择更新文件", element.stringValue, "*.*"), element);
					}

					rectX += BTN_WIDTH + PADDING;
					EditorGUI.LabelField(new Rect(rectX, rect.y, rect.width - rectX, rect.height), element.stringValue);
				}
			};

			customProjName = SVNHelper.CustomProjName;
		}

		private void SetPath(string path, SerializedProperty element)
		{
			if (string.IsNullOrEmpty(path)) return;
			path = path.Replace('/', '\\');
			element.stringValue = path;
		}

		private void SavePath()
		{
			mPathSerializedObject.ApplyModifiedProperties();

			for (int i = 0; i < CommitPathList.Count; i++)
			{
				if (string.IsNullOrEmpty(CommitPathList[i]))
					CommitPathList.RemoveAt(i);
			}

			for (int i = 0; i < UpdatePathList.Count; i++)
			{
				if (string.IsNullOrEmpty(UpdatePathList[i]))
					UpdatePathList.RemoveAt(i);
			}

			SVNHelper.SaveOption(CommitPathList, UpdatePathList, customProjName);
		}
	}
}