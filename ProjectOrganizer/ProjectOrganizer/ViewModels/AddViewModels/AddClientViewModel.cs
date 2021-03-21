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
	public class AddClientViewModel : AddItemViewModel<Client>
	{
		public AddClientViewModel(int itemId) : base(itemId)
		{
			Initialize();
		}

		public AddClientViewModel(Client item) : base(item.Id)
		{
			Initialize();
		}

		protected override void Initialize()
		{
			validator = new ClientValidator();
			ValidateItem().Wait();
		}
	}
}
