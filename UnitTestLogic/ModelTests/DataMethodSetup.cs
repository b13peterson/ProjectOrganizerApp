using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectOrganizer;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.Repositories;
using ProjectOrganizer.Models;
using Xunit;

namespace UnitTestLogic.Models {
	public class MockDatabase : IDatabaseAccesser {

		private int NextId_Client = 1;
		private int NextId_Project = 1;
		private int NextId_Workshift = 1;
		private Dictionary<int, Client> Clients = null;
		private Dictionary<int, Project> Projects = null;
		private Dictionary<int, Workshift> Workshifts = null;

		public MockDatabase() {
			CreateTable(typeof(Client));
			CreateTable(typeof(Project));
			CreateTable(typeof(Workshift));
		}

		protected void CreateTable(Type type) {
			if (type == typeof(Client)) {
				if (Clients == null) {
					Clients = new Dictionary<int, Client>();
				}
			} else if (type == typeof(Project)) {
				if (Projects == null) {
					Projects = new Dictionary<int, Project>();
				}
			} else if (type == typeof(Workshift)) {
				if (Workshifts == null) {
					Workshifts = new Dictionary<int, Workshift>();
				}
			}
		}
		protected void DeleteTable(Type type) {
			if (type == typeof(Client)) {
				Clients = null;
			} else if (type == typeof(Project)) {
				Projects = null;
			} else if (type == typeof(Workshift)) {
				Workshifts = null;
			}
		}
		public async Task<int> SaveAsync(ISQLiteDataItem dataItem) {
			Type type = dataItem.GetType();
			if (type == typeof(Client)) {
				if (dataItem.Id == 0) {
					dataItem.Id = NextId_Client++;
				}
				Clients[dataItem.Id] = (Client)dataItem;
			} else if (type == typeof(Project)) {
				if (dataItem.Id == 0) {
					dataItem.Id = NextId_Project++;
				}
				Projects[dataItem.Id] = (Project)dataItem;
			} else if (type == typeof(Workshift)) {
				if (dataItem.Id == 0) {
					dataItem.Id = NextId_Workshift++;
				}
				Workshifts[dataItem.Id] = (Workshift)dataItem;
			}
			return await Task.FromResult(dataItem.Id).ConfigureAwait(false);
		}

		public Task<List<Client>> GetAllClientsAsync() => Task.FromResult(Clients.Values.ToList());
		public Task<List<Project>> GetAllProjectsAsync() => Task.FromResult(Projects.Values.ToList());
		public Task<List<Workshift>> GetAllWorkshiftsAsync() => Task.FromResult(Workshifts.Values.ToList());
		public Task<Client> GetClientAsync(int id) => Task.FromResult(Clients[id]);
		public Task<Project> GetProjectAsync(int id) => Task.FromResult(Projects[id]);
		public Task<Workshift> GetWorkshiftAsync(int id) => Task.FromResult(Workshifts[id]);
		public Task<int> DeleteClientAsync(int id) => Task.FromResult(Clients.Remove(id) ? 1 : 0);
		public Task<int> DeleteProjectAsync(int id) => Task.FromResult(Projects.Remove(id) ? 1 : 0);
		public Task<int> DeleteWorkshiftAsync(int id) => Task.FromResult(Workshifts.Remove(id) ? 1 : 0);
		public void RecreateAllTables() {
			DeleteTable(typeof(Client));
			DeleteTable(typeof(Project));
			DeleteTable(typeof(Workshift));
			NextId_Client = 1;
			NextId_Project = 1;
			NextId_Workshift = 1;
			CreateTable(typeof(Client));
			CreateTable(typeof(Project));
			CreateTable(typeof(Workshift));
		}
	}

	[Collection("Database")]
	public abstract class TestsWithDatabase<T> : IDisposable where T : ISQLiteDataItem {
		public IItemRepository<T> repository;
		public TestsWithDatabase() {
			App.Database = new MockDatabase();
			repository = RepositoryFactory.Get<T>();
		}
		public virtual void Dispose() { }
	}

}
