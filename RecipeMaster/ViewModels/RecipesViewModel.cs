using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using RecipeMaster.Models;
using RecipeMaster.Services;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Template10.Services;
using Template10.Mvvm;
using Template10.Services.NavigationService;

namespace RecipeMaster.ViewModels
{
    public class RecipesViewModel : Template10.Mvvm.ViewModelBase
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

        private VisualState _currentState;

        private Order _selected;
        public Order Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }

        public ICommand ItemClickCommand { get; private set; }
        public ICommand StateChangedCommand { get; private set; }

        public ObservableCollection<Order> SampleItems { get; private set; } = new ObservableCollection<Order>();

        public RecipesViewModel()
        {
            ItemClickCommand = new DelegateCommand<ItemClickEventArgs>(OnItemClick);
            StateChangedCommand = new DelegateCommand<VisualStateChangedEventArgs>(OnStateChanged);
        }

        public async Task LoadDataAsync(VisualState currentState)
        {
            //_currentState = currentState;
            //SampleItems.Clear();

            //var data = await SampleDataService.GetSampleModelDataAsync();

            //foreach (var item in data)
            //{
            //    SampleItems.Add(item);
            //}
            //Selected = SampleItems.First();
        }

        private void OnStateChanged(VisualStateChangedEventArgs args)
        {
            _currentState = args.NewState;
        }

        private void OnItemClick(ItemClickEventArgs args)
        {
            Order item = args?.ClickedItem as Order;
            if (item != null)
            {
                //if (_currentState.Name == NarrowStateName)
                //{
                //    NavigationService.Navigate(typeof(RecipesDetailViewModel).FullName, item);
                //}
                //else
                //{
                //    Selected = item;
                //}
            }
        }
    }
}
