using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	public sealed class OverrideOrderGroupMgr : Singleton<OverrideOrderGroupMgr>
	{
		private Dictionary<Canvas, OverrideOrderGroupItem> overrideOrderDic =
			new Dictionary<Canvas, OverrideOrderGroupItem>();

		private int groupCanvasOrderInterval = 60;
		Dictionary<StringIntKey, int> effectDic = new Dictionary<StringIntKey, int>(StringIntComparer.Default);
		private LinkedListNode<Action> updateHandler;

		public int GroupCanvasOrderInterval => groupCanvasOrderInterval;

		public void OnGameStartup()
		{
			updateHandler = Scheduler.AddFrameListener(Update);
		}

		public void OnGameStop()
		{
			overrideOrderDic.Clear();
			effectDic.Clear();
			if (updateHandler != null)
			{
				Scheduler.RemoveFrameListener(updateHandler);
				updateHandler = null;
			}
		}

		public void SetGroupCanvasOrderInterval(int groupCanvasOrderInterval)
		{
			if (groupCanvasOrderInterval <= 0) return;
			this.groupCanvasOrderInterval = groupCanvasOrderInterval;
		}

		public Canvas AddToGroup(IOverriderOrder overriderOrder)
		{
			var canvasScalers = ListPool<CanvasScaler>.Get();
			overriderOrder.GetTarget().GetComponentsInParent(true, canvasScalers);
			if (canvasScalers.Count <= 0)
			{
				ListPool<CanvasScaler>.Release(canvasScalers);
				return null;
			}

			var canvasScaler = canvasScalers[0];
			var canvas = canvasScaler.GetComponent<Canvas>();
			if (canvas == null)
			{
				ListPool<CanvasScaler>.Release(canvasScalers);
				return null;
			}
			OverrideOrderGroupItem group;
			if (overrideOrderDic.ContainsKey(canvas))
				group = overrideOrderDic[canvas];
			else
			{
				group = new OverrideOrderGroupItem
				{
					groupCanvas = canvas,
					groupOverrideOrder = canvas.sortingOrder
				};
				overrideOrderDic.Add(canvas, group);
			}
			group.orderSet.Add(overriderOrder);
			group.isDirty = true;
			ListPool<CanvasScaler>.Release(canvasScalers);

			return canvas;
		}

		public void RemoveFromGroup(Canvas canvas, IOverriderOrder overriderOrder)
		{
			if (canvas == null || overriderOrder == null) return;
			if (overrideOrderDic.TryGetValue(canvas, out var group))
			{
				group.orderSet.Remove(overriderOrder);
				if (group.orderSet.Count == 0)
					overrideOrderDic.Remove(canvas);
				group.isDirty = true;
			}
		}

		public void SetGroupCanvasDirty(Canvas canvas)
		{
			if (canvas == null) return;
			if (overrideOrderDic.TryGetValue(canvas, out var groupItem))
				groupItem.isDirty = true;
		}

		public void Update()
		{
			ObserveGroupCanvasOrderChange();
			RefreshAllOrder();
		}

		private void ObserveGroupCanvasOrderChange()
		{
			var iter = overrideOrderDic.GetEnumerator();
			while (iter.MoveNext())
			{
				var (_, value) = iter.Current;
				if (value.groupCanvas == null || value.groupOverrideOrder == value.groupCanvas.sortingOrder) continue;
				value.groupOverrideOrder = value.groupCanvas.sortingOrder;
				value.isDirty = true;
			}
		}

		private Dictionary<Transform, IOverriderOrder> filterDic;

		private void RefreshAllOrder()
		{
			var iter = overrideOrderDic.GetEnumerator();
			while (iter.MoveNext())
			{
				var (key, value) = iter.Current;
				if (key == null)
				{
					Debug.LogError("出现内存泄露!!!");
					overrideOrderDic.Remove(key);
					break;
				}

				if (!value.isDirty) continue;
				value.isDirty = false;

				var orderList = ListPool<IOverriderOrder>.Get();
				if (filterDic == null) filterDic = new Dictionary<Transform, IOverriderOrder>();

				RefreshOrder(value, filterDic, orderList);

				ListPool<IOverriderOrder>.Release(orderList);
				filterDic.Clear();
			}
		}

		private void RefreshOrder(OverrideOrderGroupItem orderGroup,
			Dictionary<Transform, IOverriderOrder> filterDic,
			List<IOverriderOrder> orderList)
		{
			effectDic.Clear();
			orderGroup.groupCanvas.overrideSorting = true;
			var sortingOrder = orderGroup.groupCanvas.sortingOrder;
			var maxOrder = sortingOrder + groupCanvasOrderInterval - 1;
			var order = sortingOrder + 1;
			SortOrderSet(orderGroup, filterDic, orderList);
			var oldOrder = order;
			var needResetOrder = false;
			foreach (var item in orderList)
			{
				var isNeedIncOrder = true;
				var curEffect = item as UIEffect;
				Canvas effectInCanvas = null;
				if (curEffect != null)
				{
					//相同的UIEffect, 并且Canvas相同, 使用相同的Order
					var canvasList = ListPool<Canvas>.Get();
					curEffect.GetComponentsInParent(true, canvasList);
					if (canvasList.Count > 0)
					{
						effectInCanvas = canvasList[0];
						var effectKey = new StringIntKey(ObjectNameMgr.Instance.GetObjectName(curEffect),
							effectInCanvas.GetInstanceID());
						if (effectDic.ContainsKey(effectKey))
						{
							oldOrder = order;
							needResetOrder = true;
							isNeedIncOrder = false;
							order = effectDic[effectKey];
						}
					}
					ListPool<Canvas>.Release(canvasList);
				}

				var incOrder = 0;
			}
		}

		//把每个canvas下的IOverrideOrder按照树节点顺序搜索出来，然后放进orderList
		private void SortOrderSet(OverrideOrderGroupItem orderGroup, Dictionary<Transform, IOverriderOrder> filterDic,
			List<IOverriderOrder> orderList)
		{
			foreach (var item in orderGroup.orderSet)
			{
				if (item == null) continue;
				var target = item.GetTarget();
				if (target == null || target.transform == null) continue;
				if (!filterDic.ContainsKey(target.transform)) filterDic.Add(target.transform, item);
			}
		}

		private void RecursionTransform(Transform transform, Dictionary<Transform, IOverriderOrder> filterDic,
			List<IOverriderOrder> orderList)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				var child = transform.GetChild(i);
				if (filterDic.ContainsKey(child)) orderList.Add(filterDic[child]);
				if (child.childCount > 0) RecursionTransform(child, filterDic, orderList);
			}
		}
	}

	public class OverrideOrderGroupItem
	{
		public Canvas groupCanvas;
		public int groupOverrideOrder;
		public HashSet<IOverriderOrder> orderSet = new HashSet<IOverriderOrder>();
		public bool isDirty;

		public bool IsInvalid()
		{
			return !groupCanvas;
		}
	}

	public interface IOverriderOrder
	{
		GameObject GetTarget();
		void SetOverrideOrder(int order, int orderInterval, int maxOrder, out int incOrder);
	}
}