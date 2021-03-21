using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.Models;
using ProjectOrganizer.SQLAccess;
using Xamarin.Forms;

namespace ProjectOrganizer
{
	public partial class App : Application
	{

		public static IDatabaseAccesser Database { get; set; }
		public App()
		{
			InitializeComponent();
			Database = null;
			Database = new SQLiteDatabase();
			MainPage = new NavigationPage(new HomeListPage());
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}

		public static void RecreateAllTables() => Database.RecreateAllTables();

		private static DateTime now;
		public static async Task GenerateTestData()
		{
			now = DateTime.Now;
			RecreateAllTables();
			foreach (Client item in DummyClients)
			{
				await Database.SaveAsync(item).ConfigureAwait(false);
			}
			foreach (Project item in DummyProjects)
			{
				await Database.SaveAsync(item).ConfigureAwait(false);
			}
			foreach (Workshift item in DummyWorkshifts)
			{
				await Database.SaveAsync(item).ConfigureAwait(false);
			}
		}


		public static readonly List<Client> DummyClients = new List<Client> {
			new Client {Name = "Client A", Description = "This is a client description. This is the first client added."},
			new Client {Name = "Client B", Description = "This is a client description. This is the second client added."},
			new Client {Name = "Client C", Description = "This is a client description. This is the third client added."},
			new Client {Name = "Client D", Description = "This is a client description. This is the fourth client added."},
			new Client {Name = "Client E", Description = "This is a client description. This is the fifth client added."}
		};

		public static readonly List<Project> DummyProjects = new List<Project> {
			new Project {
				Name = "Project A",
				Description = "This is a Project description. This is the first Project added.",
				ClientId = 1,
				ContactName = "Manager A",
				ContactEmail = "ManageA@abc.com",
				ContactPhoneNumber = "(123) 123-1234",
				IsCurrent = false
			},
			new Project {
				Name = "Project B",
				Description = "This is a Project description. This is the second Project added.",
				ClientId = 1,
				ContactName = "Manager B",
				ContactEmail = "ManageB@abc.com",
				ContactPhoneNumber = "(123) 123",
				IsCurrent = false
			},
			new Project {
				Name = "Project C",
				Description = "This is a Project description. This is the third Project added.",
				ClientId = 4,
				ContactName = "Manager C",
				ContactEmail = "ManageC@abc.com",
				ContactPhoneNumber = "(123) 123-1",
				IsCurrent = true
			},
			new Project {
				Name = "Project D",
				Description = "This is a Project description. This is the fourth Project added.",
				ClientId = 2,
				ContactName = "Manager D",
				ContactEmail = "ManageD@abc.com",
				ContactPhoneNumber = "(123) 123-12",
				IsCurrent = false
			},
			new Project {
				Name = "Project E",
				Description = "This is a Project description. This is the fifth Project added.",
				ClientId = 3,
				ContactName = "Manager E",
				ContactEmail = "ManageE@abc.com",
				ContactPhoneNumber = "(123) 123-123",
				IsCurrent = true
			}
		};

		public static readonly List<Workshift> DummyWorkshifts = new List<Workshift> {
			new Workshift {
				Name = "Workshift A",
				Description = "This is a Workshift description. This is the first Workshift added.",
				ProjectId = 1,
				Start = now.AddHours(8),
				End = now.Add(new TimeSpan(hours: 10, minutes: 30, seconds: 0))
			},
			new Workshift {
				Name = "Workshift B",
				Description = "This is a Workshift description. This is the second Workshift added.",
				ProjectId = 4,
				Start = now.AddHours(8),
				End = now.AddHours(8)
			},
			new Workshift {
				Name = "Workshift C",
				Description = "This is a Workshift description. This is the third Workshift added.",
				ProjectId = 1,
				Start = now.AddHours(8),
				End = now.Add(new TimeSpan(hours: 8, minutes: 20, seconds: 0))
			},
			new Workshift {
				Name = "Workshift D",
				Description = "This is a Workshift description. This is the fourth Workshift added.",
				ProjectId = 2,
				Start = now.AddHours(8),
				End = now.Add(new TimeSpan(hours: 10, minutes: 00, seconds: 0))
			},
			new Workshift {
				Name = "Workshift E",
				Description = "This is a Workshift description. This is the fifth Workshift added.",
				ProjectId = 3,
				Start = now.AddHours(8),
				End = now.Add(new TimeSpan(hours: 12, minutes: 00, seconds: 0))
			}
		};
	}
}

