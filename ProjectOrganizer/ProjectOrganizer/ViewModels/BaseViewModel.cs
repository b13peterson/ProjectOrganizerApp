using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjectOrganizer.ViewModels
{
	public class BaseViewModel : INotifyPropertyChanged
	{

		private bool _isBusy;
		private string _title = string.Empty;

		public bool IsBusy
		{
			get => _isBusy;
			set => SetProperty(ref _isBusy, value);
		}

		public string Title
		{
			get => _title;
			set => SetProperty(ref _title, value);
		}

		protected bool SetProperty<TU>(ref TU privateField, TU value, [CallerMemberName] string propertyName = "", Action onChanged = null)
		{
			if (EqualityComparer<TU>.Default.Equals(privateField, value))
			{
				return false;
			}

			privateField = value;
			onChanged?.Invoke();
			OnPropertyChanged(propertyName);
			return true;
		}

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(sender: this, e: new PropertyChangedEventArgs(propertyName));
		#endregion
	}
}
