using System;
using ProjectOrganizer.Interfaces;

namespace ProjectOrganizer.Models
{
	public class Client : DataItem, ISQLiteDataItem
	{
		public Client() { }
		public Client(Client obj) : base(obj)
		{
		}
	}
}
