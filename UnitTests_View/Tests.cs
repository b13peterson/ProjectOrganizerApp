using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectOrganizer;
using Xamarin.UITest;
using Xamarin.UITest.Queries;


namespace UnitTests_View
{
	[TestFixture(Platform.Android)]
	public class Tests
	{
		[SetUp]
		public void BeforeEachTest()
		{
			_app = AppInitializer.StartApp(platform);
		}
		private IApp _app;
		private readonly Platform platform;

		public Tests(Platform platform) => this.platform = platform;

		[Ignore("private test")]
		private async Task<AppResult> AddResultAndNavigate(string text)
		{
			Debug.WriteLine($"AddResultAndNavigate called with {text}.");
			_app.WaitForElement(query: c => c.Marked(text), timeoutMessage: $"Timed out waiting for {text}", timeout: TimeSpan.FromSeconds(30));
			AppResult result = _app.Query(c => c.Marked(text))[0];
			_app.Tap(c => c.Marked(text));
			await Task.Delay(1000);
			return result;
		}

		[Ignore("Used within tests")]
		private bool LoadTestData()
		{
			App.GenerateTestData().Wait();
			return true;
		}

		[Test]
		public async Task DrillDownToContacts()
		{
			Assert.True(LoadTestData());

			var appResults = new List<AppResult>();
			_app.Screenshot("MainPage");
			appResults.Add(await AddResultAndNavigate("Clients"));

			_app.Screenshot("ClientsListPage");
			appResults.Add(await AddResultAndNavigate("Add Client"));

			_app.Screenshot("AddClientPage");
			appResults.Add(await AddResultAndNavigate("Cancel"));

			_app.Screenshot("ClientsListPage_2");
			appResults.Add(await AddResultAndNavigate("Client A"));

			_app.Screenshot("ClientDetailsPage");
			appResults.Add(await AddResultAndNavigate("Edit"));

			_app.Screenshot("AddClientPage_2");
			appResults.Add(await AddResultAndNavigate("Cancel"));

			_app.Screenshot("ClientDetailsPage_2");
			appResults.Add(await AddResultAndNavigate("Add Project"));

			_app.Screenshot("AddProjectPage");
			appResults.Add(await AddResultAndNavigate("Cancel"));

			_app.Screenshot("ClientDetailsPage_2");
			appResults.Add(await AddResultAndNavigate("Project A"));

			_app.Screenshot("ProjectDetailsPage");
			appResults.Add(await AddResultAndNavigate("Add Workshift"));

			_app.Screenshot("AddWorkshiftPage");

			Assert.IsTrue(appResults.Count == 10);
		}
	}
}