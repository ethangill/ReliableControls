using System;
using System.IO;
using StateMachineLogger.Enums;

namespace StateMachineLogger
{
	public class CsvWriter
	{
		StreamWriter sw;
		static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		static string fileName = @"dataLog.csv";
		static string filePath = desktopPath + "\\" + fileName;


		public CsvWriter()
		{
			if (!File.Exists(filePath))
			{
				sw = new StreamWriter(filePath);
				sw.Close();
			}
			if (new FileInfo(filePath).Length == 0)
			{
				WriteHeader();
			}
		}
		public bool WriteHeader()
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
		public bool WriteLog(State state, string deviceId)
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
}
