using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
	[CustomEditor(typeof(GameObjectAttach))]
	public class GameObjectAttachEditor : UnityEditor.Editor
	{
		private GameObject asset;

		private void OnEnable()
		{
			var self = target as GameObjectAttach;
			if (self == null || string.IsNullOrEmpty(self.BundleName) || string.IsNullOrEmpty(self.AssetName))
				return;
			asset = EditorResourceMgr.LoadGameObject(self.BundleName, self.AssetName);
		}

		public override void OnInspectorGUI()
		{
			var self = target as GameObjectAttach;
			var dirty = false;

			EditorGUI.BeginChangeCheck();
			asset = EditorGUILayout.ObjectField("AttachObj", asset, typeof(GameObject), false) as GameObject;
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Change GameObject Asset");
				dirty = true;
				var path = AssetDatabase.GetAssetPath(asset);
				var importer = AssetImporter.GetAtPath(path);
				self.BundleName = importer.assetBundleName;
				self.AssetName = Path.GetFileName(importer.assetPath);
				self.AssetGuid = AssetDatabase.AssetPathToGUID(path);
				var previewObj = self.GetComponent<PreviewObject>();
				if (previewObj) previewObj.SetPreview(Instantiate(asset));
			}
			EditorGUI.indentLevel++;

			EditorGUILayout.LabelField("BundleName", self.BundleName);
			EditorGUILayout.LabelField("AssetName", self.AssetName);

			if (GUILayout.Button("根据GUID修复引用"))
			{
				self.RefreshAssetBundleName();
				asset = EditorResourceMgr.LoadGameObject(self.BundleName, self.AssetName);
			}
			EditorGUI.indentLevel--;

			self.delayTime = EditorGUILayout.FloatField("DelayTime", self.delayTime);
			if (dirty || GUI.changed) EditorUtility.SetDirty(self);
		}
	}
}