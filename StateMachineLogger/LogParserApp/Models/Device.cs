using System.ComponentModel;

namespace LogParserApp.Models
{
	public class Device : INotifyPropertyChanged
	{
		private string _DeviceId;
		private int _FaultCount;

		public string DeviceId
		{
			get
			{
				return _DeviceId;
			}
			set
			{
				if (_DeviceId != value)
				{
					_DeviceId = value;
					RaisePropertyChanged("DeviceId");
				}
			}
		}
		public int FaultCount
		{
			get
			{
				return _FaultCount;
			}
			set
			{
				if (_FaultCount != value)
				{
					_FaultCount = value;
					RaisePropertyChanged("FaultCount");
				}
			}
		}

		void RaisePropertyChanged(string prop)
		{
			if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
