using UnityEngine;

namespace Game
{
	public abstract class WaitLoadAsset : CustomYieldInstruction
	{
		public string Error { get; protected set; }
		internal abstract bool Update();
	}
}