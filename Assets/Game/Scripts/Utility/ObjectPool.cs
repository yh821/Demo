using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPool<T> where T : new()
{
	private readonly Stack<T> mStack = new Stack<T>();
	private readonly UnityAction<T> mActionOnRelease;
	private int newCount = 0;

	public ObjectPool(UnityAction<T> actionOnRelease)
	{
		mActionOnRelease = actionOnRelease;
	}

	public T Get()
	{
		newCount++;
		return mStack.Count == 0 ? new T() : mStack.Pop();
	}

	public void Release(T t)
	{
		if (mStack.Count > 0 && ReferenceEquals(mStack.Peek(), t))
			Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
		mActionOnRelease?.Invoke(t);
		mStack.Push(t);
		newCount--;
	}
}