using System;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.Repositories;
using SQLite;

namespace ProjectOrganizer.Models
{
	public class Workshift : DataItem, ISQLiteDataItem
	{
		[Indexed, ForeignKey(typeof(Project))]
		public int ProjectId { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }

		public double Hours => End.Subtract(Start).TotalHours;
		public string TimeSpan
		{
			get
			{
				if (Start.Date == End.Date)
				{
					return $"{Start:g} - {End:t}";
				}

				return $"{Start:g} - {End:g}";
			}
		}

		public Workshift()
		{
			if (Start == End && Start == DateTime.MinValue)
			{
				Start = DateTime.Now.Date.AddDays(-1).AddHours(8);
				End = Start.AddHours(2);
			}
		}
	}
}
