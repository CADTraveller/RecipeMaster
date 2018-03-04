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
using Template10.Common;

namespace RecipeMaster.ViewModels
{
	public class RecipeViewModel : ViewModelBase
	{
		private const string NarrowStateName = "NarrowState";
		private const string WideStateName = "WideState";

		private VisualState _currentState;

		public RecipeViewModel()
		{
			ingredients = new ObservableCollection<Ingredient>();
			//CurrentRecipe = new Recipe();//__put a dummy in place to prevent errors
		}

		private RecipeBox _activeRecipeBox;
		private Recipe _currentRecipe;

		public Recipe CurrentRecipe

		{
			get { return _currentRecipe; }
			set
			{
				Set(ref _currentRecipe, value);
				Ingredients = _currentRecipe.Ingredients;
			}
		}

		private ObservableCollection<Ingredient> ingredients;

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

		private Ingredient selectedIngredient;

		public Ingredient SelectedIngredient
		{
			get { return selectedIngredient; }
			set { Set(ref selectedIngredient, value); }
		}

		//public async Task LoadDataAsync(VisualState currentState)
		//{
		//_currentState = currentState;
		//SampleItems.Clear();

		//var data = await SampleDataService.GetSampleModelDataAsync();

		//foreach (var item in data)
		//{
		//    SampleItems.Add(item);
		//}
		//Selected = SampleItems.First();
		//}

		//private void OnStateChanged(VisualStateChangedEventArgs args)
		//{
		//    _currentState = args.NewState;
		//}

		//private void OnItemClick(ItemClickEventArgs args)
		//{
		//    Order item = args?.ClickedItem as Order;
		//    if (item != null)
		//    {
		//        //if (_currentState.Name == NarrowStateName)
		//        //{
		//        //    NavigationService.Navigate(typeof(RecipesDetailViewModel).FullName, item);
		//        //}
		//        //else
		//        //{
		//        //    Selected = item;
		//        //}
		//    }
		//}

		public async Task NewIngredientAsync()
		{
			var dialog = new NewNamedItemDialog("Enter Ingredient Name");
			var result = await dialog.ShowAsync();

			Ingredient newIngredient = new Ingredient(dialog.TextEntry, IngredientType.Complex, CurrentRecipe);
			Ingredients.Add(newIngredient);
		}

		public async Task NewChildIngredientAsync()
		{
			var dialog = new NewNamedItemDialog("Enter Ingredient Name");
			var result = await dialog.ShowAsync();

			IIngredientContainer parent = SelectedIngredient ?? CurrentRecipe as IIngredientContainer;

			Ingredient newIngredient = new Ingredient(dialog.TextEntry, IngredientType.Complex, parent);
			parent.Ingredients.Add(newIngredient);
		}

		public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
		{
			string recipeBoxName = BootStrapper.Current.SessionState[App.ActiveRecipeBoxKey].ToString();
			_activeRecipeBox = BootStrapper.Current.SessionState[recipeBoxName] as RecipeBox;
			CurrentRecipe	= BootStrapper.Current.SessionState[App.SelectedRecipeKey] as Recipe;

			RaisePropertyChanged();
			return base.OnNavigatedToAsync(parameter, mode, state);
		}
	}
}