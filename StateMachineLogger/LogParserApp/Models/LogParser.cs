using RC.CodingChallenge;
using System;
using System.IO;
using System.Collections.Generic;

namespace LogParserApp.Models
{
	public class LogParser : IEventCounter
	{

		public Dictionary<int, Log> logs = new Dictionary<int, Log>();
		private int logId;

		private string state3 = "3";
		private string state2 = "2";
		private string state1 = "1";
		private string state0 = "0";

		//IEventCounter implementation
		public int GetEventCount(string deviceID)
		{
			if (logs.Count == 0)
				return 0;

			TimeSpan timeSpan = new TimeSpan();
			int faultCount = 0;
			int logsToCount = logs.Count - 3;

			for (int i = 0; i < logsToCount; i++)
			{
				//Check for device ID and state3 match
				if (logs[i].deviceId == deviceID && logs[i].state == state3)
				{
					//Check for device ID and state2 match in next 
					if (logs[i + 1].deviceId == deviceID && logs[i + 1].state == state2)
					{
						//Measure how long the device has been in state3 for
						//This has been changed to seconds to allow for faster testing
						timeSpan = logs[i + 1].date - logs[i].date;
						if (timeSpan.Seconds > 5)
						{
							//Cycle through subsequent records to check for state2 and state3 occurances
							for (int j = i + 2; j < logsToCount; j++)
							{
								if (logs[j].deviceId == deviceID && (logs[j].state == state2 || logs[j].state == state3))
								{
									//Check for transition to state0
									if (logs[j + 1].deviceId == deviceID && logs[j + 1].state == state0)
									{
										faultCount++;
										//Set i iterator to new index to move past previous sequenced states
										i = j + 1;
										break;
									}
								}
								else
								{
									break;
								}
							}
							continue;
						}
					}
				}
			}

			return faultCount;
		}

		public void ParseEvents(string deviceID, StreamReader eventLog)
		{
			FileStream fileStream = eventLog.BaseStream as FileStream;
			string fileName;

			if (fileStream != null)
				fileName = fileStream.Name;
			else
				return;

			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (StreamReader sr = new StreamReader(fs))
				{
					while (!sr.EndOfStream)
					{
						var line = sr.ReadLine();
						var values = line.Split(',');

						string newDeviceId = values[2];
						DateTime date = DateTime.Now;
						DateTime.TryParse(values[0], out date);
						string state = values[1];

						if (newDeviceId == deviceID)
						{
							Log newLog = new Log(newDeviceId, date, state);

							logs.Add(this.logId, newLog);
							this.logId++;
						}
					}
				}
			}
		}
	}
}
