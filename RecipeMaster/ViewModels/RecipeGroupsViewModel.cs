using GalaSoft.MvvmLight.Messaging;
using RecipeMaster.Helpers;
using RecipeMaster.Models;
using RecipeMaster.Services;
using RecipeMaster.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RecipeMaster.ViewModels
{
	public class RecipeGroupsViewModel : ViewModelBase
	{
		public RecipeGroupsViewModel()
		{
			Messenger.Default.Register<RecipeBoxSelectedMessage>(this, (message) => ReceiveMessage(message));
			activeRecipeBox = BootStrapper.Current.SessionState[App.ActiveRecipeBoxKey] as RecipeBox;
		}

		#region Properties

		private const string NarrowStateName = "NarrowState";
		private const string WideStateName = "WideState";

		private VisualState _currentState;
		private RecipeBox activeRecipeBox;

		private bool recipeBoxOpen;

		public bool RecipeBoxOpen
		{
			get { return recipeBoxOpen; }
			set { Set(ref recipeBoxOpen, value); }
		}

		private bool recipeGroupIsSelected;

		public bool RecipeGroupIsSelected
		{
			get { return recipeGroupIsSelected; }
			set { Set(ref recipeGroupIsSelected, value); }
		}

		private ObservableCollection<RecipeGroup> currentRecipeGroups;

		public ObservableCollection<RecipeGroup> CurrentRecipeGroups
		{
			get
			{
				if (activeRecipeBox == null) return null;
				return currentRecipeGroups;
			}
			set
			{
				Set(ref currentRecipeGroups, value);
				RecipeGroupIsSelected = true;
			}
		}

		private RecipeGroup selectedRecipeGroup;

		public RecipeGroup SelectedRecipeGroup
		{
			get { return selectedRecipeGroup; }
			set
			{
				Set(ref selectedRecipeGroup, value);
				BootStrapper.Current.SessionState[App.SelectedRecipeGroupKey] = SelectedRecipeGroup;//__this might not be required, we shall see
			}
		}

		private Recipe selectedRecipe;

		public Recipe SelectedRecipe
		{
			get { return selectedRecipe; }
			set
			{
				Set(ref selectedRecipe, value);
				BootStrapper.Current.SessionState[App.SelectedRecipeKey] = value;
			}
		}

		private ObservableCollection<Recipe> currentRecipes;

		public ObservableCollection<Recipe> CurrentRecipes
		{
			get
			{
				return SelectedRecipeGroup.Recipes;
			}
			set
			{
				Set(ref currentRecipes, value);
				SelectedRecipeGroup.Recipes = currentRecipes;
				RaisePropertyChanged();
			}
		}

		#endregion Properties

		#region Methods

		private object ReceiveMessage(RecipeBoxSelectedMessage item)
		{
			activeRecipeBox = item.SelectedRecipeBox;
			if (currentRecipeGroups == null) currentRecipeGroups = new ObservableCollection<RecipeGroup>();
			// if (currentRecipes == null) currentRecipes = new ObservableCollection<Recipe>();
			CurrentRecipeGroups = activeRecipeBox.RecipeGroups;
			RecipeBoxOpen = true;
			return null;
		}

		public async Task NewRecipeGroupAsync()
		{
			NewNamedItemDialog dialog = new NewNamedItemDialog("Enter Group Name");
			var result = await dialog.ShowAsync();

			RecipeGroup newGroup = new RecipeGroup(dialog.TextEntry);
			CurrentRecipeGroups.Add(newGroup);
			RaisePropertyChanged(nameof(CurrentRecipeGroups));
		}

		public void GoToRecipeDetail()
		{
			if (SelectedRecipe == null) return;

			BootStrapper.Current.SessionState["ActiveRecipe"] = SelectedRecipe;
			NavigationService.Navigate(typeof(RecipeView));
		}

		public async Task NewRecipeAsync()
		{
			if (SelectedRecipeGroup == null)
			{
				//__prompt user to select destination Group
				IEnumerable<string> groupNames = CurrentRecipeGroups.Select(g => g.Name);
				SelectTargetGroupDialog groupSelector = new SelectTargetGroupDialog(groupNames);
				await groupSelector.ShowAsync();
				string groupName = groupSelector.SelectedGroupName;
				SelectedRecipeGroup = CurrentRecipeGroups.FirstOrDefault(g => g.Name == groupName);
			}
			if(SelectedRecipeGroup.Recipes is null) SelectedRecipeGroup.Recipes = new ObservableCollection<Recipe>();

			NewNamedItemDialog dialog = new NewNamedItemDialog("Enter Recipe Name");
			var result = await dialog.ShowAsync();

			//__quit quietly if dialog was cancelled
			if (dialog.WasCancelled) return;

			//__create new Recipe with entered Name
			Recipe newRecipe = new Recipe(dialog.TextEntry);
			SelectedRecipeGroup.Recipes.Add(newRecipe);
			SelectedRecipe = newRecipe;
			await SaveRecipeBoxAsync();
			GoToRecipeDetail();
		}

		public async Task SaveRecipeBoxAsync()
		{
			await FileIOService.SaveRecipeBoxAsync(activeRecipeBox);
		}

		#endregion Methods

		#region Sample Code

		public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
		{
			//Value = (suspensionState.ContainsKey(nameof(Value))) ? suspensionState[nameof(Value)]?.ToString() : parameter?.ToString();
			string recipeBoxName = BootStrapper.Current.SessionState[App.ActiveRecipeBoxKey].ToString();
			if (BootStrapper.Current.SessionState.Keys.Contains(recipeBoxName))
			{
				try
				{
					activeRecipeBox = BootStrapper.Current.SessionState[recipeBoxName] as RecipeBox;
					CurrentRecipeGroups = activeRecipeBox?.RecipeGroups;
				}
				catch (Exception e)
				{
					throw e;
				}
			}

			await Task.CompletedTask;
		}

		public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
		{
			NavigationService.SaveAsync();
			return base.OnNavigatingFromAsync(args);
		}

		#endregion Sample Code
	}
}

//public async Task LoadDataAsync(VisualState currentState)
//{
//    _currentState = currentState;
//    SampleItems.Clear();

//    var data = await SampleDataService.GetSampleModelDataAsync();

//    foreach (var item in data)
//    {
//        SampleItems.Add(item);
//    }
//    Selected = SampleItems.First();
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
//        if (_currentState.Name == NarrowStateName)
//        {
//            NavigationService.Navigate(typeof(RecipeGroupsDetailViewModel).FullName, item);
//        }
//        else
//        {
//            Selected = item;
//        }
//    }
//}