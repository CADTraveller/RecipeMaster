using Windows.UI.Xaml.Controls;

namespace RecipeMaster.Views
{
	public sealed partial class MainPage : Page
	{
		#region Public Constructors

		public MainPage()
		{
			InitializeComponent();
			NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
		}

		#endregion Public Constructors
	}
}