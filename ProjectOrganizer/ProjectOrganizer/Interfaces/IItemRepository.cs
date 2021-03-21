
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectOrganizer.Interfaces
{
	public interface IItemRepository<T> where T : ISQLiteDataItem
	{

		Task<List<T>> GetAllItemsAsync();

		Task<T> GetItemAsync(int id);

		Task DeleteItemAsync(int id);

		Task SaveItemAsync(T item);

		Task<List<T>> GetRelatedItemsAsync<U>(U filterItem) where U : ISQLiteDataItem;
	}
}
