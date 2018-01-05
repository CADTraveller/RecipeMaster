using System;
using RecipeMaster.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

namespace RecipeMaster.Views
{
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			InitializeComponent();
			NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
		}

		private void AppBarButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void GoToDetails()
		{
			ViewModel.GotoDetailsPage();
		}
	}
}