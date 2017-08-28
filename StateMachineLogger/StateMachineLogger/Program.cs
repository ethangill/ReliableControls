using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using StateMachineLogger.Enums;

namespace StateMachineLogger
{
	public class Program
	{
		class StateTransition
		{
			readonly State current;
			readonly Command command;
			readonly int stateId;

			public static int stateTransitionId;

			public StateTransition(State state, Command command)
			{
				this.current = state;
				this.command = command;
				this.stateId = Interlocked.Increment(ref stateTransitionId);
			}
		}

		List<Tuple<StateTransition, State>> transitions = new List<Tuple<StateTransition, State>>();
		public State CurrentState { get; private set; }
		public int timeMultiplier = 1; //seconds
		public string deviceId { get; set; }

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

		public class csvWriter
		{
			StreamWriter sw;
			static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			static string fileName = @"dataLog.csv";
			static string filePath = desktopPath + "\\" + fileName;


			public csvWriter()
			{
				if (!File.Exists(filePath))
				{
					sw = new StreamWriter(filePath);
					sw.Close();
				}
				if (new FileInfo(filePath).Length == 0)
				{
					writeHeader();
				}
			}
			public bool writeHeader()
			{
				sw = new StreamWriter(filePath, true);
				if (sw != null)
				{
					DateTime currentDateTime = DateTime.Now;
					sw.WriteLine("Timestamp,Value,Device ID");
					sw.Flush();
					sw.Close();
					return true;
				}
				else
				{
					return false;
				}
			}
			public bool writeLog(State state, string deviceId)
			{
				sw = new StreamWriter(filePath, true);
				if (sw != null)
				{
					DateTime currentDateTime = DateTime.Now;
					sw.WriteLine("{0},{1},{2}", currentDateTime.ToString("u"), (int)state, deviceId);
					sw.Flush();
					sw.Close();
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		static void Main(string[] args)
		{

			if(args.Length == 0)
			{
				Console.WriteLine("Please enter a Device ID");
				return;
			}

			Program p = new Program(args[0].ToString());

			csvWriter writer = new csvWriter();

			//start transition
			Console.WriteLine("Beginning Program");
			Console.WriteLine("Current State = {0}", p.CurrentState);
			Console.WriteLine("Starting process");
			p.moveNext(Command.Start);

			writer.writeLog(p.CurrentState, p.deviceId);
			Console.WriteLine("Current state = " + p.CurrentState);

			while (true)
			{
				Thread.Sleep(1000 * 1); // one second delay

				if (p.CurrentState == State.Stage3)
				{
					Random randTime = new Random();
					int randTimeVal = randTime.Next(1, 7);
					Thread.Sleep(1000 * p.timeMultiplier * randTimeVal);
				}

				p.moveNext(Command.Cycle);
				writer.writeLog(p.CurrentState, p.deviceId);
				Console.WriteLine("Current state = " + p.CurrentState);
			}
		}
	}
}
