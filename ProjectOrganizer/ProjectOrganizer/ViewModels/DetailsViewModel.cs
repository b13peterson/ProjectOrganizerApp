using System;
using System.Threading.Tasks;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.Repositories;
using Xamarin.Forms;

namespace ProjectOrganizer.ViewModels
{
	public class DetailsViewModel<T> : BaseViewModel where T : ISQLiteDataItem
	{
		private T item;
		public T Item
		{
			get => item;
			set => SetProperty(ref item, value);
		}

		private IItemRepository<T> repository;
		public DetailsViewModel(T item)
		{
			Item = item;
			Title = $"{typeof(T).Name} Details";
			repository = RepositoryFactory.Get<T>();

			MessagingCenter.Subscribe<Message>(subscriber: this, message: $"Reload{typeof(T).Name}s", callback: async obj => await LoadViewModelItemFromId(Item.Id).ConfigureAwait(false));
		}
		protected async Task LoadViewModelItemFromId(int pkId)
		{
			try
			{
				Item = await repository.GetItemAsync(pkId).ConfigureAwait(false);
			} catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public async Task DeleteItemAsync()
		{
			MessagingCenter.Send(sender: new Message(), message: $"Delete{typeof(T).Name}", Item);
			await repository.DeleteItemAsync(Item.Id).ConfigureAwait(false);
		}
	}
}
