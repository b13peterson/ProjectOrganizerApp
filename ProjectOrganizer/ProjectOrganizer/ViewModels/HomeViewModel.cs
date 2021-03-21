using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ProjectOrganizer.Models;
using ProjectOrganizer.Repositories;
using Xamarin.Forms;

namespace ProjectOrganizer.ViewModels
{
	public class HomeViewModel : BaseViewModel
	{
		private ProjectRepository projectRepository = new ProjectRepository();
		private WorkshiftRepository workshiftRepository = new WorkshiftRepository();

		private bool _isUserWorking;
		public bool IsUserWorking
		{
			get => _isUserWorking;
			set => SetProperty(ref _isUserWorking, value);
		}

		private bool _isLoadTestDataButtonVisible;
		public bool IsLoadTestDataButtonVisible
		{
			get => _isLoadTestDataButtonVisible;
			set => SetProperty(ref _isLoadTestDataButtonVisible, value);
		}

		private string projectSearchEntry = "";

		private ObservableCollection<Project> items;
		public ObservableCollection<Project> Items
		{
			get
			{
				if (items == null)
				{
					items = new ObservableCollection<Project>();
				}
				IsLoadTestDataButtonVisible = (items?.Count ?? 0) == 0 && string.IsNullOrEmpty(projectSearchEntry);
				return items;
			}
			set => items = value;
		}

		public Command LoadItemsCommand { get; }

		private Project _currentWorkshiftProject;
		public Project CurrentWorkshiftProject
		{
			get => _currentWorkshiftProject;
			set => SetProperty(ref _currentWorkshiftProject, value);
		}

		public Workshift CurrentWorkshift { get; set; }

		public string ProjectSearchEntry
		{
			get => projectSearchEntry?.ToLower();
			set => projectSearchEntry = value;
		}

		public HomeViewModel()
		{
			Title = "Select Project to start new workshift.";

			LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand(CurrentWorkshift).ConfigureAwait(false));

			MessagingCenter.Subscribe<Message>(subscriber: this, message: "ReloadProjects", callback: async obj => await ExecuteLoadItemsCommand(null).ConfigureAwait(false));
		}

		protected async Task ExecuteLoadItemsCommand(Workshift currentWorkshift)
		{
			if (IsBusy)
			{
				return;
			}

			IsBusy = true;
			try
			{
				List<Project> items = await projectRepository.GetCurrentProjectsAsync().ConfigureAwait(false);
				if (string.IsNullOrEmpty(ProjectSearchEntry) == false)
				{
					items = items?.Where(x => x.Name.ToLower().Contains(ProjectSearchEntry) || x.Description.ToLower().Contains(ProjectSearchEntry)).ToList();
				}

				if (Items.Count == items.Count)
				{
					foreach (Project item in items)
					{
						var oldItem = Items.Where(x => x.Id == item.Id && x.HoursWorked != item.HoursWorked).FirstOrDefault();
						if (oldItem != null)
						{
							var index = Items.IndexOf(oldItem);
							Items.RemoveAt(index);
							Items.Insert(index, item);
						}
					}
				} else
				{
					Items.Clear();
					foreach (Project item in items)
					{
						Items.Add(item);
					}
				}

			} catch (Exception ex)
			{
				throw new Exception(ex.Message);
			} finally
			{
				IsBusy = false;
				Debug.WriteLine("CurrentWorkshift being set by ExecuteLoadItemsCommand.");
				CurrentWorkshift = currentWorkshift ?? workshiftRepository.GetCurrentWorkshiftAsync().Result;
				IsUserWorking = CurrentWorkshift != null;
				CurrentWorkshiftProject = Items.FirstOrDefault(x => x.Id == CurrentWorkshift?.ProjectId);
			}
		}

		public async Task ChangeCurrentWorkshift(Workshift workshift)
		{
			if (CurrentWorkshift != null && CurrentWorkshift != workshift)
			{
				CurrentWorkshift.End = DateTime.Now;
				Debug.WriteLine("CurrentWorkshift being saved by ChangeCurrentWorkshift.");
				await workshiftRepository.SaveItemAsync(CurrentWorkshift).ConfigureAwait(false);
				CurrentWorkshift = null;
			}

			IsUserWorking = workshift != null;

			if (workshift != null)
			{
				Debug.WriteLine("CurrentWorkshift being set by ChangeCurrentWorkshift.");
				CurrentWorkshift = workshift;
				await workshiftRepository.SaveItemAsync(CurrentWorkshift).ConfigureAwait(false);
			}

			await ExecuteLoadItemsCommand(CurrentWorkshift).ConfigureAwait(false);
		}
	}
}
