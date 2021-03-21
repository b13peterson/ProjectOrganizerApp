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
	public class AddProjectViewModelTests : IDisposable
	{
		public AddProjectViewModel viewModel;
		public AddProjectViewModelTests(AddProjectViewModel viewModel = null)
		{
			App.Database = new MockDatabase();
			this.viewModel = viewModel ?? new AddProjectViewModel(new Project());
		}

		public virtual void Dispose() { }

		[Fact]
		public void Item_EmptyDB_RaisePropertyChangedEvent()
		{
			// Arrange
			viewModel.Item = null;
			var item = new Project();

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
			var existingProject = new Project() { Name = "Test Project 123456789" };

			// Act
			viewModel = new AddProjectViewModel(existingProject);

			// Assert
			Assert.NotEqual("Edit Project", viewModel.Title);
		}

		[Fact]
		public void Constructor_ExistingItem_EditPageContextLoaded()
		{
			// Arrange
			var existingProject = new Project() { Name = "Test Project 123456789" };
			new ProjectRepository().SaveItemAsync(existingProject).Wait();

			// Act
			viewModel = new AddProjectViewModel(existingProject);

			// Assert
			Assert.Equal("Test Project 123456789", viewModel.Item.Name);
			Assert.Equal("Edit Project", viewModel.Title);
		}


		[Fact]
		public void SaveProjectToDatabase_AllArguments_Saved()
		{
			// Arrange
			var Project = new Project() { Name = "Test Project 123456789", Description = "Test Project !@#$%^&*()" };
			viewModel.Item = Project;

			// Act
			viewModel.SaveItemToDatabaseAsync().Wait();


			// Assert
			var ProjectRepository = new ProjectRepository();
			var resultProject = ProjectRepository.GetAllItemsAsync().Result.FirstOrDefault(x => x.Name == "Test Project 123456789");

			Assert.NotNull(resultProject);
		}

		[Theory]
		[InlineData("", "a")]
		[InlineData("", "")]
		[InlineData("a", "")]
		public void SaveProjectToDatabase_EmptyArgument_NotSaved(string name, string description)
		{
			// Arrange
			var project = new Project() { Name = name, Description = description };
			viewModel.Item = project;

			// Act
			viewModel.SaveItemToDatabaseAsync().Wait();

			// Assert
			var projectRepository = new ProjectRepository();
			var resultProject = projectRepository.GetAllItemsAsync().Result.FirstOrDefault(x => x.Name == name && x.Description == description);

			Assert.Null(resultProject);
		}
	}
}
