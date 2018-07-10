using RecipeMaster.Models;
using RecipeMaster.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinRTXamlToolkit.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RecipeMaster.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class RecipeView : Page
	{
		#region Public Constructors

		public RecipeView()
		{
			InitializeComponent();
		}

		#endregion Public Constructors

		#region Private Properties

		private RecipeViewModel Vm
		{
			get => DataContext as RecipeViewModel;
		}

		#endregion Private Properties

		#region Private Methods

		private async void Ingredient_OnClick(object sender, RoutedEventArgs e)
		{
			await Vm.NewChildIngredientAsync();
		}

		private void IngredientsTreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			try
			{
				Ingredient selectedIngredient = (Ingredient)IngredientsTreeView.SelectedItem;
				if (selectedIngredient != null)
				{
					Vm.SelectedIngredient = selectedIngredient;
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
				throw;
			}
		}

		private void TextBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			e.Handled = true;
		}

		#endregion Private Methods
	}
}