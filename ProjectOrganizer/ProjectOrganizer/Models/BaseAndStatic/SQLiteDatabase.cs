using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.Models;
using ProjectOrganizerLibrary;
using SQLite;
using Xamarin.Forms;

namespace ProjectOrganizer.SQLAccess
{
	public class SQLiteDatabase : IDatabaseAccesser
	{
		private static SQLiteAsyncConnection _conn;
		private static SQLiteAsyncConnection conn
		{
			get
			{
				if (_conn == null)
				{
					InitializeSQLConnection();
				}
				return _conn;
			}
		}

		private static void InitializeSQLConnection()
		{
			_conn = new SQLiteAsyncConnection(DependencyService.Get<IFileHelper>().GetLocalFilePath(Constants.TestDB));
			foreach (var t in SQLFactory.Types)
			{
				SQLFactory.BuildForeignKeyDictionary(t);
			}
			CreateAllTables();
		}


		/// <summary>
		/// Inserts the item if it is a new item, updates it if it already exists in the database.
		/// </summary>
		/// <param name="item">SQLiteDatam item to save to database</param>
		/// <returns>Primary Key Id of saved item</returns>
		public async Task<int> SaveAsync(ISQLiteDataItem item) => item.Id != 0 ? await conn.UpdateAsync(item) : await conn.InsertAsync(item).ConfigureAwait(false);

		public async Task<List<Client>> GetAllClientsAsync() => await conn.Table<Client>().ToListAsync().ConfigureAwait(false);
		public async Task<List<Project>> GetAllProjectsAsync() => await conn.Table<Project>().ToListAsync().ConfigureAwait(false);
		public async Task<List<Workshift>> GetAllWorkshiftsAsync() => await conn.Table<Workshift>().ToListAsync().ConfigureAwait(false);

		/// <summary>
		/// Get the client with the given Primary Key Id
		/// </summary>
		/// <param name="id">Primary Key Id of client to retrieve from database</param>
		/// <returns>Client with the given Primary Key Id</returns>
		public async Task<Client> GetClientAsync(int id) => await conn.Table<Client>().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
		public async Task<Project> GetProjectAsync(int id) => await conn.Table<Project>().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
		public async Task<Workshift> GetWorkshiftAsync(int id) => await conn.Table<Workshift>().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);

		public async Task<int> DeleteClientAsync(int id) => await conn.Table<Client>().DeleteAsync(x => x.Id == id).ConfigureAwait(false);
		public async Task<int> DeleteProjectAsync(int id) => await conn.Table<Project>().DeleteAsync(x => x.Id == id).ConfigureAwait(false);
		public async Task<int> DeleteWorkshiftAsync(int id) => await conn.Table<Workshift>().DeleteAsync(x => x.Id == id).ConfigureAwait(false);

		public static void CreateAllTables()
		{
			conn.CreateTableAsync<Client>().Wait();
			conn.CreateTableAsync<Project>().Wait();
			conn.CreateTableAsync<Workshift>().Wait();
		}

		public static void DeleteAllTables()
		{
			conn.DropTableAsync<Client>().Wait();
			conn.DropTableAsync<Project>().Wait();
			conn.DropTableAsync<Workshift>().Wait();
		}

		public void RecreateAllTables()
		{
			DeleteAllTables();
			CreateAllTables();
		}
	}
}
