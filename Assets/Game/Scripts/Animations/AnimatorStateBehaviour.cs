using UnityEngine;

namespace Game
{
	public class AnimatorStateBehaviour : StateMachineBehaviour
	{
		[SerializeField] private AnimatorAction[] enterActions;
		[SerializeField] private AnimatorAction[] exitActions;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (enterActions == null) return;
			foreach (var action in enterActions)
				action.TriggerAction(animator, stateInfo);
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (exitActions == null) return;
			foreach (var action in exitActions)
				action.TriggerAction(animator, stateInfo);
		}
	}
}