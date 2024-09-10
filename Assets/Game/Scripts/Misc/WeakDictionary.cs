using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
	public class WeakDictionary<K, V>
	{
		private Dictionary<K, WeakReference> dict = new Dictionary<K, WeakReference>();

		public V this[K key]
		{
			get
			{
				var weakRef = dict[key];
				return weakRef.Target != null ? (V) weakRef.Target : default;
			}
			set => Add(key, value);
		}

		private ICollection<K> Keys => dict.Keys;

		private ICollection<V> Values
		{
			get { return dict.Keys.Select(key => (V) dict[key].Target).ToList(); }
		}

		private void Add(K key, V value)
		{
			if (dict.ContainsKey(key))
			{
				if (dict[key].Target != null)
					throw new ArgumentException("key exists!");
				dict[key].Target = value;
			}
			else
			{
				var weakRef = new WeakReference(value);
				dict.Add(key, weakRef);
			}
		}

		public void TryRemove(K key)
		{
			if (!dict.ContainsKey(key)) return;
			dict.Remove(key);
		}

		public bool ContainsKey(K key)
		{
			return dict.ContainsKey(key);
		}

		public bool TryGetValue(K key, out V value)
		{
			if (dict.TryGetValue(key, out var weakRef))
			{
				if (weakRef.Target != null)
				{
					value = (V) weakRef.Target;
					return true;
				}
			}
			value = default;
			return false;
		}

		public void GC()
		{
			var kList = (from kv in dict where kv.Value.Target == null select kv.Key).ToList();
			foreach (var key in kList)
				dict.Remove(key);
		}
	}
}