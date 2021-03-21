using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.Models;
using static ProjectOrganizer.App;

namespace ProjectOrganizer.Repositories
{
	public class ProjectRepository : IItemRepository<Project>
	{

		public async Task DeleteItemAsync(int id)
		{
			var workshiftRepository = new WorkshiftRepository();

			List<Workshift> workshifts = await workshiftRepository.GetAllItemsAsync().ConfigureAwait(false);
			var workshiftsWithDeletedProject = workshifts.Where(w => w.ProjectId == id).ToList();

			foreach (Workshift Workshift in workshiftsWithDeletedProject)
			{
				await workshiftRepository.DeleteItemAsync(Workshift.Id).ConfigureAwait(false);
			}

			await Database.DeleteProjectAsync(id).ConfigureAwait(false);
		}

		public async Task<List<Project>> GetAllItemsAsync() => await Database.GetAllProjectsAsync().ConfigureAwait(false);

		public async Task<Project> GetItemAsync(int id) => await Database.GetProjectAsync(id).ConfigureAwait(false);

		//Rethinked, C. (2018) Building dynamic LINQ queries using Expression Trees and Func. Retrieved from https://coderethinked.com/building-dynamic-linq-queries-using-expression-trees-and-func/
		public async Task<List<Project>> GetRelatedItemsAsync<U>(U filterItem)
		where U : ISQLiteDataItem
		{
			ForeignKeyRelationship fkRelationship = SQLFactory.ForeignKeyRelationships.FirstOrDefault(x => x.PrimaryTable == filterItem.GetType());
			if (fkRelationship != null && fkRelationship.ForeignTable == typeof(Project))
			{
				Task<List<Project>> allItemsTask = GetAllItemsAsync();
				ParameterExpression param = Expression.Parameter(typeof(Project), name: "x");
				MemberExpression member = Expression.Property(param, typeof(Project).GetProperty(fkRelationship.ForeignKeyColumnName));
				ConstantExpression constant = Expression.Constant(filterItem.Id, typeof(int));
				Expression body = Expression.Equal(member, constant);
				Type delegateType = typeof(Func<,>).MakeGenericType(typeof(Project), typeof(bool));
				Func<Project, bool> exp = Expression.Lambda<Func<Project, bool>>(body, param).Compile();
				List<Project> allItems = await allItemsTask.ConfigureAwait(false);
				return allItems.Where(exp).ToList();
			}
			return null;
		}
		
		public async Task SaveItemAsync(Project Project) => await Database.SaveAsync(Project).ConfigureAwait(false);

		public async Task<List<Project>> GetCurrentProjectsAsync() => (await Database.GetAllProjectsAsync().ConfigureAwait(false)).Where(x => x.IsCurrent == true).ToList();
	}
}
