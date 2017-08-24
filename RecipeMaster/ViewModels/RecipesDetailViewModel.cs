using System;
using System.Windows.Input;

using GalaSoft.MvvmLight.Command;

using RecipeMaster.Models;
using RecipeMaster.Services;

using Windows.UI.Xaml;
using Template10.Mvvm;
using Template10.Services.NavigationService;

namespace RecipeMaster.ViewModels
{
    public class RecipesDetailViewModel : ViewModelBase
    {

        //public NavigationService NavigationService
        //{
        //    get
        //    {
        //        return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationService>();
        //    }
        //}
        const string NarrowStateName = "NarrowState";
        const string WideStateName = "WideState";

        public ICommand StateChangedCommand { get; private set; }

        private Order _item;
        public Order Item
        {
            get { return _item; }
            set { Set(ref _item, value); }
        }

        public RecipesDetailViewModel()
        {
            StateChangedCommand = new DelegateCommand<VisualStateChangedEventArgs>(OnStateChanged);
        }
        
        private void OnStateChanged(VisualStateChangedEventArgs args)
        {
            if (args.OldState.Name == NarrowStateName && args.NewState.Name == WideStateName)
            {
                NavigationService.GoBack();
            }
        }
    }
}
