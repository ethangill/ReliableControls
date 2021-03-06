﻿using System.ComponentModel;

namespace LogParserApp.ViewModels
{
	public class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		protected void RaisePropertyChanged(string prop)
		{
			if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
		}
	}
}
