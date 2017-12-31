using RecipeMaster.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace RecipeMaster.Views
{
    public sealed partial class RecipeGroupsPage : Page
    {
        private RecipeGroupsViewModel ViewModel
        {
            get { return DataContext as RecipeGroupsViewModel; }
        }

        public RecipeGroupsPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadDataAsync(WindowStates.CurrentState);
        }

        public  void RecipeGroup_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

        private void TypeMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem selection = sender as MenuFlyoutItem;
            string selectedText = selection.Text;

        }
    }
}
