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
	public class AddClientViewModelTests : IDisposable
	{
		public AddClientViewModel viewModel;
		public AddClientViewModelTests(AddClientViewModel viewModel = null)
		{
			// Class arrange
			App.Database = new MockDatabase();
			this.viewModel = viewModel ?? new AddClientViewModel(new Client());
		}
		public virtual void Dispose() { }

		[Fact]
		public void Item_EmptyDB_RaisePropertyChangedEvent()
		{
			// Arrange
			viewModel.Item = null;
			var item = new Client();

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
			var existingClient = new Client() { Name = "Test Client 123456789" };

			// Act
			viewModel = new AddClientViewModel(existingClient);

			// Assert
			Assert.NotEqual("Edit Client", viewModel.Title);
		}

		[Fact]
		public void Constructor_ExistingItem_EditPageContextLoaded()
		{
			// Arrange
			var existingClient = new Client() { Name = "Test Client 123456789" };
			new ClientRepository().SaveItemAsync(existingClient).Wait();

			// Act
			viewModel = new AddClientViewModel(existingClient);

			// Assert
			Assert.Equal("Test Client 123456789", viewModel.Item.Name);
			Assert.Equal("Edit Client", viewModel.Title);
		}

		[Fact]
		public void SaveClientToDatabase_AllArguments_Saved()
		{
			// Arrange
			var client = new Client() { Name = "Test Client 123456789", Description = "Test Client !@#$%^&*()" };
			viewModel.Item = client;

			// Act
			viewModel.SaveItemToDatabaseAsync().Wait();


			// Assert
			var clientRepository = new ClientRepository();
			var resultClient = clientRepository.GetAllItemsAsync().Result.FirstOrDefault(x => x.Name == "Test Client 123456789");

			Assert.NotNull(resultClient);
		}

		[Theory]
		[InlineData("", "a")]
		[InlineData("", "")]
		[InlineData("a", "")]
		public void SaveClientToDatabase_EmptyArgument_NotSaved(string name, string description)
		{
			// Arrange
			var client = new Client() { Name = name, Description = description };
			viewModel.Item = client;

			// Act
			viewModel.SaveItemToDatabaseAsync().Wait();

			// Assert
			var clientRepository = new ClientRepository();
			var resultClient = clientRepository.GetAllItemsAsync().Result.FirstOrDefault(x => x.Name == name && x.Description == description);

			Assert.Null(resultClient);
		}
	}
}
