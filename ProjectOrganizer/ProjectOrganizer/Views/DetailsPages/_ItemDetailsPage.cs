using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.Models;
using ProjectOrganizer.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static ProjectOrganizer.Models.SQLFactory;


namespace ProjectOrganizer.Views
{
	public abstract class ItemDetailsPage<TMainItem, TListItem> : ItemsListPage<TListItem>
	where TMainItem : ISQLiteDataItem
	where TListItem : ISQLiteDataItem
	{

		protected DetailsViewModel<TMainItem> detailsVm;

		protected ItemDetailsPage(DetailsViewModel<TMainItem> detailsVm) : base(new ListViewModel<TListItem>(detailsVm.Item))
		{
			if (detailsVm == null)
			{
				throw new NullReferenceException($"Null DetailsVm loaded into {GetType().Name}.");
			}
			Debug.WriteLine($"{GetType().Name} constructor running.");
			BindingContext = this.detailsVm = detailsVm;
		}

		protected async void EditItem_Clicked(object sender, EventArgs e) => await NavigateToEditPage(detailsVm.Item);

		protected async void DeleteItem_Clicked(object sender, EventArgs e)
		{
			if (await DisplayAlert(title: $"Delete {detailsVm.Item.GetType().Name}", message: $"Are you sure you want to delete the {detailsVm.Item.GetType().Name}?", accept: "Yes", cancel: "No"))
			{
				await detailsVm.DeleteItemAsync().ConfigureAwait(false);
				Device.BeginInvokeOnMainThread(async () => await Navigation.PopAsync().ConfigureAwait(false));
			}
		}

		protected async Task NavigateToEditPage(TMainItem item) => await Navigation.PushAsync(NewAddPage(typeof(TMainItem), NewAddViewModel(item))).ConfigureAwait(false);
	}

	[DesignTimeVisible(false), XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class ClientDetailsPage : ItemDetailsPage<Client, Project>
	{
		/// <summary>
		/// Contains client information and a list of projects for the client.
		/// </summary>
		/// <param name="detailsVm"></param>
		public ClientDetailsPage(DetailsViewModel<Client> detailsVm) : base(detailsVm)
		{
			InitializeComponent();
			ItemsListView.BindingContext = ListVm;
		}

		protected override void DeselectListViewItem() => ItemsListView.SelectedItem = null;

		private async void AddProjectToClient_Clicked(object sender, EventArgs e) => await Navigation.PushAsync(new AddProjectPage(new AddProjectViewModel(new Project { ClientId = detailsVm.Item.Id }))).ConfigureAwait(false);
	}

	[DesignTimeVisible(false), XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class ProjectDetailsPage : ItemDetailsPage<Project, Workshift>
	{
		/// <summary>
		/// Contains project information and a list of workshifts for the project. Also contains navigation to project's contact list page.
		/// </summary>
		/// <param name="detailsVm"></param>
		public ProjectDetailsPage(DetailsViewModel<Project> detailsVm) : base(detailsVm)
		{
			InitializeComponent();
			ItemsListView.BindingContext = ListVm;
		}

		protected override void DeselectListViewItem() => ItemsListView.SelectedItem = null;

		protected override async Task NavigateToItemSelectedPage(Workshift item) => await NavigateToEditPage(item).ConfigureAwait(false);

		protected async void AddWorkshiftClicked(object sender, EventArgs e) => await Navigation.PushAsync(new AddWorkshiftPage(new AddWorkshiftViewModel(new Workshift { ProjectId = detailsVm.Item.Id }))).ConfigureAwait(false);
	}
}
