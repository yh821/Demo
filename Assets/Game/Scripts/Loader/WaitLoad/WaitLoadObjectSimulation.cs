using UnityEngine;

namespace Game
{
	internal sealed class WaitLoadObjectSimulation : WaitLoadObject
	{
		private Object simulatedObject;
		private float simulateTime;

		public override bool keepWaiting
		{
			get
			{
				simulateTime -= Time.unscaledDeltaTime;
				return simulateTime > 0;
			}
		}

		internal WaitLoadObjectSimulation(Object simulatedObject, float simulateTime)
		{
			this.simulatedObject = simulatedObject;
			this.simulateTime = simulateTime;
		}

		internal WaitLoadObjectSimulation(string format, params object[] args)
		{
			Error = string.Format(format, args);
		}

		internal override bool Update()
		{
			return false;
		}

		public override Object GetObject()
		{
			return simulatedObject;
		}
	}
}