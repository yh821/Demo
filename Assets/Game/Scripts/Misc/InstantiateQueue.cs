using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public sealed class InstantiateQueue
	{
		private struct Task
		{
			public GameObject Prefab;
			public Action<GameObject> Callback;
			public int Priority;
		}

		private class TaskComparer : IComparer<Task>
		{
			private static TaskComparer defaultInstance;

			public static TaskComparer Default => defaultInstance ??= new TaskComparer();

			public int Compare(Task x, Task y)
			{
				return x.Priority.CompareTo(y.Priority);
			}
		}

		private int instantiateCountPerFrame = 1;
		PriorityQueue<Task> pendingTasks = new PriorityQueue<Task>(TaskComparer.Default);
		private LinkedListNode<Action> updateHandle;
		private int instantiateCount;

		public static InstantiateQueue Global { get; set; }

		public int InstantiateCountPerFrame
		{
			get => instantiateCountPerFrame;
			set => instantiateCountPerFrame = value;
		}

		public InstantiateQueue()
		{
			updateHandle = Scheduler.AddFrameListener(Update);
		}

		~InstantiateQueue()
		{
			Scheduler.RemoveFrameListener(updateHandle);
			updateHandle = null;
		}

		internal void Instantiate(GameObject prefab, int priority, Action<GameObject> callback)
		{
			if (instantiateCount < InstantiateCountPerFrame)
			{
				var go = Singleton<PrefabPool>.Instance.Instantiate(prefab);
				instantiateCount++;
				callback(go);
			}
			else
				pendingTasks.Push(new Task
				{
					Prefab = prefab, Callback = callback
				});
		}

		private void Update()
		{
			try
			{
				while (pendingTasks.Count > 0 && instantiateCount < InstantiateCountPerFrame)
				{
					var task = pendingTasks.Pop();
					var go = Singleton<PrefabPool>.Instance.Instantiate(task.Prefab);
					instantiateCount++;
					task.Callback(go);
				}
			}
			finally
			{
				instantiateCount = 0;
			}
		}
	}
}