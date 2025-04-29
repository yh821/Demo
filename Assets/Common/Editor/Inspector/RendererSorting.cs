using UnityEditor;
using UnityEngine;

namespace Common.Editor
{
	[CustomEditor(typeof(MeshRenderer))]
	public class MeshRendererSorting : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var renderer = target as MeshRenderer;

			var layerNames = new string[SortingLayer.layers.Length];
			for (int i = 0; i < layerNames.Length; i++)
			{
				layerNames[i] = SortingLayer.layers[i].name;
			}

			var layerValue = SortingLayer.GetLayerValueFromID(renderer.sortingLayerID);
			layerValue = EditorGUILayout.Popup("Sorting Layer", layerValue, layerNames);

			var layer = SortingLayer.layers[layerValue];
			renderer.sortingLayerName = layer.name;
			renderer.sortingLayerID = layer.id;
			renderer.sortingOrder = EditorGUILayout.IntField("Order in Layer", renderer.sortingOrder);

			foreach (var mat in renderer.sharedMaterials)
			{
				mat.renderQueue = EditorGUILayout.IntField(mat.name, mat.renderQueue);
			}
		}
	}

	[CustomEditor(typeof(SkinnedMeshRenderer))]
	public class SkinnedMeshRendererSorting : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			var renderer = target as SkinnedMeshRenderer;

			var layerNames = new string[SortingLayer.layers.Length];
			for (int i = 0; i < layerNames.Length; i++)
			{
				layerNames[i] = SortingLayer.layers[i].name;
			}

			var layerValue = SortingLayer.GetLayerValueFromID(renderer.sortingLayerID);
			layerValue = EditorGUILayout.Popup("Sorting Layer", layerValue, layerNames);

			var layer = SortingLayer.layers[layerValue];
			renderer.sortingLayerName = layer.name;
			renderer.sortingLayerID = layer.id;
			renderer.sortingOrder = EditorGUILayout.IntField("Order in Layer", renderer.sortingOrder);

			foreach (var mat in renderer.sharedMaterials)
			{
				mat.renderQueue = EditorGUILayout.IntField(mat.name, mat.renderQueue);
			}
		}
	}
}