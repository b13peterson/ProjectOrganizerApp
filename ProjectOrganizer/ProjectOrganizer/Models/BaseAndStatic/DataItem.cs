using System;
using SQLite;
using ProjectOrganizer.Interfaces;

namespace ProjectOrganizer.Models
{
	public abstract class DataItem
	{
		public DataItem() { }
		public DataItem(ISQLiteDataItem obj)
		{
			if (!(obj is DataItem item))
			{
				throw new ArgumentException("Object passed to ISQLiteDataItem was for cloning was not a derived class of ISQLiteDataItem.");
			}
			Id = item.Id;
			Name = item.Name;
			Description = item.Description;
		}

		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		[Unique]
		public string Name { get; set; }

		public string Description { get; set; }
	}
}
