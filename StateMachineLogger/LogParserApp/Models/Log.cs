using System;
using System.Threading;

namespace LogParserApp.Models
{
	public class Log
	{
		public string deviceId { get; set; }
		public int id { get; set; }
		public DateTime date { get; set; }
		public string state { get; set; }

		public static int globalLogId;

		public Log(string newDeviceId, DateTime newDate, string newState)
		{
			this.deviceId = newDeviceId;
			this.date = newDate;
			this.state = newState;
			this.id = Interlocked.Increment(ref globalLogId);
		}
	}
}
