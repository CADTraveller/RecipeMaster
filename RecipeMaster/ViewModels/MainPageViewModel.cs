using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using RecipeMaster.Models;
using System.Collections.ObjectModel;
using Windows.Storage.AccessCache;

namespace RecipeMaster.ViewModels
{
	public class MainPageViewModel : ViewModelBase
	{
		public MainPageViewModel()
		{
			if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				Value = "Designtime value";
				return;
			}

			else
			{
				
			}


		}


		#region Properties
		Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

		string _Value = "Gas";
		public string Value { get { return _Value; } set { Set(ref _Value, value); } }

		private RecipeBox selectedRecipeBox;
		public RecipeBox SelectedRecipeBox
		{
			get { return selectedRecipeBox; }
			set { Set(ref selectedRecipeBox, value); }
		}




		#endregion


		#region Events
		public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
		{
			if (suspensionState.Any())
			{
				Value = suspensionState[nameof(Value)]?.ToString();
			}
			await Task.CompletedTask;

			if (localSettings.Values.ContainsKey("recentRecipeBoxes"))
			{
				recentRecipeBoxes = localSettings.Values["recentRecipeBoxes"] as List<RecentRecipeBox>;
			}

		}

		private List<RecentRecipeBox> recentRecipeBoxes;

		public List<RecentRecipeBox> RecentRecipeBoxes
		{
			get { return recentRecipeBoxes; }
			set { Set(ref recentRecipeBoxes, value); }
		}

		public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
		{
			if (suspending)
			{
				suspensionState[nameof(Value)] = Value;
			}
			await Task.CompletedTask;
		}

		public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
		{
			args.Cancel = false;
			await Task.CompletedTask;
		}
		#endregion

		#region Methods
		public void GotoDetailsPage() =>
			NavigationService.Navigate(typeof(Views.DetailPage), Value);

		public void GotoSettings() =>
			NavigationService.Navigate(typeof(Views.SettingsPage), 0);

		public void GotoPrivacy() =>
			NavigationService.Navigate(typeof(Views.SettingsPage), 1);

		public void GotoAbout() =>
			NavigationService.Navigate(typeof(Views.SettingsPage), 2);

		public async void OpenFile()
		{
			SelectedRecipeBox = await Services.FileIOService.OpenRecipeBoxFromFileAsync();

			//__make a record of this file for future executions
			if (localSettings.Values.ContainsKey("recentRecipeBoxes"))
			{
				recentRecipeBoxes = localSettings.Values["recentRecipeBoxes"] as List<RecentRecipeBox>;
			}
		}

		public void SaveFile()
		{

		}

		public void NewRecipeBox()
		{

		}

		#endregion

	}
}
