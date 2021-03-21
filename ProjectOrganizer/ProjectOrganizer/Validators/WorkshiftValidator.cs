using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using ProjectOrganizer.Models;
using ProjectOrganizer.Repositories;

namespace ProjectOrganizer.Validators
{
	public class WorkshiftValidator : AbstractValidator<Workshift>
	{
		private IEnumerable<Workshift> allWorkshifts;
		public WorkshiftValidator()
		{
			getAllWorkshifts().Wait();
			RuleFor(Workshift => Workshift.Name).NotEmpty();
			RuleFor(Workshift => Workshift.Name).Must(IsNameUnique).WithMessage("Name must be unique");
			RuleFor(Workshift => Workshift.End).Must(IsEndAfterStart).WithMessage("End cannot be before start.");
		}

		private async Task getAllWorkshifts()
		{
			var WorkshiftRepository = new WorkshiftRepository();
			allWorkshifts = await WorkshiftRepository.GetAllItemsAsync().ConfigureAwait(false);
		}

		private bool IsNameUnique(Workshift editedWorkshift, string newValue) => allWorkshifts.Count(w => w.Id != editedWorkshift.Id && w.Name == newValue) == 0;

		private bool IsEndAfterStart(Workshift editedWorkshift, DateTime end) => end >= editedWorkshift.Start;

	}
}
