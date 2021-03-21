using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.Repositories;
using ProjectOrganizer.Models;
using ProjectOrganizer.Validators;
using Xamarin.Forms;
using System.Diagnostics;

namespace ProjectOrganizer.ViewModels
{
	public class AddProjectViewModel : AddItemViewModel<Project>
	{
		public AddProjectViewModel(int itemId) : base(itemId)
		{
			Debug.WriteLine($"AddProjectViewModel constructor called with itemId = {itemId}. Item = {Item.Id} {Item.Name}");
			Initialize();
		}

		public AddProjectViewModel(Project item) : base(item.Id)
		{
			Item.ClientId = item.ClientId;
			Initialize();
		}

		protected override void Initialize()
		{
			validator = new ProjectValidator();
			ValidateItem().Wait();
		}
	}
}
