using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Command;

using RecipeMaster.Models;
using RecipeMaster.Services;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Template10.Services;
using Template10.Mvvm;
using Template10.Services.NavigationService;

namespace RecipeMaster.ViewModels
{
    public class RecipeViewModel : ViewModelBase
	{
        const string NarrowStateName = "NarrowState";
        const string WideStateName = "WideState";

        private VisualState _currentState;

        public RecipeViewModel()
        {
            
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
    }
}
