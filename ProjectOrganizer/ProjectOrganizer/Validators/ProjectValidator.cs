using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using ProjectOrganizer.Models;
using ProjectOrganizer.Repositories;

namespace ProjectOrganizer.Validators
{
	public class ProjectValidator : AbstractValidator<Project>
	{
		private IEnumerable<Project> allProjects;
		public ProjectValidator()
		{
			getAllProjects().Wait();
			RuleFor(project => project.Name).NotEmpty();
			RuleFor(project => project.Name).Must(IsNameUnique)
								   .WithMessage("Name must be unique.");

			RuleFor(project => project.Description).NotEmpty();
		}

		private async Task getAllProjects()
		{
			var projectRepository = new ProjectRepository();
			allProjects = await projectRepository.GetAllItemsAsync().ConfigureAwait(false);
		}

		private bool IsNameUnique(Project editedProject, string newValue) => allProjects.Count(p => p.Id != editedProject.Id && p.Name == newValue) == 0;

	}
}
