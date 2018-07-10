using RecipeMaster.ViewModels;

using Windows.UI.Xaml.Controls;

namespace RecipeMaster.Views
{
	public sealed partial class ActiveRecipesPage : Page
	{
		#region Public Constructors

		public ActiveRecipesPage()
		{
			InitializeComponent();
		}

		#endregion Public Constructors

		#region Private Properties

		private ActiveRecipesViewModel ViewModel
		{
			get { return DataContext as ActiveRecipesViewModel; }
		}

		#endregion Private Properties
	}
}