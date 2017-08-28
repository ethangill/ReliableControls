using LogParserApp.Helpers;
using LogParserApp.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Data;

namespace LogParserApp.ViewModels
{
	public class ViewModelMain : ViewModelBase
	{
		public ObservableCollection<Device> _Devices { get; set; }
		public string _NewDevice { get; set; }
		private object _lock = new object();

		public string NewDevice
		{
			get
			{
				return _NewDevice;
			}
			set
			{
				_NewDevice = value;
				NotifyPropertyChanged("NewDevice");
			}
		}

		public ObservableCollection<Device> Devices
		{
			get
			{
				return _Devices;
			}
			set
			{
				_Devices = value;
				NotifyPropertyChanged("Devices");
			}
		}

		string _TextProperty;
		public string TextProperty
		{
			get
			{
				return _TextProperty;
			}
			set
			{
				if (_TextProperty != value)
				{
					_TextProperty = value;
					RaisePropertyChanged("TextProperty");
				}
			}
		}

		public RelayCommand AddDeviceCommand { get; set; }
		public RelayCommand StartParserCommand { get; set; }
		public RelayCommand CloseCommand { get; set; }

		public ViewModelMain()
		{
			Devices = new ObservableCollection<Device>();
			AddDeviceCommand = new RelayCommand(AddDevice);
			StartParserCommand = new RelayCommand(StartParser);

			BindingOperations.EnableCollectionSynchronization(_Devices, _lock);
		}

		void AddDevice(object parameter)
		{
			if (parameter == null)
				return;
			Devices.Add(new Device { DeviceId = parameter.ToString(), FaultCount = 0 });
		}

		void StartParser(object parameter)
		{
			if (parameter == null)
				return;

			bool alreadyFound = Devices.Any(x => x.DeviceId == parameter.ToString());
			if (alreadyFound)
			{
				TextProperty = string.Empty;
				return;
			}

			Devices.Add(new Device { DeviceId = parameter.ToString(), FaultCount = 0 });
			TextProperty = string.Empty;


			Thread newThread = new Thread(() => AnalyzeLog(parameter.ToString()));
			newThread.Start();

		}

		private void AnalyzeLog(string deviceId)
		{
			string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			string fileName = @"dataLog.csv";
			string filePath = desktopPath + "\\" + fileName;
			int faultCount = 0;

			if (!File.Exists(filePath))
				return;
			LogParser parser = new LogParser();
			StreamReader sr = new StreamReader(filePath);
			parser.ParseEvents(deviceId, sr);


			faultCount = parser.GetEventCount(deviceId);

			var item = Devices.FirstOrDefault(x => x.DeviceId == deviceId);
			if (item != null)
				item.FaultCount = faultCount;
		}
	}
}
