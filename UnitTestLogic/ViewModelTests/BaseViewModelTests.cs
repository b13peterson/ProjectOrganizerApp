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

namespace UnitTestLogic.ViewModelTests
{

	[Collection("Database")]
	public class BaseViewModelTests : IDisposable
	{
		public BaseViewModel viewModel;
		public BaseViewModelTests()
		{
			// Class arrange
			App.Database = new MockDatabase();
			viewModel = new BaseViewModel();
		}

		public virtual void Dispose() { }

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void IsBusy_EmptyDB_RaisePropertyChangedEvent(bool initialValue)
		{
			// Act	
			viewModel.IsBusy = initialValue;

			// Assert
			Assert.PropertyChanged(viewModel, "IsBusy", () => { viewModel.IsBusy = !initialValue; });
			Assert.Equal(!initialValue, viewModel.IsBusy);
		}


		[Theory]
		[InlineData("old", "new")]
		[InlineData("old", null)]
		[InlineData(null, "new")]
		public void Title_EmptyDB_RaisePropertyChangedEvent(string initialValue, string newValue)
		{
			// Act
			viewModel.Title = initialValue;

			// Assert
			Assert.PropertyChanged(viewModel, "Title", () => { viewModel.Title = newValue; });
			Assert.Equal(newValue, viewModel.Title);
		}
	}

	
}
