using RecipeMaster.Models;
using RecipeMaster.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RecipeMaster.Views
{
    public sealed partial class RecipeGroupsDetailPage : Page
    {
        private RecipeGroupsDetailViewModel ViewModel
        {
            get { return DataContext as RecipeGroupsDetailViewModel; }
        }

        public RecipeGroupsDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.Item = e.Parameter as Order;
        }
    }
}
