using RecipeMaster.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using RecipeMaster.Models;
using RecipeMaster.Services;
using System.Collections.ObjectModel;
using Windows.Storage.AccessCache;
using Windows.UI.Notifications;
using Windows.Storage;
using Newtonsoft.Json;

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
				if (localSettings.Values.Count > 0)
				{
					populateRecentRecipeBoxList();
				}
			}
		}

		private void populateRecentRecipeBoxList()
		{
			if (!localSettings.Values.ContainsKey(storedRecentsKey)) return;

			string storedRecentsJson = localSettings.Values[storedRecentsKey].ToString();
			if (string.IsNullOrEmpty(storedRecentsJson)) return;

			try
			{
				List<RecentRecipeBox> storedRecents = JsonConvert.DeserializeObject<List<RecentRecipeBox>>(storedRecentsJson);
			}
			catch (Exception)//__entry is corrupt or wrong format
			{
				localSettings.Values.Remove(storedRecentsKey);
			}

		}

		private bool showRecentFiles;

		public bool ShowRecentFiles
		{
			get { return showRecentFiles; }
			set { Set(ref showRecentFiles, value); }
		}

		private bool showNoHistory;

		public bool ShowNoHistory
		{
			get
			{
				if (recentRecipeBoxes == null) return true;
				return recentRecipeBoxes.Count == 0;
			}
			//set { showNoHistory = value; }
		}

		public bool ShowHistory
		{
			get
			{
				if (recentRecipeBoxes == null) return false;
				return recentRecipeBoxes.Count > 0;
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

		private ObservableCollection<RecentRecipeBox> recentRecipeBoxes;

		public ObservableCollection<RecentRecipeBox> RecentRecipeBoxes
		{
			get { return recentRecipeBoxes; }
			set { Set(ref recentRecipeBoxes, value); }
		}

		private const string storedRecentsKey = "RecentRecipeBoxesList";

		#endregion


		#region Events
		public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
		{
			if (suspensionState.Any())
			{
				Value = suspensionState[nameof(Value)]?.ToString();
			}
			await Task.CompletedTask;

			//if (LocalSettings.Values.ContainsKey(settingsKey))
			//{

			//	try
			//	{
			//		string storedBoxesJson = LocalSettings.Values[settingsKey] as string;
			//		List<RecentRecipeBox> storedBoxes = Newtonsoft.Json.JsonConvert.DeserializeObject(storedBoxesJson) as List<RecentRecipeBox>;
			//		recentRecipeBoxes = new ObservableCollection<RecentRecipeBox>(storedBoxes);
			//	}
			//	catch (Exception)
			//	{
			//		var toastTemplate = ToastTemplateType.ToastText01;
			//		var toastTemplateXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
			//		var textNodes = toastTemplateXml.GetElementsByTagName("text");
			//		textNodes[0].AppendChild(toastTemplateXml.CreateTextNode("Could not unpack history"));

			//		var toast = new ToastNotification(toastTemplateXml);
			//		ToastNotificationManager.CreateToastNotifier().Show(toast);
			//	}

			//}

		}



		public ApplicationDataContainer LocalSettings { get => localSettings; set => localSettings = value; }

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
			SelectedRecipeBox = await FileIOService.OpenRecipeBoxFromFileAsync();

			//__make a record of this file for future executions	
			recordSelectedRecipeBox();
		}

		private void recordSelectedRecipeBox()
		{
			RecentRecipeBox boxJustOpened = new RecentRecipeBox()
			{
				Path = selectedRecipeBox.LastPath,
				Name = selectedRecipeBox.Name,
				LastOpened = DateTime.Now,
				Description = selectedRecipeBox.Description
			};

			//__add this to the displayed list of recents
			if (recentRecipeBoxes == null) recentRecipeBoxes = new ObservableCollection<RecentRecipeBox>();

			//__remove any existing reference
			if (RecentRecipeBoxes.Any(r => r.Name == boxJustOpened.Name))
			{
				var cleanedList = RecentRecipeBoxes.Where(r => r.Name != boxJustOpened.Name);
				RecentRecipeBoxes = cleanedList as ObservableCollection<RecentRecipeBox>;
			}
			RecentRecipeBoxes.Insert(0, boxJustOpened);

			//__record updated list
			List<RecentRecipeBox> recentsList = recentRecipeBoxes.ToList();
			string recentsJson = JsonConvert.SerializeObject(recentsList);
			localSettings.Values[storedRecentsKey] = recentsJson;
		}
		public void SaveFile()
		{

		}

		public async void NewRecipeBox()
		{
			RecipeBox newRecipeBox = await FileIOService.CreateNewRecipeBoxAsync();

			//__record this file or move it to top of recents list

			//__set as selected, go to details page
		}

		#endregion

	}
}
