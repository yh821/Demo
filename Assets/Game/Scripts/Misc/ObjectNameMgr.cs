using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class ObjectNameMgr : Singleton<ObjectNameMgr>
	{
		private const int MAX_SLOT_SIZE = 8192;
		private Dictionary<int, int> slotIndexDic = new Dictionary<int, int>(MAX_SLOT_SIZE);
		private string[] nameSlots = new string[MAX_SLOT_SIZE];
		private int curSlotIndex = 0;

		public string GetObjectName(Object obj)
		{
			var id = obj.GetInstanceID();
			if (!slotIndexDic.TryGetValue(id, out var slotIndex))
			{
				slotIndex = curSlotIndex;
				slotIndexDic.Add(id, slotIndex);
				nameSlots[curSlotIndex++] = obj.name;
				if (curSlotIndex >= MAX_SLOT_SIZE)
				{
					slotIndexDic.Clear();
					curSlotIndex = 0;
				}
			}
			return nameSlots[slotIndex];
		}

		public void SetObjectName(Object obj, string name)
		{
			var id = obj.GetInstanceID();
			if (slotIndexDic.TryGetValue(id, out var slotIndex))
			{
				nameSlots[slotIndex] = name;
			}
			obj.name = name;
		}
	}
}