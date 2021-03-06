//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Command;

using RecipeMaster.Models;
using RecipeMaster.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using RecipeMaster.Services;
using Template10.Common;

namespace RecipeMaster.ViewModels
{
	using Windows.UI.Xaml.Controls;

	public class RecipeViewModel : ViewModelBase
	{
		private const string NarrowStateName = "NarrowState";
		private const string WideStateName = "WideState";

		private RecipeBox _activeRecipeBox;
		private Recipe _currentRecipe;
		private VisualState _currentState;
		private bool _entryModeActive;

		private ObservableCollection<Ingredient> ingredients;

		private Ingredient selectedIngredient;

		public bool EntryModeActive
		{
			get => _entryModeActive;
			set
			{
				if(_entryModeActive == value) return;
				Set(ref _entryModeActive, value);
				RaisePropertyChanged("EditModeActive");
				foreach (Ingredient ingredient in Ingredients)
				{
					ingredient.EntryModeActive = value;
				}
			}
		}

		public bool EditModeActive
		{
			get => !_entryModeActive;
			set
			{
				if(!_entryModeActive == value) return;
				Set(ref _entryModeActive, !value);
				RaisePropertyChanged("EntryModeActive");
				foreach (Ingredient ingredient in Ingredients)
				{
					ingredient.EntryModeActive = !value;
				}
			}
		}
		public RecipeViewModel()
		{
			ingredients = new ObservableCollection<Ingredient>();

		}
		public Recipe CurrentRecipe

		{
			get { return _currentRecipe; }
			set
			{
				Set(ref _currentRecipe, value);
				Ingredients = _currentRecipe.Ingredients;
			}
		}
		public ObservableCollection<Ingredient> Ingredients
		{
			get
			{
				if (_currentRecipe == null) return null;
				return _currentRecipe.Ingredients;
			}

			set
			{
				Set(ref ingredients, value);
				CurrentRecipe.Ingredients = ingredients;
				
			}
		}
		public Ingredient SelectedIngredient
		{
			get { return selectedIngredient; }
			set { Set(ref selectedIngredient, value); }
		}


		public async Task NewChildIngredientAsync()
		{
			if (SelectedIngredient is null) return;
			var dialog = new NewNamedItemDialog("Enter Ingredient Name");
			var result = await dialog.ShowAsync();

			
			Ingredient newIngredient = new Ingredient(dialog.TextEntry, IngredientType.Complex);
			SelectedIngredient.Ingredients.Add(newIngredient);
			SelectedIngredient.LinkChildEvents(newIngredient);
		}

		public async Task NewIngredientAsync()
		{
			var dialog = new NewNamedItemDialog("Enter Ingredient Name");
			var result = await dialog.ShowAsync();
			if(result == ContentDialogResult.Secondary) return;

			var vm = dialog.DataContext as NewIngredientViewModel;
			Ingredient newIngredient = new Ingredient(vm.Name, vm.ConvertedType);
			CurrentRecipe.AddIngredient(newIngredient);
			RaisePropertyChanged("Ingredients");
		}

		public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
		{
			CurrentRecipe = BootStrapper.Current.SessionState[App.SelectedRecipeKey] as Recipe;

			RaisePropertyChanged();
			return base.OnNavigatedToAsync(parameter, mode, state);
		}

		public async Task Save()
		{
			await FileIOService.SaveRecipeBoxAsync(_activeRecipeBox);
		}
	}
}