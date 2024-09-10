using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CreateFrameAnimtion : EditorWindow
{
	private static string pathPrefix = "Assets/Game/UIs";

	private Object dirObj;
	private int frameRate = 15;
	private int clipLength = 2000;
	private bool isLoop = true;

	[MenuItem("Game/Animation/创建序列帧动画")]
	private static void ShowWindow()
	{
		GetWindow<CreateFrameAnimtion>(false, "创建序列帧动画");
	}

	private void OnGUI()
	{
		EditorGUI.BeginChangeCheck();
		dirObj = EditorGUILayout.ObjectField("动画目录", dirObj, typeof(Object), false);
		if (EditorGUI.EndChangeCheck())
		{
			var path = AssetDatabase.GetAssetPath(dirObj);
			if (!Directory.Exists(path))
			{
				EditorUtility.DisplayDialog("错误", "只能对目录进行处理", "确定");
				dirObj = null;
				return;
			}
			if (!path.StartsWith(pathPrefix))
			{
				EditorUtility.DisplayDialog("错误", "只能对UI目录进行处理", "确定");
				dirObj = null;
				return;
			}
		}
		if (dirObj == null) return;

		frameRate = EditorGUILayout.IntField("总帧数", frameRate);
		clipLength = EditorGUILayout.IntField("时长(毫秒)", clipLength);
		isLoop = EditorGUILayout.Toggle("循环播放", isLoop);

		if (GUILayout.Button("开始创建"))
		{
			var path = AssetDatabase.GetAssetPath(dirObj);
			var files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
			var sprites = new List<Sprite>();
			foreach (var file in files)
			{
				var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(file);
				if (sprite == null)
				{
					Debug.Log(file);
					continue;
				}
				sprites.Add(sprite);
			}

			if (sprites.Count <= 0)
			{
				EditorUtility.DisplayDialog("错误", "没有Sprite图", "确定");
				return;
			}

			sprites.Sort((a, b) => string.CompareOrdinal(a.name, b.name));

			var clip = new AnimationClip {frameRate = frameRate};
			var clipSetting = AnimationUtility.GetAnimationClipSettings(clip);
			clipSetting.loopTime = isLoop;
			AnimationUtility.SetAnimationClipSettings(clip, clipSetting);

			var curveBinding = new EditorCurveBinding {type = typeof(Image), propertyName = "m_Sprite", path = ""};
			var interval = Mathf.CeilToInt((float) clipLength / frameRate);
			var objRefKeyframes = new ObjectReferenceKeyframe[sprites.Count];
			for (int i = 0; i < sprites.Count; i++)
			{
				objRefKeyframes[i].time = interval * i / 1000f;
				objRefKeyframes[i].value = sprites[i];
			}

			AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, objRefKeyframes);
			var folder = Path.GetFileNameWithoutExtension(path);
			AssetDatabase.CreateAsset(clip, Path.Combine(path, folder + ".anim"));
			AssetDatabase.Refresh();
		}
	}
}