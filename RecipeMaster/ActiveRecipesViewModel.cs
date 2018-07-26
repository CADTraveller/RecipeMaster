//using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using RecipeMaster.Helpers;
using RecipeMaster.Models;
using System.Collections.ObjectModel;

namespace RecipeMaster.ViewModels
{
	public class ActiveRecipesViewModel : Mvvm.ViewModelBase
	{
		#region Public Constructors

		public ActiveRecipesViewModel()
		{
			Messenger.Default.Register<RecipeSelectedMessage>(this, (message) => ReceiveMessage(message));
			activeRecipes = new ObservableCollection<Recipe>();
		}

		#endregion Public Constructors

		#region Public Properties

		public ObservableCollection<Recipe> ActiveRecipes
		{
			get { return activeRecipes; }
			set { Set(ref activeRecipes, value); }
		}

		public Recipe SelectedRecipe
		{
			get { return selectedRecipe; }
			set { Set(ref selectedRecipe, value); }
		}

		#endregion Public Properties

		#region Private Fields

		private ObservableCollection<Recipe> activeRecipes;

		private Recipe selectedRecipe;

		#endregion Private Fields

		#region Private Methods

		private void ReceiveMessage(RecipeSelectedMessage message)
		{
			if (activeRecipes == null)
			{
				activeRecipes = new ObservableCollection<Recipe>();
			}

			ActiveRecipes.Add(message.SelectedRecipe);
		}

		#endregion Private Methods
	}
}