using System.Collections.Generic;
using System.IO;
using Common;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
	public class GmWindows : EditorWindow
	{
		[MenuItem("Game/GmView")]
		private static void ShowGmWindow()
		{
			GetWindow<GmWindows>(false, "GmView");
		}

		private class GmGroupData
		{
			public string name;
			public bool foldout = true;
			public List<List<string>> children;

			public GmGroupData(string name)
			{
				this.name = name;
				children = new List<List<string>>();
			}

			public void AddChild(List<string> child)
			{
				children.Add(child);
			}
		}

		private static readonly string[] TAB =
		{
			"所有指令",
			"分组显示",
		};

		private int Tab
		{
			get => mTab;
			set
			{
				if (mTab != value) RefreshGmShowList();
				mTab = value;
			}
		}

		private int mTab = 0;

		private const string GmFile = "Assets/Game/Scripts/Editor/CommonGM.json";
		private const string GmUserFile = "UserSettings/CommonGM.json";
		private const int WIDTH_NO2 = 26;
		private const int WIDTH_NO3 = 36;
		private const int WIDTH_NO4 = 50;
		private const int WIDTH_NO5 = 60;
		private const int BTN_HEIGHT = 20;
		private const int BTN_WIDTH = 80;
		private const int VIEW_HEIGHT = 256;

		private List<List<string>> mShowGmList = new List<List<string>>();
		private List<List<string>> mAllGmList = new List<List<string>>();
		private List<GmGroupData> mGmGroupList = new List<GmGroupData>();
		private bool mSettingMode = false;
		private string mSearchKey = "";
		private string mCmContent = "";
		private string mNewGmName = "";
		private string mNewGmCmd = "";
		private string mNewGmGroup = "";
		private string mNewGmDesc = "";
		private bool mNewGmAuto = false;

		private int mBtnWidth = BTN_WIDTH;
		private int mBtnHeight = BTN_HEIGHT;
		private int mColumn = 5;
		private Vector2 mAllGmScrollPos = Vector2.zero;
		private Vector2 mGmGroupScrollPos = Vector2.zero;
		private Color mAutoSettingColor = new Color(0.75f, 0.75f, 0);

		private void OnEnable()
		{
			mAllGmList = IOHelper.LoadOrCreate<List<List<string>>>(File.Exists(GmUserFile) ? GmUserFile : GmFile);
			RefreshGmShowList();
		}

		private void OnGUI()
		{
			EditorGUIUtility.labelWidth = BTN_WIDTH;

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("GM:", GUILayout.Width(25));
				mCmContent = EditorGUILayout.TextArea(mCmContent);
				GUI.color = Color.green;
				if (GUILayout.Button("发送命令", GUILayout.MaxWidth(120)))
				{
					ExecuteGm(mCmContent);
				}
				GUI.color = Color.white;
			}
			EditorGUILayout.EndHorizontal();

			mColumn = Mathf.FloorToInt((position.width - 3) / (mBtnWidth + 3));
			Tab = GUILayout.Toolbar(Tab, TAB);
			switch (Tab)
			{
				case 0:
					DrawAllGmList();
					break;
				case 1:
					DrawGmGroupList();
					break;
			}
			DrawEditorGm();
		}

		private void DrawAllGmList()
		{
			GUI.color = mSettingMode ? Color.grey : Color.white;
			mAllGmScrollPos = EditorGUILayout.BeginScrollView(mAllGmScrollPos, GUILayout.MaxHeight(VIEW_HEIGHT));
			{
				DrawGmButtonList(mShowGmList);
			}
			EditorGUILayout.EndScrollView();
			GUI.color = Color.white;
		}

		private void DrawGmGroupList()
		{
			GUI.color = mSettingMode ? Color.grey : Color.white;
			mGmGroupScrollPos = EditorGUILayout.BeginScrollView(mGmGroupScrollPos, GUILayout.MaxHeight(VIEW_HEIGHT));
			{
				foreach (var gmGroup in mGmGroupList)
				{
					var groupName = string.IsNullOrEmpty(gmGroup.name) ? "未分组" : gmGroup.name;
					gmGroup.foldout = EditorGUILayout.Foldout(gmGroup.foldout, groupName, true);
					if (!gmGroup.foldout) continue;
					DrawGmButtonList(gmGroup.children);
				}
			}
			EditorGUILayout.EndScrollView();
			GUI.color = Color.white;
		}

		private void DrawGmButtonList(List<List<string>> gmList)
		{
			for (var i = 0; i < gmList.Count; i += mColumn)
			{
				EditorGUILayout.BeginHorizontal();
				for (var j = 0; j < mColumn; j++)
				{
					var index = i + j;
					if (index >= gmList.Count) continue;
					var gm = gmList[index];
					if (GUILayout.Button(gm[0], GUILayout.Width(BTN_WIDTH)))
					{
						mNewGmName = gm[0];
						mNewGmCmd = gm[1];
						mNewGmAuto = gm[2] == "1";
						mNewGmGroup = gm[3];
						mNewGmDesc = gm[4];
						mCmContent = mNewGmCmd;
						if (!mSettingMode)
							SetGmCmd(mNewGmCmd, mNewGmAuto);
					}
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawEditorGm()
		{
			EditorGUILayout.BeginHorizontal();
			{
				GUI.enabled = mSettingMode;
				EditorGUILayout.LabelField("GM名:", GUILayout.Width(WIDTH_NO4));
				mNewGmName = EditorGUILayout.TextField(mNewGmName);
				EditorGUILayout.LabelField("自动发送:", GUILayout.Width(WIDTH_NO4));
				mNewGmAuto = EditorGUILayout.Toggle(mNewGmAuto, GUILayout.Width(14));
				if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"), GUILayout.Width(WIDTH_NO2)))
				{
					if (string.IsNullOrEmpty(mNewGmName))
					{
						ShowNotification(new GUIContent("请设置GM名"));
						return;
					}
					if (string.IsNullOrEmpty(mNewGmCmd))
					{
						ShowNotification(new GUIContent("请设置GM指令"));
						return;
					}
					if (!mNewGmCmd.StartsWith("/gm ") && !mNewGmCmd.StartsWith("/cmd "))
					{
						ShowNotification(new GUIContent("GM指令格式有误"));
						return;
					}
					var isExist = false;
					foreach (var list in mAllGmList)
					{
						if (list[0] == mNewGmName)
						{
							list[1] = mNewGmCmd;
							list[2] = mNewGmAuto ? "1" : "0";
							list[3] = mNewGmGroup;
							list[4] = mNewGmDesc;
							ShowNotification(new GUIContent($"已修改GM:{mNewGmName}"));
							isExist = true;
							break;
						}
					}
					if (!isExist)
					{
						mAllGmList.Add(new List<string>
							{mNewGmName, mNewGmCmd, mNewGmAuto ? "1" : "0", mNewGmGroup, mNewGmDesc});
						ShowNotification(new GUIContent($"已添加GM:{mNewGmName}"));
						RefreshGmShowList();
					}
				}
				if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus"), GUILayout.Width(WIDTH_NO2)))
				{
					foreach (var list in mAllGmList)
					{
						if (list[0] == mNewGmName)
						{
							mAllGmList.Remove(list);
							ShowNotification(new GUIContent($"已删除GM名:{mNewGmName}"));
							break;
						}
					}
					RefreshGmShowList();
				}
				GUI.enabled = true;
				GUI.color = new Color(0.6f, 0.9f, 1f, 1f);
				if (GUILayout.Button(mSettingMode ? "保存指令" : "编辑指令", GUILayout.Width(BTN_WIDTH)))
				{
					mSettingMode = !mSettingMode;
					if (!mSettingMode) IOHelper.SaveJson(GmUserFile, mAllGmList);
				}
				GUI.color = Color.white;
			}
			EditorGUILayout.EndHorizontal();

			if (mSettingMode)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("GM指令:", GUILayout.Width(WIDTH_NO4));
				mNewGmCmd = EditorGUILayout.TextArea(mNewGmCmd);
				EditorGUILayout.EndHorizontal();
			}

			GUI.enabled = mSettingMode;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("GM组:", GUILayout.Width(WIDTH_NO3));
			mNewGmGroup = EditorGUILayout.TextField(mNewGmGroup, GUILayout.Width(WIDTH_NO5));
			EditorGUILayout.LabelField("说明:", GUILayout.Width(WIDTH_NO3));
			mNewGmDesc = EditorGUILayout.TextArea(mNewGmDesc);
			EditorGUILayout.EndHorizontal();
			GUI.enabled = true;
		}

		//TODO 设置当前指令
		private void SetGmCmd(string cmd, bool auto = false)
		{
			mCmContent = cmd;
			if (auto) ExecuteGm(cmd);
		}

		//TODO 执行当前指令
		private void ExecuteGm(string gmContent)
		{
			if (gmContent.Contains('\n'))
			{
				var gmList = gmContent.Split('\n');
				foreach (var g in gmList)
				{
					var gm = g.Trim();
					if (!string.IsNullOrEmpty(gm))
						Debug.Log("ExecuteGm:" + gm);
				}
			}
			else Debug.Log("ExecuteGm: " + gmContent);
		}

		private void RefreshGmShowList(int tab = -1)
		{
			mShowGmList.Clear();
			mGmGroupList.Clear();
			var list = mAllGmList;
			if (string.IsNullOrEmpty(mSearchKey))
			{
				foreach (var item in list)
				{
					mShowGmList.Add(item);
					AddGmInGroup(item);
				}
			}
			else
			{
				var key = mSearchKey.ToLower();
				foreach (var item in list)
				{
					if (item[0].ToLower().Contains(key))
						mShowGmList.Add(item);
				}
			}
			if (tab >= 0) mTab = tab;
		}

		private void AddGmInGroup(List<string> data)
		{
			var groupName = data[3];
			var item = mGmGroupList.Find(a => a.name == groupName);
			if (item == null)
			{
				item = new GmGroupData(groupName);
				mGmGroupList.Add(item);
			}
			item.AddChild(data);
		}
	}
}