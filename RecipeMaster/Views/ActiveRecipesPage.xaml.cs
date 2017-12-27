using RecipeMaster.ViewModels;

using Windows.UI.Xaml.Controls;

namespace RecipeMaster.Views
{
    public sealed partial class ActiveRecipesPage : Page
    {
        private ActiveRecipesViewModel ViewModel
        {
            get { return DataContext as ActiveRecipesViewModel; }
        }

        public ActiveRecipesPage()
        {
            InitializeComponent();
        }
    }
}
