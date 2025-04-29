using UnityEngine;
using UnityEditor;

public partial class BuildInWindow
{
	Vector2 scrollPosition = new Vector2(0, 0);
	string search = "";
	GUIStyle textStyle;

	// private static GUIStyleViewer window;
	// [MenuItem("Tools/内置GUIStyle", false, 10)]
	// private static void OpenStyleViewer()
	// {
	//     window = GetWindow<GUIStyleViewer>(false, "内置GUIStyle");
	// }

	private void OnGuiStyle()
	{
		if (textStyle == null)
		{
			textStyle = new GUIStyle("HeaderLabel");
			textStyle.fontSize = 25;
		}

		GUILayout.BeginHorizontal("HelpBox");
		GUILayout.Label("结果如下：", textStyle);
		GUILayout.FlexibleSpace();
		GUILayout.Label("Search:");
		search = EditorGUILayout.TextField(search);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
		GUILayout.Label("样式展示", textStyle, GUILayout.Width(300));
		GUILayout.Label("名字", textStyle, GUILayout.Width(300));
		GUILayout.EndHorizontal();


		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		foreach (var style in GUI.skin.customStyles)
		{
			if (style.name.ToLower().Contains(search.ToLower()))
			{
				GUILayout.Space(15);
				GUILayout.BeginHorizontal("PopupCurveSwatchBackground");
				if (GUILayout.Button(style.name, style, GUILayout.Width(300)))
				{
					EditorGUIUtility.systemCopyBuffer = style.name;
					Debug.Log("style Name: " + style.name);
					Debug.Log("style Normal: " + style.normal.background);
					Debug.Log("style Normal Color: " + style.normal.textColor);
					Debug.Log("style Active: " + style.active.background);
					Debug.Log("style Active Color: " + style.active.textColor);
					Debug.Log("style Hover: " + style.hover.background);
					Debug.Log("style Hover Color: " + style.hover.textColor);
					Debug.Log("style Focused: " + style.focused.background);
					Debug.Log("style Focused Color: " + style.focused.textColor);
					Debug.Log("style border: " + style.border);
					Debug.Log("style margin:" + style.margin);
					Debug.Log("style padding: " + style.padding);
					Debug.Log("style overflow: " + style.overflow);
					Debug.Log("style fixedWidth: " + style.fixedWidth);
					Debug.Log("style fixedHeight: " + style.fixedHeight);
					Debug.Log("style stretchWidth: " + style.stretchWidth);
					Debug.Log("style stretchHeight: " + style.stretchHeight);
					Debug.Log("style lineHeight: " + style.lineHeight);
					Debug.Log("style Font: " + style.font);
					Debug.Log("style Font Size: " + style.fontSize);
				}

				EditorGUILayout.SelectableLabel(style.name, GUILayout.Width(300));
				GUILayout.EndHorizontal();
			}
		}

		GUILayout.EndScrollView();
	}
}