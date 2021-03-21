using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.Models;
using ProjectOrganizer.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProjectOrganizer.Views
{
	public abstract class AddItemPage<T> : ContentPage where T : ISQLiteDataItem
	{
		protected Timer entryTimer;

		protected override void OnDisappearing()
		{
			entryTimer?.Close();
			entryTimer = null;
		}

		/// <summary>
		/// Saves the ViewModel item
		/// Sends the Reload list message
		/// Pops off the navigation page
		/// </summary>
		protected async void Save_Clicked(object sender, EventArgs e)
		{
			MessagingCenter.Send(sender: new Message(), message: $"Reload{typeof(T).Name}s");
			await MainThread.InvokeOnMainThreadAsync(async () => await Navigation.PopAsync());
		}

		protected async void Cancel_Clicked(object sender, EventArgs e) => await Navigation.PopAsync().ConfigureAwait(false);

		protected async void OnDataEntryChanged(object sender, TextChangedEventArgs e)
		{
			if (e.NewTextValue == "")
			{
				await ValidateItem().ConfigureAwait(false);
				return;
			}

			if (entryTimer?.Enabled ?? false)
			{
				entryTimer?.Stop();
			} else
			{
				entryTimer = new Timer(600) { AutoReset = false };
				entryTimer.Elapsed += async delegate
				{
					await ValidateItem().ConfigureAwait(false);
					RefreshCommands();
				};
			}
			entryTimer.Start();
		}

		protected abstract Task ValidateItem();

		protected abstract void RefreshCommands();
	}

	[DesignTimeVisible(false), XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class AddClientPage : AddItemPage<Client>
	{
		public AddClientViewModel viewModel;

		public AddClientPage(AddClientViewModel clientVm)
		{
			BindingContext = viewModel = clientVm;
			InitializeComponent();
		}

		protected override async Task ValidateItem() => await viewModel.ValidateItem().ConfigureAwait(false);
		protected override void RefreshCommands() => MainThread.BeginInvokeOnMainThread(() => viewModel.RefreshCommands());
	}

	[DesignTimeVisible(false), XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class AddProjectPage : AddItemPage<Project>
	{
		public AddProjectViewModel viewModel;
		public AddProjectPage(AddProjectViewModel projectVm)
		{
			BindingContext = viewModel = projectVm;
			InitializeComponent();
		}
		protected override async Task ValidateItem() => await viewModel.ValidateItem().ConfigureAwait(false);
		protected override void RefreshCommands() => MainThread.BeginInvokeOnMainThread(() => viewModel.RefreshCommands());
	}

	[DesignTimeVisible(false), XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class AddWorkshiftPage : AddItemPage<Workshift>
	{
		public AddWorkshiftViewModel viewModel;
		public AddWorkshiftPage(AddWorkshiftViewModel workshiftVm)
		{
			viewModel = workshiftVm;
			InitializeComponent();

			BindingContext = viewModel;

		}

		private void OnProjectChanged(object sender, EventArgs e)
		{
			if ((sender is Picker picker) && (picker.SelectedItem is Project project))
			{
				viewModel.SetProjectOfWorkshift(project);
			}
		}

		private async void ValidateItem(object sender, EventArgs e) {
			await viewModel.ValidateItem().ConfigureAwait(false);
		}

		private async void OnDeleteClicked(object sender, EventArgs e){
			if (await DisplayAlert(title: $"Delete Workshift", message: $"Are you sure you want to delete the Workshift?", accept: "Yes", cancel: "No"))
			{
				await viewModel.DeleteItemAsync().ConfigureAwait(false);
				Device.BeginInvokeOnMainThread(async () => await Navigation.PopAsync().ConfigureAwait(false));
			}
		}
		protected override async Task ValidateItem() => await viewModel.ValidateItem().ConfigureAwait(false);
		protected override void RefreshCommands() => MainThread.BeginInvokeOnMainThread(() => viewModel.RefreshCommands()); 
		
		protected async void OnDateTimeEntryChanged(object sender, EventArgs e)
		{
			await ValidateItem().ConfigureAwait(false);
			RefreshCommands();
		}
	}
}