using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using RecipeMaster.Models;
using RecipeMaster.Services;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using RecipeMaster.Helpers;
using RecipeMaster.Views;
using Template10.Mvvm;

namespace RecipeMaster.ViewModels
{
    public class RecipeGroupsViewModel : Template10.Mvvm.ViewModelBase
	{
        public RecipeGroupsViewModel()
        {
            //ItemClickCommand = new DelegateCommand<ItemClickEventArgs>(OnItemClick);
            //StateChangedCommand = new DelegateCommand<VisualStateChangedEventArgs>(OnStateChanged);
            //RecipeGroupSelectedCommand = new DelegateCommand(OnRecipeGroupSelected);
            NewRecipeGroupCommand = new DelegateCommand(NewRecipeGroup);

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

        //private ObservableCollection<Recipe> currentRecipes;
        //public ObservableCollection<Recipe> CurrentRecipes
        //{
        //    get
        //    {

        //        return currentRecipes;
        //    }
        //    set
        //    {
        //        Set(ref currentRecipes, value);
        //        RaisePropertyChanged("CurrentRecipeGroups");
        //    }
        //}



        #endregion

        #region Commands
        private Order _selected;
        public Order Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }

        public ICommand ItemClickCommand { get; private set; }
        public ICommand StateChangedCommand { get; private set; }
        public ICommand RecipeGroupSelectedCommand { get; private set; }
        public ICommand NewRecipeGroupCommand { get; private set; }
        public ICommand RecipeSelectedCommand { get; private set; }


        public ObservableCollection<Order> SampleItems { get; private set; } = new ObservableCollection<Order>();




        private object ReceiveMessage(RecipeBoxSelectedMessage item)
        {
            activeRecipeBox = item.SelectedRecipeBox;
            if (currentRecipeGroups == null) currentRecipeGroups = new ObservableCollection<RecipeGroup>();
            // if (currentRecipes == null) currentRecipes = new ObservableCollection<Recipe>();
            CurrentRecipeGroups = activeRecipeBox.RecipeGroups;
            RecipeBoxOpen = true;
            return null;
        }

        private void NewRecipeGroup()
        {
            NewNamedItemDialog dialog = new NewNamedItemDialog("Enter Group Name");
            var result = dialog.ShowAsync();

            RecipeGroup newGroup = new RecipeGroup(dialog.TextEntry);
            CurrentRecipeGroups.Add(newGroup);
        }

        private DelegateCommand newRecipeCommand;
        public DelegateCommand NewRecipeCommand =>
            newRecipeCommand ?? (newRecipeCommand = new DelegateCommand(async () => await CreateNewRecipeAsync()));
        public async Task CreateNewRecipeAsync()
        {
            NewNamedItemDialog dialog = new NewNamedItemDialog("Enter Recipe Name");
            var result = await dialog.ShowAsync();
            if (dialog.WasCancelled) return;
            Recipe newRecipe = new Recipe(dialog.TextEntry);
            SelectedRecipeGroup.Recipes.Add(newRecipe);

        }

        private DelegateCommand activateRecipeCommand;
        public DelegateCommand ActivateRecipeCommand =>
            activateRecipeCommand ??
            (activateRecipeCommand = new DelegateCommand(async () => await ActivateRecipe()));

        public async Task ActivateRecipe()
        {
            if (selectedRecipe == null) return;
            RecipeSelectedMessage message = new RecipeSelectedMessage() { SelectedRecipe = selectedRecipe };
            Messenger.Default.Send(message);
        }

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
        #endregion
    }
}
