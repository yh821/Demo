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

		private const string GmFile = "Assets/Game/Scripts/Editor/CommonGM.json";
		private const string GmUserFile = "UserSettings/CommonGM.json";
		private const int ICON_WIDTH = 26;
		private const int LAB_WIDTH = 50;
		private const int BTN_WIDTH = 80;
		private const int Column = 5;

		private List<List<string>> mShowGmList = new List<List<string>>();
		private List<List<string>> mGmList = new List<List<string>>();
		private bool mSettingMode = false;
		private string mSearchKey = "";
		private string mCmContent = "";
		private string mNewGmName = "";
		private string mNewGmCmd = "";
		private bool mNewGmAuto = false;

		private Vector2 mGmScrollPos = Vector2.zero;

		private void OnEnable()
		{
			mGmList = IOHelper.LoadOrCreate<List<List<string>>>(File.Exists(GmUserFile) ? GmUserFile : GmFile);
			RefreshGmShowList();
		}

		private void OnGUI()
		{
			EditorGUIUtility.labelWidth = BTN_WIDTH;

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("GM:", GUILayout.Width(25));
				mCmContent = EditorGUILayout.TextField(mCmContent);
				GUI.color = Color.green;
				if (GUILayout.Button("发送命令", GUILayout.MaxWidth(120)))
				{
					ExecuteGm(mCmContent);
				}
				GUI.color = Color.white;
			}
			EditorGUILayout.EndHorizontal();

			DrawCommonGmList();
		}

		private void DrawCommonGmList()
		{
			mGmScrollPos = EditorGUILayout.BeginScrollView(mGmScrollPos, "Box");
			{
				for (var i = 0; i < mShowGmList.Count; i += Column)
				{
					EditorGUILayout.BeginHorizontal();
					for (var j = 0; j < Column; j++)
					{
						var index = i + j;
						if (index >= mShowGmList.Count) continue;
						var gm = mShowGmList[index];
						if (GUILayout.Button(gm[0], GUILayout.Width(BTN_WIDTH)))
						{
							if (mSettingMode)
							{
								mNewGmName = gm[0];
								mNewGmCmd = gm[1];
								mNewGmAuto = gm[2] == "1";
								mCmContent = mNewGmCmd;
							}
							else SetGmCmd(gm[1], gm[2] == "1");
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndScrollView();
			GUI.color = Color.white;

			EditorGUILayout.BeginHorizontal();
			{
				GUI.enabled = mSettingMode;
				EditorGUILayout.LabelField("GM名:", GUILayout.Width(LAB_WIDTH));
				mNewGmName = EditorGUILayout.TextField(mNewGmName);
				EditorGUILayout.LabelField("自动发送:", GUILayout.Width(LAB_WIDTH));
				mNewGmAuto = EditorGUILayout.Toggle(mNewGmAuto, GUILayout.Width(14));
				if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Plus"), GUILayout.Width(ICON_WIDTH)))
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
					foreach (var list in mGmList)
					{
						if (list[0] == mNewGmName)
						{
							list[1] = mNewGmCmd;
							list[2] = mNewGmAuto ? "1" : "0";
							ShowNotification(new GUIContent($"已修改GM:{mNewGmName}"));
							return;
						}
					}
					mGmList.Add(new List<string> {mNewGmName, mNewGmCmd, mNewGmAuto ? "1" : "0"});
					ShowNotification(new GUIContent($"已添加GM:{mNewGmName}"));
					RefreshGmShowList();
				}
				if (GUILayout.Button(EditorGUIUtility.IconContent("Toolbar Minus"), GUILayout.Width(ICON_WIDTH)))
				{
					foreach (var list in mGmList)
					{
						if (list[0] == mNewGmName)
						{
							mGmList.Remove(list);
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
					if (!mSettingMode) IOHelper.SaveJson(GmUserFile, mGmList);
				}
				GUI.color = Color.white;
			}
			EditorGUILayout.EndHorizontal();

			if (mSettingMode)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("GM指令:", GUILayout.Width(LAB_WIDTH));
				mNewGmCmd = EditorGUILayout.TextField(mNewGmCmd);
				EditorGUILayout.EndHorizontal();
			}
		}

		//TODO 设置当前指令
		private void SetGmCmd(string cmd, bool auto = false)
		{
			mCmContent = cmd;
			if (auto) ExecuteGm(cmd);
		}

		//TODO 执行当前指令
		private void ExecuteGm(string cmd)
		{
			Debug.Log("ExecuteGm: " + cmd);
		}

		private void RefreshGmShowList()
		{
			mShowGmList.Clear();
			var list = mGmList;
			if (string.IsNullOrEmpty(mSearchKey))
			{
				foreach (var item in list)
					mShowGmList.Add(item);
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
		}
	}
}