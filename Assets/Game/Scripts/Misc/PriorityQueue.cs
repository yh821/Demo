using System;
using System.Collections;
using System.Collections.Generic;

namespace Game
{
	public sealed class PriorityQueue<T> : IEnumerable<T>, IEnumerable
	{
		private IComparer<T> comparer;
		private T[] heap;

		public PriorityQueue() : this(null) { }
		public PriorityQueue(IComparer<T> comparer) : this(16, comparer) { }

		public PriorityQueue(int capacity, IComparer<T> comparer = null)
		{
			this.comparer = comparer ?? Comparer<T>.Default;
			heap = new T[capacity];
		}

		public int Count { get; private set; }

		public T this[int index] => heap[index];

		public IEnumerator<T> GetEnumerator()
		{
			return new Enumerator(heap, Count);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Clear()
		{
			Count = 0;
		}

		public void Push(T v)
		{
			if (Count >= heap.Length)
				Array.Resize(ref heap, Count * 2);
			heap[Count] = v;
			SiftUp(Count++);
		}

		public T Pop()
		{
			var obj = Top();
			heap[0] = heap[--Count];
			if (Count > 0) SiftDown(0);
			return obj;
		}

		public T Top()
		{
			if (Count > 0) return heap[0];
			throw new InvalidOperationException("The PriorityQueue is empty!");
		}

		private void SiftUp(int n)
		{
			var x = heap[n];
			for (int i = n / 2; n > 0 && comparer.Compare(x, heap[i]) > 0; i /= 2)
			{
				heap[n] = heap[i];
				n = i;
			}
			heap[n] = x;
		}

		private void SiftDown(int n)
		{
			var x = heap[n];
			for (int i = n * 2; i < Count; i *= 2)
			{
				if (i + 1 < Count && comparer.Compare(heap[i + 1], heap[i]) > 0) i++;
				if (comparer.Compare(x, heap[i]) < 0)
				{
					heap[n] = heap[i];
					n = i;
				}
				else break;
			}
			heap[n] = x;
		}

		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			private readonly T[] heap;
			private readonly int count;
			private int index;

			internal Enumerator(T[] heap, int count)
			{
				this.heap = heap;
				this.count = count;
				index = -1;
			}

			public bool MoveNext()
			{
				return index <= count && ++index < count;
			}

			public void Reset()
			{
				index = -1;
			}

			public T Current => heap[index];

			object IEnumerator.Current => Current;

			public void Dispose() { }
		}
	}
}