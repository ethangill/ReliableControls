using StateMachineLogger.Enums;
using System.Threading;

namespace StateMachineLogger
{
	public class StateTransition
	{
		private State current;
		private Command command;
		private int stateId;

		public static int stateTransitionId;

		public StateTransition(State state, Command command)
		{
			this.current = state;
			this.command = command;
			//Auto Increment
			this.stateId = Interlocked.Increment(ref stateTransitionId);
		}
	}
}
