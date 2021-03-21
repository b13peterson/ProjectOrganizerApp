using System;
using System.Collections.Generic;
using System.Linq;
using ProjectOrganizer;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.Models;
using ProjectOrganizer.ViewModels;
using ProjectOrganizer.Repositories;
using UnitTestLogic.Models;
using static ProjectOrganizer.Models.SQLFactory;
using Xunit;

namespace UnitTests_Logic.ViewModelTests
{
	[Collection("Database")]
	public class AddWorkshiftViewModelTests : IDisposable
	{
		public AddWorkshiftViewModel viewModel;
		public AddWorkshiftViewModelTests(AddWorkshiftViewModel viewModel = null)
		{
			App.Database = new MockDatabase();
			this.viewModel = viewModel ?? new AddWorkshiftViewModel(new Workshift());
		}
		public virtual void Dispose() { }

		[Fact]
		public void Item_EmptyDB_RaisePropertyChangedEvent()
		{
			// Arrange
			viewModel.Item = null;
			var item = new Workshift();

			// Act
			Assert.PropertyChanged(viewModel, "Item", () => { viewModel.Item = item; });
			Assert.Equal(item, viewModel.Item);

			Assert.PropertyChanged(viewModel, "Item", () => { viewModel.Item = null; });
			Assert.Null(viewModel.Item);
		}

		[Fact]
		public void Constructor_ExistingItem_EditPageContextLoadFail()
		{
			// Arrange
			var existingWorkshift = new Workshift() { Name = "Test Workshift 123456789" };

			// Act
			viewModel = new AddWorkshiftViewModel(existingWorkshift);

			// Assert
			Assert.NotEqual("Edit Workshift", viewModel.Title);
		}

		[Fact]
		public void Constructor_ExistingItem_EditPageContextLoaded()
		{
			// Arrange
			var existingWorkshift = new Workshift() { Name = "Test Workshift 123456789" };
			new WorkshiftRepository().SaveItemAsync(existingWorkshift).Wait();

			// Act
			viewModel = new AddWorkshiftViewModel(existingWorkshift);

			// Assert
			Assert.Equal("Test Workshift 123456789", viewModel.Item.Name);
			Assert.Equal("Edit Workshift", viewModel.Title);
		}

		[Fact]
		public void SaveWorkshiftToDatabase_AllArguments_Saved()
		{
			// Arrange
			var Workshift = new Workshift() { Name = "Test Workshift 123456789", Description = "Test Description !@#$%^&*()" };
			viewModel.Item = Workshift;

			// Act
			viewModel.SaveItemToDatabaseAsync().Wait();

			// Assert
			var WorkshiftRepository = new WorkshiftRepository();
			var resultWorkshift = WorkshiftRepository.GetAllItemsAsync().Result.FirstOrDefault(x => x.Name == "Test Workshift 123456789");

			Assert.NotNull(resultWorkshift);
		}

		[Theory]
		[InlineData("", "a")]
		[InlineData("", "")]
		public void SaveWorkshiftToDatabase_EmptyArgument_NotSaved(string name, string description)
		{
			// Arrange
			var workshift = new Workshift() { Name = name, Description = description };
			viewModel.Item = workshift;

			// Act
			viewModel.SaveItemToDatabaseAsync().Wait();

			// Assert
			var workshiftRepository = new WorkshiftRepository();
			var resultWorkshift = workshiftRepository.GetAllItemsAsync().Result.FirstOrDefault(x => x.Name == name && x.Description == description);

			Assert.Null(resultWorkshift);
		}

		[Theory]
		[InlineData(1, "Project A")]
		[InlineData(4, "Project B")]
		public void SelectedProject_NewViewModel_SelectedProjectIsCorrect(int workshiftPkId, string projectName)
		{
			// Arrange
			App.GenerateTestData().Wait();
			var workshiftRepository = new WorkshiftRepository();
			var workshift = workshiftRepository.GetItemAsync(workshiftPkId).Result;

			// Act
			viewModel = new AddWorkshiftViewModel(workshift);

			// Assert
			Assert.Equal(projectName, viewModel.SelectedProject?.Name);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		public void SetProjectOfWorkshift_NewViewModel_ProjectIsSet(int projectId)
		{
			// Arrange
			App.GenerateTestData().Wait();
			var workshiftRepository = new WorkshiftRepository();
			var projectRepository = new ProjectRepository();
			var workshift = workshiftRepository.GetItemAsync(1).Result;
			viewModel = new AddWorkshiftViewModel(workshift);
			var project = projectRepository.GetItemAsync(projectId).Result;

			// Act
			viewModel.SetProjectOfWorkshift(project);

			// Assert
			Assert.Equal(projectId, viewModel.Item.ProjectId);
		}
	}
}
