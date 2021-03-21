using System;
using ProjectOrganizer.Models;
using ProjectOrganizer.Interfaces;

namespace ProjectOrganizer.Repositories
{
	public static class RepositoryFactory
	{
		/// <summary>
		/// Get a new repository for the given datatype
		/// </summary>
		/// <typeparam name="T">An ISQLiteDataItem derived from </typeparam>
		/// <returns></returns>
		public static IItemRepository<T> Get<T>() where T : ISQLiteDataItem
		{
			if (typeof(T) == typeof(Client))
			{
				return (IItemRepository<T>)new ClientRepository();
			} else if (typeof(T) == typeof(Project))
			{
				return (IItemRepository<T>)new ProjectRepository();
			} else if (typeof(T) == typeof(Workshift))
			{
				return (IItemRepository<T>)new WorkshiftRepository();
			};
			throw new ArgumentException("Type of repository requested was not from ISQLiteDataItem.");
		}

	}
}
