using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Template10.Mvvm;
using RecipeMaster.Models;
using RecipeMaster.Services;

using Windows.UI.Xaml;
using RecipeMaster.Helpers;
using RecipeMaster.Views;
using Windows.UI.Xaml.Navigation;
using System.Collections.Generic;

namespace RecipeMaster.ViewModels
{
    public class RecipeGroupsViewModel : ViewModelBase
    {
        public RecipeGroupsViewModel()
        {
            //ItemClickCommand = new DelegateCommand<ItemClickEventArgs>(OnItemClick);
            //StateChangedCommand = new DelegateCommand<VisualStateChangedEventArgs>(OnStateChanged);
            //RecipeGroupSelectedCommand = new DelegateCommand(OnRecipeGroupSelected);
            //NewRecipeGroupCommand = new DelegateCommand(NewRecipeGroup);

            Messenger.Default.Register<RecipeBoxSelectedMessage>(this, (message) => ReceiveMessage(message));
        }

        #region Properties
        //public NavigationServiceEx NavigationService
        //{
        //    get
        //    {
        //        return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationServiceEx>();
        //    }
        //}

        const string NarrowStateName = "NarrowState";
        const string WideStateName = "WideState";

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
                RaisePropertyChanged("CurrentRecipes");
            }
        }

        private Recipe selectedRecipe;
        public Recipe SelectedRecipe
        {
            get { return selectedRecipe; }
            set { Set(ref selectedRecipe, value); }
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



        #endregion

        #region Methods


        //public ICommand ItemClickCommand { get; private set; }
        //public ICommand StateChangedCommand { get; private set; }
        //public ICommand RecipeGroupSelectedCommand { get; private set; }
        //public ICommand NewRecipeGroupCommand { get; private set; }
        //public ICommand RecipeSelectedCommand { get; private set; }


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
        }

        public void GoToRecipeDetail()
        {
            if (SelectedRecipe == null) return;
            NavigationService.Navigate(typeof(RecipeView), SelectedRecipe);
        }

        public async Task NewRecipeAsync()
        {
            if (SelectedRecipeGroup == null) return;

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
        //private DelegateCommand newRecipeCommand;
        //public DelegateCommand NewRecipeCommand =>
        //	newRecipeCommand ?? (newRecipeCommand = new DelegateCommand(async () => await CreateNewRecipeAsync()));
        //public async Task CreateNewRecipeAsync()
        //{
        //	NewNamedItemDialog dialog = new NewNamedItemDialog("Enter Recipe Name");
        //	var result = await dialog.ShowAsync();
        //	if (dialog.WasCancelled) return;
        //	Recipe newRecipe = new Recipe(dialog.TextEntry);
        //	SelectedRecipeGroup.Recipes.Add(newRecipe);

        //}

        //private DelegateCommand activateRecipeCommand;
        //public DelegateCommand ActivateRecipeCommand =>
        //	activateRecipeCommand ??
        //	(activateRecipeCommand = new DelegateCommand(async () => await ActivateRecipe()));

        //public async Task ActivateRecipe()
        //{
        //	if (selectedRecipe == null) return;
        //	RecipeSelectedMessage message = new RecipeSelectedMessage() { SelectedRecipe = selectedRecipe };
        //	Messenger.Default.Send(message);
        //}

        #endregion

        #region Sample Code

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

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            //Value = (suspensionState.ContainsKey(nameof(Value))) ? suspensionState[nameof(Value)]?.ToString() : parameter?.ToString();

            try
            {
                activeRecipeBox = parameter as RecipeBox;
                CurrentRecipeGroups = activeRecipeBox.RecipeGroups;
            }
            catch (Exception e)
            {

                throw e;
            }

            await Task.CompletedTask;
        }
        #endregion
    }
}
