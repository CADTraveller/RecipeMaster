using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Command;

using RecipeMaster.Models;
using RecipeMaster.Services;
using RecipeMaster.Views;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Template10.Services;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;

namespace RecipeMaster.ViewModels
{
    public class RecipeViewModel : ViewModelBase
	{
        const string NarrowStateName = "NarrowState";
        const string WideStateName = "WideState";

        private VisualState _currentState;

        public RecipeViewModel()
        {
			ingredients = new ObservableCollection<Ingredient>();
			//CurrentRecipe = new Recipe();//__put a dummy in place to prevent errors
        }

		private Recipe currentRecipe;
		public Recipe CurrentRecipe

		{
			get { return currentRecipe; }
			set
			{
				Set(ref currentRecipe, value);
				Ingredients = currentRecipe.Ingredients;
			}
		}


		private ObservableCollection<Ingredient> ingredients;
		public ObservableCollection<Ingredient> Ingredients
		{
			get
			{
				if (currentRecipe == null) return null;
				return currentRecipe.Ingredients;
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

			Ingredient newIngredient = new Ingredient(dialog.TextEntry, IngredientType.Complex, SelectedIngredient);
			Ingredients.Add(newIngredient);
		}

		public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
		{
			CurrentRecipe = parameter as Recipe;
			RaisePropertyChanged();
			return base.OnNavigatedToAsync(parameter, mode, state);
		}
	}
}
