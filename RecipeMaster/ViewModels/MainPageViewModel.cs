//using RecipeMaster.Mvvm;
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
using Template10.Mvvm;

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






		#region Properties
		Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

		string _Value = "Gas";
		public string Value { get { return _Value; } set { Set(ref _Value, value); } }


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

		private RecipeBox currentRecipeBox;

		private string statusMessage = "Nothing Selected";
		public string StatusMessage
		{
			get { return statusMessage; }
			set { Set(ref statusMessage, value); }
		}

		private RecentRecipeBox selectedRecentRecipeBox;
		public RecentRecipeBox SelectedRecentRecipeBox
		{
			get { return selectedRecentRecipeBox; }
			set
			{
				Set(ref selectedRecentRecipeBox, value);
				StatusMessage = selectedRecentRecipeBox.Description;
			}
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

		#region Commands
		private DelegateCommand viewRecipeBoxDetailsCommand;
		public DelegateCommand ViewRecipeBoxDetailsCommand => viewRecipeBoxDetailsCommand ?? (viewRecipeBoxDetailsCommand = new DelegateCommand(() => GotoDetailsPage()));

		#endregion

		#region Methods
		//public void GotoDetailsPage() =>
		//	NavigationService.Navigate(typeof(Views.DetailPage), Value);

		public void GotoDetailsPage() =>
			 NavigationService.Navigate(typeof(Views.DetailPage), SelectedRecentRecipeBox);

		public void GotoSettings() =>
			NavigationService.Navigate(typeof(Views.SettingsPage), 0);

		public void GotoPrivacy() =>
			NavigationService.Navigate(typeof(Views.SettingsPage), 1);

		public void GotoAbout() =>
			NavigationService.Navigate(typeof(Views.SettingsPage), 2);

		public async void OpenFile()
		{
			currentRecipeBox = await FileIOService.OpenRecipeBoxFromFileAsync();
			SelectedRecentRecipeBox = CreateRecentRecipeBox(currentRecipeBox);

			//__make a record of this file for future executions	
			recordSelectedRecipeBox();
		}

		private RecentRecipeBox CreateRecentRecipeBox(RecipeBox recipeBox)
		{
			RecentRecipeBox rrb = new RecentRecipeBox()
			{
				Path = recipeBox.LastPath,
				Name = recipeBox.Name,
				LastOpened = DateTime.Now,
				Description = recipeBox.Description
			};
			return rrb;
		}

		private void recordSelectedRecipeBox()
		{
			if (currentRecipeBox == null) return;

			//__add this to the displayed list of recents
			if (recentRecipeBoxes == null) recentRecipeBoxes = new ObservableCollection<RecentRecipeBox>();

			//__remove any existing reference
			if (RecentRecipeBoxes.Any(r => r.Name == selectedRecentRecipeBox.Name))
			{
				var cleanedList = RecentRecipeBoxes.Where(r => r.Name != selectedRecentRecipeBox.Name);
				RecentRecipeBoxes = cleanedList as ObservableCollection<RecentRecipeBox>;
			}
			RecentRecipeBoxes.Insert(0, selectedRecentRecipeBox);

			//__record updated list
			List<RecentRecipeBox> recentsList = recentRecipeBoxes.ToList();
			string recentsJson = JsonConvert.SerializeObject(recentsList);
			localSettings.Values[storedRecentsKey] = recentsJson;
		}

		private void populateRecentRecipeBoxList()
		{
			if (!localSettings.Values.ContainsKey(storedRecentsKey))
			{
				RaisePropertyChanged("ShowNoHistory");
				return;
			}

			string storedRecentsJson = localSettings.Values[storedRecentsKey].ToString();
			if (string.IsNullOrEmpty(storedRecentsJson)) return;

			try
			{
				List<RecentRecipeBox> storedRecents = JsonConvert.DeserializeObject<List<RecentRecipeBox>>(storedRecentsJson);
				if (recentRecipeBoxes != null) RecentRecipeBoxes.Clear();

				RecentRecipeBoxes = new ObservableCollection<RecentRecipeBox>(storedRecents);
				RaisePropertyChanged("ShowHistory");
			}
			catch (Exception)//__entry is corrupt or wrong format
			{
				localSettings.Values.Remove(storedRecentsKey);
			}

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
