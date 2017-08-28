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
		#region Properties
		public ObservableCollection<Device> _Devices { get; set; }
		public string _NewDevice { get; set; }

		//Setup synchronization lock for writing to _Devices when updating items
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

		//TextBox property
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
		#endregion

		#region Commands

		public RelayCommand AddDeviceCommand { get; set; }
		public RelayCommand StartParserCommand { get; set; }
		public RelayCommand CloseCommand { get; set; }

		#endregion

		#region Methods
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
			
			//Start Analyze on separate thread
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

			//Build log for device Id
			parser.ParseEvents(deviceId, sr);

			//Get pattern count
			faultCount = parser.GetEventCount(deviceId);

			//Update count in list view
			var item = Devices.FirstOrDefault(x => x.DeviceId == deviceId);
			if (item != null)
				item.FaultCount = faultCount;
		}
		#endregion
	}
}
