using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
	public class ProgressIndicator : IDisposable
	{
		private float total = 1;
		private float current = 0;
		private double showInterval = 0.03;
		private string title;
		private double lastShowTime;

		public ProgressIndicator(string title)
		{
			this.title = title;
		}

		public void SetTotal(float total)
		{
			this.total = total;
		}

		public void AddProgress(float count = 1f)
		{
			current += count;
		}

		public void SetProgress(float progress)
		{
			current = progress;
		}

		public bool Show(string message)
		{
			var showTime = lastShowTime;
			lastShowTime = EditorApplication.timeSinceStartup;
			if (lastShowTime - showTime <= showInterval) return false;
			var progress = current / total;
			return EditorUtility.DisplayCancelableProgressBar(title, message, progress);
		}

		public bool Show(string format, params object[] args)
		{
			return Show(string.Format(format, args));
		}

		public bool ShowForce(string message)
		{
			var progress = current / total;
			return EditorUtility.DisplayCancelableProgressBar(title, message, progress);
		}

		public bool ShowForce(string format, params object[] args)
		{
			return ShowForce(string.Format(format, args));
		}

		public void Dispose()
		{
			EditorUtility.ClearProgressBar();
		}
	}
}