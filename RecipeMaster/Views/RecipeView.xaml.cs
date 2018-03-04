using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using RecipeMaster.Models;
using RecipeMaster.ViewModels;
using WinRTXamlToolkit.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RecipeMaster.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class RecipeView : Page
	{
	    RecipeViewModel Vm
	    {
            get => DataContext as RecipeViewModel;
	    }

        public RecipeView()
		{
			InitializeComponent();
		}

	    private void IngredientsTreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	    {
	        
	        try
	        {
	            Ingredient selectedIngredient = (Ingredient)IngredientsTreeView.SelectedItem;
	            if(selectedIngredient != null) Vm.SelectedIngredient = selectedIngredient;
	        }
	        catch (Exception exception)
	        {
	            Console.WriteLine(exception);
	            throw;
	        }
	    }

	    private async void Ingredient_OnClick(object sender, RoutedEventArgs e)
	    {
	        await Vm.NewChildIngredientAsync();
	    }

	    
	}
}
