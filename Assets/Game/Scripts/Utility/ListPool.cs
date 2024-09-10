using System.Collections.Generic;

public static class ListPool<T>
{
	private static readonly ObjectPool<List<T>> SListPool = new ObjectPool<List<T>>(l => l.Clear());

	public static List<T> Get()
	{
		return SListPool.Get();
	}

	public static void Release(List<T> list)
	{
		SListPool.Release(list);
	}
}