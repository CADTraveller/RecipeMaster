using System;

//using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using RecipeMaster.Helpers;
using System.Collections.ObjectModel;
using RecipeMaster.Models;
using Template10.Mvvm;

namespace RecipeMaster.ViewModels
{
    public class ActiveRecipesViewModel : ViewModelBase
	{
        public ActiveRecipesViewModel()
        {
            Messenger.Default.Register<RecipeSelectedMessage>(this, (message) => ReceiveMessage(message));
            activeRecipes = new ObservableCollection<Recipe>();

        } 

        private void ReceiveMessage(RecipeSelectedMessage message)
        {
            if (activeRecipes == null) activeRecipes = new ObservableCollection<Recipe>();
            ActiveRecipes.Add(message.SelectedRecipe);
        }

        private ObservableCollection<Recipe> activeRecipes;
        public ObservableCollection<Recipe> ActiveRecipes
        {
            get { return activeRecipes; }
            set { Set(ref activeRecipes, value); }
        }

        private Recipe selectedRecipe;
        public Recipe SelectedRecipe
        {
            get { return selectedRecipe; }
            set { Set(ref selectedRecipe, value); }
        }


    }
}
