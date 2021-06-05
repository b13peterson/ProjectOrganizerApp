using System;
using System.Collections.Generic;
using System.Reflection;
using ProjectOrganizerLibrary.Extensions;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.ViewModels;
using ProjectOrganizer.Views;
using Xamarin.Forms;

namespace ProjectOrganizer.Models
{
	public enum SQLTypeEnum
	{
		Client,
		Project,
		Workshift
	}
	public class ForeignKeyRelationship
	{
		public Type PrimaryTable;
		public Type ForeignTable;
		public string ForeignKeyColumnName;
	}

	public static class SQLFactory
	{

		private static List<ForeignKeyRelationship> _foreignKeyRelationships;
		public static List<ForeignKeyRelationship> ForeignKeyRelationships
		{
			get
			{
				if (_foreignKeyRelationships == null)
				{
					_foreignKeyRelationships = new List<ForeignKeyRelationship>();
				}
				return _foreignKeyRelationships;
			}
			set => _foreignKeyRelationships = value;
		}

		public static void BuildForeignKeyDictionary(Type fkType)
		{
			foreach (PropertyInfo fkColumn in fkType.GetProperties())
			{
				foreach (Attribute fkColumnAttribute in Attribute.GetCustomAttributes(fkColumn))
				{
					if (!(fkColumnAttribute is ForeignKeyAttribute attribute))
					{
						continue;
					}
					var fkRelationship = new ForeignKeyRelationship()
					{
						PrimaryTable = attribute.ReferenceTableType,
						ForeignTable = fkType,
						ForeignKeyColumnName = fkColumn.Name
					};
					if (ForeignKeyRelationships.Contains(fkRelationship) == false)
					{
						ForeignKeyRelationships.Add(fkRelationship);
					}
				}
			}
		}

		public static readonly Type[] Types = { typeof(Client), typeof(Project), typeof(Workshift) };

		private static SQLTypeEnum GetEnumValueFromType(Type type) => type.Name.ToEnum<SQLTypeEnum>();

		public static SQLTypeEnum GetEnumValueFromType<T>(T item) where T : ISQLiteDataItem => GetEnumValueFromType(typeof(T));

		public static ISQLiteDataItem NewItem(Type type) => NewItem(GetEnumValueFromType(type));

		private static ISQLiteDataItem NewItem(SQLTypeEnum type)
		{
			switch (type)
			{
				case SQLTypeEnum.Client:
					return new Client();
				case SQLTypeEnum.Project:
					return new Project();
				case SQLTypeEnum.Workshift:
					return new Workshift();
				default:
					throw new ArgumentException(message: "Invalid enum value.", paramName: nameof(type));
			}
		}

		public static ContentPage NewAddPage(SQLTypeEnum type) => NewAddPage(type, NewAddViewModel(type));
		public static ContentPage NewAddPage(Type type, BaseViewModel viewModel = null) => NewAddPage(type: GetEnumValueFromType(type), viewModel);
		public static ContentPage NewAddPage(SQLTypeEnum type, BaseViewModel viewModel = null)
		{
			BaseViewModel specificViewModel = viewModel ?? NewAddViewModel(type);
			switch (type)
			{
				case SQLTypeEnum.Client:
					return new AddClientPage((AddClientViewModel)specificViewModel);
				case SQLTypeEnum.Project:
					return new AddProjectPage((AddProjectViewModel)specificViewModel);
				case SQLTypeEnum.Workshift:
					return new AddWorkshiftPage((AddWorkshiftViewModel)specificViewModel);
				default:
					throw new ArgumentException(message: "Invalid enum value.", paramName: nameof(type));
			}
		}

