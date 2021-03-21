using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using ProjectOrganizer.Models;
using ProjectOrganizer.Repositories;
using ProjectOrganizer.ViewModels;
using ProjectOrganizer.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProjectOrganizer
{
	[DesignTimeVisible(false)]
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomeListPage : ContentPage
	{
		private Timer searchTimer;
		private HomeViewModel viewModel;
		public HomeListPage() => InitializeComponent();

		protected override void OnAppearing()
		{
			base.OnAppearing();
			BindingContext = viewModel = new HomeViewModel();
			viewModel.LoadItemsCommand.Execute(null);
		}

		private async void OnClientsClicked(object sender, EventArgs e) => await Navigation.PushAsync(new ClientsListPage()).ConfigureAwait(false);

		private async void OnWorkshiftsClicked(object sender, EventArgs e) => await Navigation.PushAsync(new WorkshiftsListPage(new ListViewModel<Workshift>())).ConfigureAwait(false);

		private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			Debug.WriteLine("MainPage OnItemSelected called.");
			if (!(e.SelectedItem is Project item))
			{
				return;
			}

			if (viewModel.CurrentWorkshiftProject == item)
			{
				return;
			}

			DateTime now = DateTime.Now;
			await viewModel.ChangeCurrentWorkshift(new Workshift { ProjectId = item.Id, Start = now, End = now, Name = $"Project {item.Name} Workshift {now:O}" }).ConfigureAwait(false);
			viewModel.CurrentWorkshiftProject = item;
		}

		private async void EndCurrentWorkshift(object sender, EventArgs e) => await viewModel.ChangeCurrentWorkshift(null).ConfigureAwait(false);

		// Adapted from Rosenberg, A. (2016, August) How to run code fater a delay in Xamarin Android. Retrieved from https://forums.xamarin.com/discussion/74307/how-to-run-code-after-a-delay-in-xamarin-android
		private void OnProjectSearchTextChanged(object sender, TextChangedEventArgs e)
		{
			if (searchTimer?.Enabled ?? false)
			{
				searchTimer?.Stop();
			} else
			{
				searchTimer = new Timer(600) { AutoReset = false };
				searchTimer.Elapsed += delegate
				{
					viewModel.LoadItemsCommand.Execute(null);
				};
			}
			searchTimer.Start();
		}

		protected override void OnDisappearing()
		{
			searchTimer?.Close();
			searchTimer = null;
		}
		private async void OnLoadTestDataClicked(object obj, EventArgs e)
		{
			if (await DisplayAlert(title: "Load Test Data", message: "Are you sure you want to delete all data and replace it with test data?", accept: "Yes", cancel: "No"))
			{
				await App.GenerateTestData().ConfigureAwait(false);
				viewModel.LoadItemsCommand.Execute(null);
			}
		}

		private async void OnDeleteButtonClicked(object sender, EventArgs e)
		{
			if (await DisplayAlert(title: "Delete All Data", message: "Are you sure you want to delete all data?", accept: "Yes", cancel: "No"))
			{
				var clientRepo = new ClientRepository();
				var clients = await clientRepo.GetAllItemsAsync().ConfigureAwait(false);
				foreach (var client in clients)
				{
					await clientRepo.DeleteItemAsync(client.Id).ConfigureAwait(false);
				}

				var projectRepo = new ProjectRepository();
				var projects = await projectRepo.GetAllItemsAsync().ConfigureAwait(false);
				foreach (var project in projects)
				{
					await projectRepo.DeleteItemAsync(project.Id).ConfigureAwait(false);
				}

				var workshiftRepo = new WorkshiftRepository();
				var workshifts = await workshiftRepo.GetAllItemsAsync().ConfigureAwait(false);
				foreach (var workshift in workshifts)
				{
					await workshiftRepo.DeleteItemAsync(workshift.Id).ConfigureAwait(false);
				}

				viewModel.LoadItemsCommand.Execute(null);
			}
		}
	}
}