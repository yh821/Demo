using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	[DisallowMultipleComponent]
	public sealed class Scheduler : MonoBehaviour
	{
		private struct DelayTime
		{
			public float Time;
			public Action Task;
		}

		private static LinkedList<Action> frameTasks = new LinkedList<Action>();
		private static List<Action> executing = new List<Action>();
		private static List<Action> postTasks = new List<Action>();
		private static List<Action> nextFrameTasks = new List<Action>();
		private static List<DelayTime> delayTasks = new List<DelayTime>();
		private static Scheduler instance;

		public static Scheduler Instance
		{
			get
			{
				CheckInstance();
				return instance;
			}
		}

		public static void Clear()
		{
			frameTasks.Clear();
			executing.Clear();
			lock (postTasks) postTasks.Clear();
			nextFrameTasks.Clear();
			delayTasks.Clear();
		}

		public static LinkedListNode<Action> AddFrameListener(Action action)
		{
			return frameTasks.AddLast(action);
		}

		public static void RemoveFrameListener(LinkedListNode<Action> handle)
		{
			frameTasks.Remove(handle);
		}

		public static Coroutine RunCoroutine(IEnumerator coroutine)
		{
			return Instance.StartCoroutine(coroutine);
		}

		public static void StopShcCoroutine(Coroutine coroutine)
		{
			Instance.StopCoroutine(coroutine);
		}

		public static void PostTask(Action task)
		{
			lock (postTasks) postTasks.Add(task);
		}

		public static void Delay(Action task)
		{
			nextFrameTasks.Add(task);
		}

		public static void Delay(Action task, float time)
		{
			delayTasks.Add(new DelayTime()
			{
				Task = task,
				Time = Time.realtimeSinceStartup + time
			});
		}

		[RuntimeInitializeOnLoadMethod]
		private static void CheckInstance()
		{
			if (instance != null || !Application.isPlaying) return;
			var go = new GameObject(nameof(Scheduler), typeof(Scheduler));
			DontDestroyOnLoad(go);
			instance = go.GetComponent<Scheduler>();
		}

		private void Awake()
		{
			DontDestroyOnLoad(this);
		}

		private void OnDestroy()
		{
			Clear();
		}

		private void Update()
		{
			LinkedListNode<Action> next;
			for (var node = frameTasks.First; node != null; node = next)
			{
				next = node.Next;
				var action = node.Value;
				try
				{
					action();
				}
				catch (Exception e)
				{
					Debug.LogError(e.Message);
				}
			}
			lock (postTasks)
			{
				if (postTasks.Count > 0)
				{
					foreach (var task in postTasks)
						executing.Add(task);
					postTasks.Clear();
				}
			}
			if (nextFrameTasks.Count > 0)
			{
				foreach (var task in nextFrameTasks)
					executing.Add(task);
				nextFrameTasks.Clear();
			}
			if (delayTasks.Count > 0)
			{
				var now = Time.realtimeSinceStartup;
				delayTasks.RemoveAll(task =>
				{
					if (now < task.Time) return false;
					executing.Add(task.Task);
					return true;
				});
			}
			Executing();
		}

		private void Executing()
		{
			foreach (var action in executing)
			{
				try
				{
					action();
				}
				catch (Exception e)
				{
					Debug.LogError(e.Message);
				}
			}
			executing.Clear();
		}
	}
}