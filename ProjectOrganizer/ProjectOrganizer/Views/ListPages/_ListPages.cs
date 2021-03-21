using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.Models;
using ProjectOrganizer.ViewModels;
using Xamarin.Forms;
using static ProjectOrganizer.Models.SQLFactory;


namespace ProjectOrganizer.Views
{
	public abstract class ItemsListPage<T> : ContentPage where T : ISQLiteDataItem
	{
		protected readonly ListViewModel<T> ListVm;
		protected ItemsListPage(ListViewModel<T> viewModel = null) => BindingContext = ListVm = viewModel ?? new ListViewModel<T>();

		protected override void OnAppearing()
		{
			Debug.WriteLine($"{GetType().Name} OnAppearing called.");
			base.OnAppearing();
			if (!ListVm.Items.Any())
			{
				ListVm.LoadItemsCommand.Execute(null);
			}
		}

		protected async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			Debug.WriteLine($"{GetType().Name} OnItemSelected called.");
			if (!(e.SelectedItem is T item))
			{
				return;
			}
			DeselectListViewItem();
			await NavigateToItemSelectedPage(item).ConfigureAwait(false);
		}
		protected virtual async void AddItem_Clicked(object sender, EventArgs e) => await NavigateToEditPage((T)NewItem(typeof(T))).ConfigureAwait(false);

		protected virtual async Task NavigateToEditPage(T item) => await Navigation.PushAsync(NewAddPage(typeof(T), NewAddViewModel(item))).ConfigureAwait(false);

		protected virtual async Task NavigateToDetailsPage(T item) => await Navigation.PushAsync(NewDetailsPage(typeof(T), NewDetailsViewModel(item))).ConfigureAwait(false);

		protected virtual async Task NavigateToItemSelectedPage(T item) => await NavigateToDetailsPage(item).ConfigureAwait(false);

		protected abstract void DeselectListViewItem();
	}

	[DesignTimeVisible(false)]
	public partial class ClientsListPage : ItemsListPage<Client>
	{

		public ClientsListPage(ListViewModel<Client> viewModel = null) : base(viewModel) => InitializeComponent();

		protected override void DeselectListViewItem() => ItemsListView.SelectedItem = null;
	}

	[DesignTimeVisible(false)]
	public partial class WorkshiftsListPage : ItemsListPage<Workshift>
	{

		public WorkshiftsListPage(ListViewModel<Workshift> viewModel) : base(viewModel) => InitializeComponent();

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (ListVm.Items.Count == 0)
			{
				ListVm.LoadItemsCommand.Execute(null);
			}
		}
		protected override void DeselectListViewItem() => ItemsListView.SelectedItem = null;
		protected override async Task NavigateToItemSelectedPage(Workshift item) => await Navigation.PushAsync(new AddWorkshiftPage(new AddWorkshiftViewModel(item))).ConfigureAwait(false);
	}
}
