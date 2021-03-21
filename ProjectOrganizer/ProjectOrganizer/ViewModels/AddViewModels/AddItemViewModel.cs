using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using ProjectOrganizer.Interfaces;
using ProjectOrganizer.Repositories;
using ProjectOrganizer.Models;
using ProjectOrganizer.Validators;
using Xamarin.Forms;
using System.Diagnostics;

namespace ProjectOrganizer.ViewModels
{

	public abstract class AddItemViewModel<T> : BaseViewModel where T : ISQLiteDataItem
	{
		private bool _isItemInError;
		public bool IsItemInError
		{
			get => _isItemInError;
			set => SetProperty(ref _isItemInError, value);
		}

		private string _errorMessage;
		public string ErrorMessage
		{
			get
			{
				if (_errorMessage == null)
				{
					_errorMessage = "";
				}
				return _errorMessage;
			}
			set
			{
				SetProperty(ref _errorMessage, value);
				IsItemInError = !string.IsNullOrEmpty(_errorMessage);
			}
		}

		protected T _item;
		public T Item
		{
			get
			{
				Debug.WriteLine($"Item retrieved. Item is {(_item == null ? "Null" : "Not Null")}. ItemId = {_item?.Id ?? 0}.");
				return _item;
			}
			set
			{
				Debug.WriteLine($"Item set. Item is {(_item == null ? "Null" : "Not Null")}. ItemId = {_item?.Id ?? 0}. Value is {value}.");
				SetProperty(ref _item, value);
			}
		}

		public Command SaveCommand { get; set; }
		protected IValidator<T> validator;

		protected IItemRepository<T> repository;

		public AddItemViewModel(int itemId = 0)
		{
			SaveCommand = new Command(
				execute: async () => await SaveItemToDatabaseAsync().ConfigureAwait(false),
				canExecute: () => !IsItemInError
				);

			repository = RepositoryFactory.Get<T>();

			if (itemId > 0)
			{
				Item = repository.GetItemAsync(itemId).Result;
				Title = $"Edit {typeof(T).Name}";
			} else
			{
				Item = (T)SQLFactory.NewItem(typeof(T));
				Title = $"New {typeof(T).Name}";
			}
		}

		public AddItemViewModel(T item) : this(item.Id) { }

		public void RefreshCommands() => SaveCommand.ChangeCanExecute();

		public virtual async Task<ValidationResult> ValidateItem()
		{
			ValidationResult result = await validator.ValidateAsync(Item).ConfigureAwait(false);
			ErrorMessage = result.ToString();

			return result;
		}

		public virtual async Task<bool> SaveItemToDatabaseAsync()
		{
			ValidationResult result = await ValidateItem().ConfigureAwait(false);

			if (result.IsValid)
			{
				try
				{
					await repository.SaveItemAsync(Item).ConfigureAwait(false);
					return true;
				} catch (Exception e)
				{
					ErrorMessage = $"There was a problem with saving the item. {e.Message}";
				}
			}

			return false;
		}

		/// <summary>
		/// To be called during constructor if any initializion is required.
		/// Needs to create concrete validator and run ValidateItem.Wait().
		/// </summary>
		protected abstract void Initialize();
	}
}