		public static BaseViewModel NewAddViewModel<T>(T item) where T : ISQLiteDataItem => NewAddViewModel(type: item.GetType(), item);
		private static BaseViewModel NewAddViewModel(SQLTypeEnum type) => NewAddViewModel(type, item: NewItem(type));
		private static BaseViewModel NewAddViewModel<T>(Type type, T item) where T : ISQLiteDataItem => NewAddViewModel(type: GetEnumValueFromType(type), item);
		private static BaseViewModel NewAddViewModel<T>(SQLTypeEnum type, T item) where T : ISQLiteDataItem
		{
			ISQLiteDataItem specificItem = item != null ? item : NewItem(type);
			switch (type)
			{
				case Models.SQLTypeEnum.Client:
					return new AddClientViewModel(specificItem.Id);
				case Models.SQLTypeEnum.Project:
					return new AddProjectViewModel(specificItem.Id);
				case Models.SQLTypeEnum.Workshift:
					return new AddWorkshiftViewModel(specificItem.Id);
				default:
					throw new ArgumentException(message: "Invalid enum value.", paramName: nameof(type));
			}
		}

		public static ContentPage NewDetailsPage(Type type, BaseViewModel viewModel = null) => NewDetailsPage(type: GetEnumValueFromType(type), viewModel);
		public static ContentPage NewDetailsPage(Models.SQLTypeEnum type, BaseViewModel viewModel = null)
		{
			BaseViewModel specificViewModel = viewModel ?? NewDetailsViewModel(type);
			switch (type)
			{
				case Models.SQLTypeEnum.Client:
					return new ClientDetailsPage((DetailsViewModel<Client>)specificViewModel);
				case Models.SQLTypeEnum.Project:
					return new ProjectDetailsPage((DetailsViewModel<Project>)specificViewModel);
				default:
					throw new ArgumentException(message: "Invalid enum value.", paramName: nameof(type));
			}
		}

		public static BaseViewModel NewDetailsViewModel<T>(T item) where T : ISQLiteDataItem => NewDetailsViewModel(type: item.GetType(), item);
		private static BaseViewModel NewDetailsViewModel(SQLTypeEnum type) => NewDetailsViewModel(type, item: NewItem(type));
		public static BaseViewModel NewDetailsViewModel<T>(Type type, T item) where T : ISQLiteDataItem => NewDetailsViewModel(GetEnumValueFromType(type), item);
		public static BaseViewModel NewDetailsViewModel<T>(SQLTypeEnum type, T item) where T : ISQLiteDataItem
		{
			ISQLiteDataItem specificItem = item != null ? item : NewItem(type);
			switch (type)
			{
				case Models.SQLTypeEnum.Client:
					return new DetailsViewModel<Client>((Client)specificItem);
				case Models.SQLTypeEnum.Project:
					return new DetailsViewModel<Project>((Project)specificItem);
				case Models.SQLTypeEnum.Workshift:
					return new DetailsViewModel<Workshift>((Workshift)specificItem);
				default:
					throw new ArgumentException(message: "Invalid enum value.", paramName: nameof(type));
			}
		}

		public static ContentPage NewListPage(Type type, BaseViewModel viewModel = null) => NewListPage(type: GetEnumValueFromType(type), viewModel);
		public static ContentPage NewListPage(Models.SQLTypeEnum type, BaseViewModel viewModel = null)
		{
			BaseViewModel specificViewModel = viewModel ?? NewListViewModel(type);
			switch (type)
			{
				case Models.SQLTypeEnum.Client:
					return new ClientsListPage((ListViewModel<Client>)specificViewModel);
				case Models.SQLTypeEnum.Workshift:
					return new WorkshiftsListPage((ListViewModel<Workshift>)specificViewModel);
				default:
					throw new ArgumentException(message: "Invalid enum value.", paramName: nameof(type));
			}
		}

		public static BaseViewModel NewListViewModel(Type listType, ISQLiteDataItem filterItem = null) => NewListViewModel(listType: GetEnumValueFromType(listType), filterItem);
		public static BaseViewModel NewListViewModel(Models.SQLTypeEnum listType, ISQLiteDataItem filterItem = null)
		{
			ISQLiteDataItem specificItem = filterItem;
			switch (listType)
			{
				case Models.SQLTypeEnum.Client:
					return new ListViewModel<Client>(filterItem);
				case Models.SQLTypeEnum.Project:
					return new ListViewModel<Project>(filterItem);
				case Models.SQLTypeEnum.Workshift:
					return new ListViewModel<Workshift>(filterItem);
				default:
					throw new ArgumentException(message: "Invalid enum value.", paramName: nameof(listType));
			}
		}
	}
}
