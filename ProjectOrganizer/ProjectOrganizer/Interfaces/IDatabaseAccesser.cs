using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectOrganizer.Models;

namespace ProjectOrganizer.Interfaces
{
	public interface IDatabaseAccesser
	{
		Task<int> SaveAsync(ISQLiteDataItem item);

		Task<List<Client>> GetAllClientsAsync();
		Task<List<Project>> GetAllProjectsAsync();
		Task<List<Workshift>> GetAllWorkshiftsAsync();

		Task<Client> GetClientAsync(int id);
		Task<Project> GetProjectAsync(int id);
		Task<Workshift> GetWorkshiftAsync(int id);

		Task<int> DeleteClientAsync(int id);
		Task<int> DeleteProjectAsync(int id);
		Task<int> DeleteWorkshiftAsync(int id);

		void RecreateAllTables();
	}
}
