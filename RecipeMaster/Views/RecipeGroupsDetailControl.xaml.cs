using RecipeMaster.Models;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RecipeMaster.Views
{
    public sealed partial class RecipeGroupsDetailControl : UserControl
    {
        public Order MasterMenuItem
        {
            get { return GetValue(MasterMenuItemProperty) as Order; }
            set { SetValue(MasterMenuItemProperty, value); }
        }

        public static DependencyProperty MasterMenuItemProperty = DependencyProperty.Register("MasterMenuItem",
            typeof(Order),typeof(RecipeGroupsDetailControl),new PropertyMetadata(null));


        public RecipeGroupsDetailControl()
        {
            InitializeComponent();
        }
    }
}
