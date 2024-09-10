using System;
using UnityEngine;

namespace Game
{
	[Serializable]
	internal struct AnimatorAction
	{
		[SerializeField] private string eventName;
		[SerializeField] private string eventParam;

		internal string EventName => eventName;
		internal string EventParam => eventParam;

		internal void TriggerAction(Animator animator, AnimatorStateInfo stateInfo)
		{
			var component = animator.GetComponent<AnimatorEventDispatcher>();
			if (component == null) return;
			component.DispatchEvent(eventName, eventParam, stateInfo);
		}
	}
}