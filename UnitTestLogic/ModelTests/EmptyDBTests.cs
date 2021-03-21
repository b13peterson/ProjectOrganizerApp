using System.Collections.Generic;
using System.Linq;
using ProjectOrganizer;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.Models;
using ProjectOrganizer.Repositories;
using Xunit;

namespace UnitTestLogic.Models.EmptyDB
{

	[Collection("Database")]
	public abstract class EmptyDBTests<T> : TestsWithDatabase<T> where T : ISQLiteDataItem
	{

		public EmptyDBTests() => App.RecreateAllTables();

		[Theory]
		[InlineData("Name", "desc")]
		[InlineData("", "")]
		[InlineData("abc123!@#$%^&*()_+=- ';/.]}", "abc123!@#$%^&*()_+=- ';/.]}")]
		public void AddItem_AnyItem_ItemSaved(string Name, string Description)
		{
			var item = (T)SQLFactory.NewItem(typeof(T));
			item.Name = Name;
			item.Description = Description;
			repository.SaveItemAsync(item).Wait();
			List<T> list = repository.GetAllItemsAsync().Result;
			Assert.StrictEqual(item, list.FirstOrDefault());
		}
	}

	[Collection("Database")]
	public class ClientEmptyDB_Tests : EmptyDBTests<Client> { }

	[Collection("Database")]
	public class ProjectEmptyDB_Tests : EmptyDBTests<Project>
	{
		[Fact]
		public void GetCurrentProjectsTest()
		{
			List<Project> currentProjects = ((ProjectRepository)repository).GetCurrentProjectsAsync().Result;
			Assert.Empty(currentProjects);
		}
	}

	[Collection("Database")]
	public class WorkshiftEmptyDB_Tests : EmptyDBTests<Workshift>
	{
		[Fact]
		public void GetCurrentWorkshiftTest()
		{
			var workshift = ((WorkshiftRepository)repository).GetCurrentWorkshiftAsync().Result;
			Assert.Null(workshift);
		}
	}



}
