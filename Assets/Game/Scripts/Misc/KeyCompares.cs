using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public struct StringIntKey
	{
		public string strValue;
		public int intValue;

		public StringIntKey(string strValue, int intValue)
		{
			this.strValue = strValue;
			this.intValue = intValue;
		}
	}

	public class StringIntComparer : IEqualityComparer<StringIntKey>
	{
		private static volatile StringIntComparer defaultComparer;

		public static StringIntComparer Default => defaultComparer ??= new StringIntComparer();

		public bool Equals(StringIntKey x, StringIntKey y)
		{
			return x.strValue == y.strValue && x.intValue == y.intValue;
		}

		public int GetHashCode(StringIntKey obj)
		{
			var hashCode = obj.strValue.GetHashCode();
			hashCode ^= obj.intValue.GetHashCode();
			return hashCode;
		}
	}
}