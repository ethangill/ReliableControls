using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using StateMachineLogger.Enums;

namespace StateMachineLogger
{
	public class Program
	{
		#region Properties
		public List<Tuple<StateTransition, State>> transitions = new List<Tuple<StateTransition, State>>();
		public State CurrentState { get; private set; }

		//Represents seconds. Change to 60 for minutes. Must also change detection method in LogParser
		public int timeMultiplier = 1;
		public string deviceId { get; set; }

		#endregion

		#region Methods
		public Program(string id)
		{
			CurrentState = State.Stage0;
			deviceId = id;
			transitions = new List<Tuple<StateTransition, State>>();

			transitions.Add(new Tuple<StateTransition, State>(new StateTransition(State.Stage0, Command.Start), State.Stage1));
			transitions.Add(new Tuple<StateTransition, State>(new StateTransition(State.Stage0, Command.Start), State.Stage1));
			transitions.Add(new Tuple<StateTransition, State>(new StateTransition(State.Stage1, Command.Cycle), State.Stage2));
			transitions.Add(new Tuple<StateTransition, State>(new StateTransition(State.Stage1, Command.Cycle), State.Stage3));
			transitions.Add(new Tuple<StateTransition, State>(new StateTransition(State.Stage2, Command.Cycle), State.Stage0));
			transitions.Add(new Tuple<StateTransition, State>(new StateTransition(State.Stage2, Command.Cycle), State.Stage1));
			transitions.Add(new Tuple<StateTransition, State>(new StateTransition(State.Stage2, Command.Cycle), State.Stage3));
			transitions.Add(new Tuple<StateTransition, State>(new StateTransition(State.Stage3, Command.Cycle), State.Stage0));
			transitions.Add(new Tuple<StateTransition, State>(new StateTransition(State.Stage3, Command.Cycle), State.Stage1));
			transitions.Add(new Tuple<StateTransition, State>(new StateTransition(State.Stage3, Command.Cycle), State.Stage2));
		}

		public State getNext(Command command)
		{
			StateTransition transition = new StateTransition(CurrentState, command);

			if (command == Command.Start)
			{
				return State.Stage0;
			}
			else
			{
				Random rand = new Random();
				int transIndex = rand.Next(1, transitions.Count);
				return transitions[transIndex].Item2;
			}
		}

		public State moveNext(Command command)
		{
			CurrentState = getNext(command);
			return CurrentState;
		}
		#endregion

		#region Main Console App
		//Main Console
		//Takes in one argument for Device ID
		static void Main(string[] args)
		{

			if(args.Length == 0)
			{
				Console.WriteLine("Please enter a Device ID");
				return;
			}

			Program p = new Program(args[0].ToString());

			CsvWriter writer = new CsvWriter();

			//Start transition
			Console.WriteLine("Beginning Program");
			Console.WriteLine("Current State = {0}", p.CurrentState);
			Console.WriteLine("Starting process");
			p.moveNext(Command.Start);

			writer.WriteLog(p.CurrentState, p.deviceId);
			Console.WriteLine("Current state = " + p.CurrentState);

			while (true)
			{
				//One second delay
				Thread.Sleep(1000 * 1);

				if (p.CurrentState == State.Stage3)
				{
					//Randomize State3 Hold - Between 1 and 7 time units
					Random randTime = new Random();
					int randTimeVal = randTime.Next(1, 7);
					Thread.Sleep(1000 * p.timeMultiplier * randTimeVal);
				}

				p.moveNext(Command.Cycle);
				writer.WriteLog(p.CurrentState, p.deviceId);
				Console.WriteLine("Current state = " + p.CurrentState);
			}
		}

		#endregion
	}
}