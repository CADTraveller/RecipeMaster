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
		#region Public Constructors

		public RecipeGroupsViewModel()
		{
			Messenger.Default.Register<RecipeBoxSelectedMessage>(this, (message) => ReceiveMessage(message));
		}

		#endregion Public Constructors

		#region Properties

		public ObservableCollection<RecipeGroup> CurrentRecipeGroups
		{
			get
			{
				if (activeRecipeBox == null)
				{
					return null;
				}

				return currentRecipeGroups;
			}
			set
			{
				Set(ref currentRecipeGroups, value);
				RecipeGroupIsSelected = true;
			}
		}

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

		public bool RecipeBoxOpen
		{
			get { return recipeBoxOpen; }
			set { Set(ref recipeBoxOpen, value); }
		}

		public bool RecipeGroupIsSelected
		{
			get { return recipeGroupIsSelected; }
			set { Set(ref recipeGroupIsSelected, value); }
		}

		public Recipe SelectedRecipe
		{
			get { return selectedRecipe; }
			set
			{
				Set(ref selectedRecipe, value);
				BootStrapper.Current.SessionState[App.SelectedRecipeKey] = value;
			}
		}

		public RecipeGroup SelectedRecipeGroup
		{
			get { return selectedRecipeGroup; }
			set
			{
				Set(ref selectedRecipeGroup, value);
				BootStrapper.Current.SessionState[App.SelectedRecipeGroupKey] = SelectedRecipeGroup;//__this might not be required, we shall see
			}
		}

		private const string NarrowStateName = "NarrowState";
		private const string WideStateName = "WideState";

		private VisualState _currentState;
		private RecipeBox activeRecipeBox;

		private ObservableCollection<RecipeGroup> currentRecipeGroups;
		private ObservableCollection<Recipe> currentRecipes;
		private bool recipeBoxOpen;
		private bool recipeGroupIsSelected;
		private Recipe selectedRecipe;
		private RecipeGroup selectedRecipeGroup;

		#endregion Properties

		#region Methods

		public void GoToRecipeDetail()
		{
			if (SelectedRecipe == null)
			{
				return;
			}

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

			NewNamedItemDialog dialog = new NewNamedItemDialog("Enter Recipe Name");
			ContentDialogResult result = await dialog.ShowAsync();

			//__quit quietly if dialog was cancelled
			if (dialog.WasCancelled)
			{
				return;
			}

			//__create new Recipe with entered Name
			Recipe newRecipe = new Recipe(dialog.TextEntry);
			SelectedRecipeGroup.Recipes.Add(newRecipe);
			SelectedRecipe = newRecipe;
			await SaveRecipeBoxAsync();
			GoToRecipeDetail();
		}

		public async Task NewRecipeGroupAsync()
		{
			NewNamedItemDialog dialog = new NewNamedItemDialog("Enter Group Name");
			ContentDialogResult result = await dialog.ShowAsync();

			RecipeGroup newGroup = new RecipeGroup(dialog.TextEntry);
			CurrentRecipeGroups.Add(newGroup);
		}

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

		public async Task SaveRecipeBoxAsync()
		{
			await FileIOService.SaveRecipeBoxAsync(activeRecipeBox);
		}

		private object ReceiveMessage(RecipeBoxSelectedMessage item)
		{
			activeRecipeBox = item.SelectedRecipeBox;
			if (currentRecipeGroups == null)
			{
				currentRecipeGroups = new ObservableCollection<RecipeGroup>();
			}
			// if (currentRecipes == null) currentRecipes = new ObservableCollection<Recipe>();
			CurrentRecipeGroups = activeRecipeBox.RecipeGroups;
			RecipeBoxOpen = true;
			return null;
		}

		#endregion Methods
	}
}