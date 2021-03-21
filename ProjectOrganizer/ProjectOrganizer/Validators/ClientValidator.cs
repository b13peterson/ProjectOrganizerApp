using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using ProjectOrganizer.Models;
using ProjectOrganizer.Repositories;

namespace ProjectOrganizer.Validators
{
	public class ClientValidator : AbstractValidator<Client>
	{
		private IEnumerable<Client> allClients;
		public ClientValidator()
		{
			getAllClients().Wait();

			RuleFor(client => client.Name).NotEmpty();
			RuleFor(client => client.Name).Must(IsNameUnique)
								 .WithMessage("Name must be unique.");

			RuleFor(client => client.Description).NotEmpty();
		}

		private async Task getAllClients()
		{
			var clientRepository = new ClientRepository();
			allClients = await clientRepository.GetAllItemsAsync().ConfigureAwait(false);
		}

		private bool IsNameUnique(Client editedClient, string newValue) => allClients.Count(c => c.Id != editedClient.Id && c.Name == newValue) == 0;
	}
}
