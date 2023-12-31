﻿using UnityEditor;
using UnityEngine;

namespace Common
{
	public static class EditorHelper
	{
		#region 实例化预设

		public const string UGUI_ROOT_NAME = "GameRoot/BaseView/Root";

		[MenuItem("Tools/实例化预设 #&s")]
		public static void InstantiatePrefab()
		{
			var select = Selection.activeObject;
			if (select != null && PrefabUtility.GetPrefabType(select) == PrefabType.Prefab)
			{
				var target = GetSceneObject(select.name);
				if (target == null)
				{
					target = PrefabUtility.InstantiatePrefab(select) as GameObject;
					var isHaveCanvas = target.GetComponentInChildren<UnityEngine.UI.CanvasScaler>() != null;
					if (!isHaveCanvas && TempParent != null)
						target.transform.SetParent(TempParent, false);
					target.name = select.name;
					Selection.activeObject = target;
				}
			}
		}

		static Transform TempParent
		{
			get
			{
				if (mTempParent == null)
				{
					var go = GameObject.Find(UGUI_ROOT_NAME);
					if (go != null)
						mTempParent = go.transform;
				}

				return mTempParent;
			}
		}

		static Transform mTempParent = null;

		static GameObject GetSceneObject(string name)
		{
			if (TempParent != null)
			{
				var t = TempParent.Find(name);
				if (t != null)
					return t.gameObject;
			}

			return GameObject.Find(name);
		}

		#endregion

	}
}