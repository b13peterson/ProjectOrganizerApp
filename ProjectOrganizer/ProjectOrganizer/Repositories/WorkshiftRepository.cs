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
	public class WorkshiftRepository : IItemRepository<Workshift>
	{ 
		public async Task DeleteItemAsync(int id) => await Database.DeleteWorkshiftAsync(id).ConfigureAwait(false);

		public async Task<List<Workshift>> GetAllItemsAsync() => await Database.GetAllWorkshiftsAsync().ConfigureAwait(false);

		public async Task<Workshift> GetItemAsync(int id) => await Database.GetWorkshiftAsync(id).ConfigureAwait(false);

		//Rethinked, C. (2018) Building dynamic LINQ queries using Expression Trees and Func. Retrieved from https://coderethinked.com/building-dynamic-linq-queries-using-expression-trees-and-func/
		public async Task<List<Workshift>> GetRelatedItemsAsync<U>(U filterItem)
		where U : ISQLiteDataItem
		{
			ForeignKeyRelationship fkRelationship = SQLFactory.ForeignKeyRelationships.FirstOrDefault(x => x.PrimaryTable == filterItem.GetType());
			if (fkRelationship != null && fkRelationship.ForeignTable == typeof(Workshift))
			{
				Task<List<Workshift>> allItemsTask = GetAllItemsAsync();
				ParameterExpression param = Expression.Parameter(typeof(Workshift), name: "x");
				MemberExpression member = Expression.Property(param, typeof(Workshift).GetProperty(fkRelationship.ForeignKeyColumnName));
				ConstantExpression constant = Expression.Constant(filterItem.Id, typeof(int));
				Expression body = Expression.Equal(member, constant);
				Type delegateType = typeof(Func<,>).MakeGenericType(typeof(Workshift), typeof(bool));
				Func<Workshift, bool> exp = Expression.Lambda<Func<Workshift, bool>>(body, param).Compile();
				List<Workshift> allItems = await allItemsTask.ConfigureAwait(false);
				return allItems.Where(exp).ToList();
			}
			return null;
		}
		
		public async Task SaveItemAsync(Workshift Workshift) => await Database.SaveAsync(Workshift).ConfigureAwait(false);

		public async Task<Workshift> GetCurrentWorkshiftAsync() => (await Database.GetAllWorkshiftsAsync().ConfigureAwait(false)).FirstOrDefault(x => x.Start == x.End);
	}
}
