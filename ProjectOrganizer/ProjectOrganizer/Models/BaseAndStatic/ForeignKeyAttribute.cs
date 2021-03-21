using System;

namespace ProjectOrganizer.Models
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ForeignKeyAttribute : Attribute
	{
		public Type ReferenceTableType { get; }
		public ForeignKeyAttribute(Type refTableType) => ReferenceTableType = refTableType;
	}
}
