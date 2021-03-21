namespace ProjectOrganizer.Interfaces
{
	public interface ISQLiteDataItem
	{
		int Id { get; set; }
		string Name { get; set; }
		string Description { get; set; }
	}
}
