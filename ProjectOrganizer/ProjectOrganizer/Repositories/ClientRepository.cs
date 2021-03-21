using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.Models;
using static ProjectOrganizer.App;

namespace ProjectOrganizer.Repositories
{
	public class ClientRepository : IItemRepository<Client>
	{

		public async Task DeleteItemAsync(int id)
		{
			var projectRepository = new ProjectRepository();

			List<Project> projects = await projectRepository.GetAllItemsAsync().ConfigureAwait(false);
			var projectsWithDeletedClient = projects.Where(p => p.ClientId == id).ToList();

			foreach (Project project in projectsWithDeletedClient)
			{
				await projectRepository.DeleteItemAsync(project.Id).ConfigureAwait(false);
			}

			await Database.DeleteClientAsync(id).ConfigureAwait(false);
		}

		public async Task<List<Client>> GetAllItemsAsync() => await Database.GetAllClientsAsync().ConfigureAwait(false);

		public async Task<Client> GetItemAsync(int id) => await Database.GetClientAsync(id).ConfigureAwait(false);

		public async Task<List<Client>> GetRelatedItemsAsync<U>(U filterItem)
		where U : ISQLiteDataItem => await GetAllItemsAsync().ConfigureAwait(false);

		public async Task SaveItemAsync(Client client) => await Database.SaveAsync(client).ConfigureAwait(false);
	}


}
