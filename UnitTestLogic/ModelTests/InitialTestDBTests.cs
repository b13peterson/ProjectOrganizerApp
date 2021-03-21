using System.Collections.Generic;
using System.Linq;
using ProjectOrganizer;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.Models;
using ProjectOrganizer.Repositories;
using Xunit;

namespace UnitTestLogic.Models
{

	[Collection("Database")]
	public abstract class InitialTestDBTests<T> : TestsWithDatabase<T> where T : ISQLiteDataItem
	{
		public InitialTestDBTests() {
			// Class Arrange
			App.GenerateTestData().Wait();
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(3)]
		[InlineData(4)]
		public void GetItem_TestDataLoaded_ItemNotNull(int id)
		{
			// Act
			T item = repository.GetItemAsync(id).Result;

			// Assert
			Assert.NotNull(item);
		}

		[Fact]
		public void GetAllItems_TestDataLoaded_ItemsNotNull()
		{
			// Act
			List<T> items = repository.GetAllItemsAsync().Result;

			// Assert
			Assert.NotNull(items);
		}
	}

	[Collection("Database")]
	public class ClientTestDB_Tests : InitialTestDBTests<Client>
	{
		public ClientTestDB_Tests() { }

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(3)]
		[InlineData(4)]
		[InlineData(5)]
		public void DeleteClient_TestDataLoaded_RelatedProjectsDeleted(int clientId)
		{
			// Arrange
			var projectRepo = new ProjectRepository();
			List<Project> getProjectsBeforeTask = projectRepo.GetAllItemsAsync().Result;

			// Act
			repository.DeleteItemAsync(clientId).Wait();
			List<Project> projectsAfterDelete = projectRepo.GetAllItemsAsync().Result;

			// Assert
			Assert.DoesNotContain(projectsAfterDelete, x => x.ClientId == clientId);
		}
	}

	[Collection("Database")]
	public class ProjectTestDB_Tests : InitialTestDBTests<Project>
	{
		public ProjectTestDB_Tests() { }

		[Fact]
		public void GetCurrentProjects_TestDataLoaded_LoadedCurrentProjects()
		{
			// Act
			List<Project> currentProjects = ((ProjectRepository)repository).GetCurrentProjectsAsync().Result;

			// Assert
			Assert.Equal(2, currentProjects.Count(x => x.Name == "Project C" || x.Name == "Project E"));
		}
	}

	[Collection("Database")]
	public class WorkshiftTestDB_Tests : InitialTestDBTests<Workshift>
	{
		public WorkshiftTestDB_Tests() { }

		[Fact]
		public void GetCurrentWorkshift_TestDataLoaded_CurrentWorkshiftReturned()
		{
			// Act
			Workshift workshift = ((WorkshiftRepository)repository).GetCurrentWorkshiftAsync().Result;

			// Assert
			Assert.Equal("Workshift B", workshift.Name);
		}
	}
}
