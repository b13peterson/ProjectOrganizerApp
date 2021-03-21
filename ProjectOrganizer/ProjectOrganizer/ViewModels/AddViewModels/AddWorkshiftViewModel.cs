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
	public class AddWorkshiftViewModel : AddItemViewModel<Workshift>
	{
		private TimeSpan _startTime;
		public TimeSpan StartTime
		{
			get => _startTime;
			set
			{
				SetProperty(ref _startTime, value);
				Item.Start = Item.Start.Date.Add(StartTime);
			}
		}

		private TimeSpan _endTime;
		public TimeSpan EndTime
		{
			get => _endTime;
			set
			{
				SetProperty(ref _endTime, value);
				Item.End = Item.End.Date.Add(EndTime);
			}
		}

		public List<Project> Projects { get; private set; }

		private Project _selectedProject;
		public Project SelectedProject
		{
			get => _selectedProject;
			set => SetProperty(ref _selectedProject, value);
		}

		private bool _doesWorkshiftExistInDatabase;
		public bool DoesWorkshiftExistInDatabase
		{
			get => _doesWorkshiftExistInDatabase;
			set => SetProperty(ref _doesWorkshiftExistInDatabase, value);
		}

		public bool IsProjectSet => Item.ProjectId > 0;
		public bool IsProjectChangeable => !IsProjectSet;

		public AddWorkshiftViewModel(int itemId) : base(itemId)
		{
			Initialize();
		}

		public AddWorkshiftViewModel(Workshift item) : base(item.Id)
		{
			Item.ProjectId = item.ProjectId;
			Initialize();
		}

		protected override void Initialize()
		{
			StartTime = Item.Start.TimeOfDay;
			EndTime = Item.End.TimeOfDay;
			validator = new WorkshiftValidator();
			var projectRepository = RepositoryFactory.Get<Project>();
			Projects = projectRepository.GetAllItemsAsync().Result;
			if (Item.ProjectId > 0)
			{
				SelectedProject = projectRepository.GetItemAsync(Item.ProjectId).Result;
			}
			DoesWorkshiftExistInDatabase = Item.Id != 0;
			ValidateItem().Wait();
		}

		public async Task DeleteItemAsync()
		{
			MessagingCenter.Send(sender: new Message(), message: $"DeleteWorkshift", Item);
			await repository.DeleteItemAsync(Item.Id).ConfigureAwait(false);
		}

		public void SetProjectOfWorkshift(Project project) => Item.ProjectId = project.Id;
	}
}
