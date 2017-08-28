using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineLogger.Enums
{
	public enum State
	{
		Stage0,
		Stage1,
		Stage2,
		Stage3
	}

	public enum Command
	{
		Start,
		Cycle
	}
}
