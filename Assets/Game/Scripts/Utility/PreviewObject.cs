using System;
using UnityEditor;
using UnityEngine;

namespace Game
{
	[ExecuteInEditMode]
	public sealed class PreviewObject : MonoBehaviour
	{
		private float playingTime = 0;
		private double lastTime = -1;
		private GameObject preview;

		public bool SimulateInEditMode { get; set; }

		public void ClearPreview()
		{
			if (preview == null) return;
			var delObj = preview;
			preview = null;
			EditorApplication.delayCall += () => DestroyImmediate(delObj);
		}

		public void SetPreview(GameObject previewObj)
		{
			ClearPreview();
			preview = previewObj;
			preview.tag = "EditorOnly";
			preview.transform.SetParent(transform, false);
			if (SimulateInEditMode)
			{
				var componet = preview.GetComponent<EffectController>();
				if (componet != null)
				{
					componet.SimulateInit();
					componet.SimulateStart();
				}
				else
				{
					foreach (var child in preview.GetComponentsInChildren<ParticleSystem>())
						child.Simulate(0, false, true);
				}
			}
			SetHideFlags(preview, HideFlags.HideAndDontSave);
		}

		private void Awake()
		{
			hideFlags = HideFlags.HideAndDontSave;
			EditorApplication.playmodeStateChanged += () =>
			{
				if (!EditorApplication.isPlaying
				    && !EditorApplication.isPlayingOrWillChangePlaymode
				    && !EditorApplication.isCompiling
				    || preview == null) return;
				DestroyImmediate(preview);
				preview = null;
			};
		}

		private void OnDestroy()
		{
			if (preview == null) return;
			DestroyImmediate(preview);
			preview = null;
		}

		private void OnEnable()
		{
			if (Application.isPlaying) return;
			EditorApplication.update += UpdatePreview;
			lastTime = EditorApplication.timeSinceStartup;
			if (preview == null) return;
			preview.SetActive(true);
		}

		private void OnDisable()
		{
			if (Application.isPlaying) return;
			EditorApplication.update -= UpdatePreview;
			lastTime = EditorApplication.timeSinceStartup;
			if (preview == null) return;
			preview.SetActive(false);
		}

		private void UpdatePreview()
		{
			if (!SimulateInEditMode) return;
			var timeSinceStartup = EditorApplication.timeSinceStartup;
			var num1 = (float) (timeSinceStartup - lastTime);
			lastTime = timeSinceStartup;
			playingTime += num1;
			if (preview == null) return;
			var component = preview.GetComponent<EffectController>();
			if (component != null)
				component.SimulateDelta(playingTime, num1);
			else
			{
				var num2 = 0f;
				var particles = preview.GetComponentsInChildren<ParticleSystem>();
				foreach (var particle in particles)
				{
					var main = particle.main;
					if (num2 < main.duration)
					{
						main = particle.main;
						num2 = main.duration;
					}
				}
				foreach (var particle in particles)
					particle.Simulate(num1, false, false);
			}
		}

		private void SetHideFlags(GameObject obj, HideFlags flags)
		{
			obj.hideFlags = flags;
			for (int i = 0; i < obj.transform.childCount; i++)
				SetHideFlags(obj.transform.GetChild(i).gameObject, flags);
		}
	}
}