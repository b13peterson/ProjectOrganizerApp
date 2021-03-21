using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.Repositories;
using Xamarin.Forms;

namespace ProjectOrganizer.ViewModels
{
	public class ListViewModel<T> : BaseViewModel where T : ISQLiteDataItem
	{

		private ISQLiteDataItem filterDataItem { get; }
		private IItemRepository<T> repository;

		private ObservableCollection<T> items;
		public ObservableCollection<T> Items
		{
			get
			{
				if (items == null)
				{
					items = new ObservableCollection<T>();
				}
				return items;
			}
			set => items = value;
		}

		private string _listTitle = "";
		public string ListTitle
		{
			get => _listTitle;
			set => SetProperty(ref _listTitle, value);
		}

		public Command LoadItemsCommand { get; set; }

		public ListViewModel(ISQLiteDataItem filterDataItem = null)
		{
			Title = $"{typeof(T).Name}s";
			this.filterDataItem = filterDataItem; ;
			repository = RepositoryFactory.Get<T>();

			LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand<T>(this.filterDataItem).ConfigureAwait(false));

			MessagingCenter.Subscribe<Message>(subscriber: this, message: $"Reload{typeof(T).Name}s", callback: async obj => await ExecuteLoadItemsCommand<T>(this.filterDataItem).ConfigureAwait(false));

			MessagingCenter.Subscribe<Message, T>(subscriber: this, message: $"Delete{typeof(T).Name}", callback: (message, item) => Items.Remove(item));
		}

		protected async Task ExecuteLoadItemsCommand<TU>(ISQLiteDataItem primaryKeyItem = null) where TU : ISQLiteDataItem
		{
			if (IsBusy)
			{
				return;
			}
			IsBusy = true;
			try
			{
				List<T> items = primaryKeyItem == null ? await repository.GetAllItemsAsync().ConfigureAwait(false) : await repository.GetRelatedItemsAsync(primaryKeyItem).ConfigureAwait(false);
				Items.Clear();
				if (items != null)
				{
					foreach (T item in items)
					{
						Items.Add(item);
					}
				}
			} catch (Exception ex)
			{
				throw new Exception(ex.Message);
			} finally
			{
				IsBusy = false;
			}
		}
	}
}
