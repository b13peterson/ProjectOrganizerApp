using System;
using System.Linq;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.Repositories;
using SQLite;

namespace ProjectOrganizer.Models
{
	public class Project : DataItem, ISQLiteDataItem
	{
		private double _hoursWorked;

		[Indexed, ForeignKey(typeof(Client))]
		public int ClientId { get; set; }
		public bool IsCurrent { get; set; }
		public string ContactName { get; set; }
		public string ContactPhoneNumber { get; set; }
		public string ContactEmail { get; set; }

		public Project() { }
		public Project(Project obj) : base(obj)
		{
			ClientId = obj.ClientId;
			IsCurrent = obj.IsCurrent;
			ContactName = obj.ContactName;
			ContactPhoneNumber = obj.ContactPhoneNumber;
			ContactEmail = obj.ContactEmail;
		}

		[Ignore]
		public double HoursWorked
		{
			get
			{
				if (_hoursWorked.Equals(0))
				{
					_hoursWorked = RepositoryFactory.Get<Workshift>().GetAllItemsAsync().Result.Where(w => w.ProjectId == Id)?.Sum(w => w.Hours) ?? 0;
				}
				return _hoursWorked;
			}
		}

		public string ClientName => RepositoryFactory.Get<Client>().GetItemAsync(ClientId).Result.Name;
	}
}
