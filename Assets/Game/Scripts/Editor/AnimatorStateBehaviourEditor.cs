using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Game.Editor
{
	[CustomEditor(typeof(AnimatorStateBehaviour))]
	internal sealed class AnimatorStateBehaviourEditor : UnityEditor.Editor
	{
		private SerializedProperty enterActions;
		private SerializedProperty exitActions;
		private ReorderableList enterList;
		private ReorderableList exitList;

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			GUILayout.Space(EditorGUIUtility.singleLineHeight);
			enterList.DoLayoutList();
			GUILayout.Space(EditorGUIUtility.singleLineHeight);
			exitList.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}

		private void OnEnable()
		{
			if (target == null) return;
			var serializedObj = serializedObject;
			enterActions = serializedObj.FindProperty("enterActions");
			enterList = new ReorderableList(serializedObj, enterActions);
			enterList.drawHeaderCallback = rect => GUI.Label(rect, "Enter Actions");
			enterList.elementHeight = 2 * EditorGUIUtility.singleLineHeight;
			enterList.drawElementCallback = (rect, index, selected, focused) =>
				DrawAnimatorAction(enterActions, rect, index, selected, focused);
			exitActions = serializedObj.FindProperty("exitActions");
			exitList = new ReorderableList(serializedObj, exitActions);
			exitList.drawHeaderCallback = rect => GUI.Label(rect, "Exit Actions");
			exitList.elementHeight = 2 * EditorGUIUtility.singleLineHeight;
			exitList.drawElementCallback = (rect, index, selected, focused) =>
				DrawAnimatorAction(exitActions, rect, index, selected, focused);
		}

		private void DrawAnimatorAction(SerializedProperty property,
			Rect rect, int index, bool selected, bool focused)
		{
			var element = property.GetArrayElementAtIndex(index);
			var relative1 = element.FindPropertyRelative("eventName");
			var relative2 = element.FindPropertyRelative("eventParam");
			var pos = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
			EditorGUI.PropertyField(pos, relative1, new GUIContent("Event Name"));
			pos.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(pos, relative2, new GUIContent("Event Param"));
		}
	}
}