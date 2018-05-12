//using RecipeMaster.Mvvm;
using RecipeMaster.Models;
using RecipeMaster.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;

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
				//FileIOService.ClearHistory();
				populateRecentRecipeBoxListAsync();
				
			}
		}

		#region Properties

		private Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
		
		private string _Value = "Gas";

		public string Value
		{
			get { return _Value; }
			set => Set(ref _Value, value);
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


		#endregion Properties

		#region Events

		public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
		{
			if (suspensionState.Any())
			{
				Value = suspensionState[nameof(Value)]?.ToString();
			}
			await Task.CompletedTask;
			
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

		#endregion Events

		#region Commands

		private DelegateCommand viewRecipeBoxDetailsCommand;
		public DelegateCommand ViewRecipeBoxDetailsCommand => viewRecipeBoxDetailsCommand ?? (viewRecipeBoxDetailsCommand = new DelegateCommand(() => GotoRecipeGroupsView()));

		#endregion Commands

		#region Methods

		//public void GotoDetailsPage() =>
		//	NavigationService.Navigate(typeof(Views.DetailPage), Value);

		public async Task GotoRecipeGroupsView()
		{
			if (SelectedRecentRecipeBox == null) return;
			string recipeBoxName = SelectedRecentRecipeBox.Name;

			//__see if this RecipeBox is already in memory
			if (!BootStrapper.Current.SessionState.Keys.Contains(recipeBoxName))
			{
				currentRecipeBox = await FileIOService.OpenRecipeBoxAsync(SelectedRecentRecipeBox);
				BootStrapper.Current.SessionState[recipeBoxName] = currentRecipeBox;
			}
			
			BootStrapper.Current.SessionState[App.ActiveRecipeBoxKey] = recipeBoxName;
			if (currentRecipeBox != null)
			{
				NavigationService.Navigate(typeof(Views.RecipeGroupsView));
			}
		}

		public void GotoSettings() =>
			NavigationService.Navigate(typeof(Views.SettingsPage), 0);

		public void GotoPrivacy() =>
			NavigationService.Navigate(typeof(Views.SettingsPage), 1);

		public void GotoAbout() =>
			NavigationService.Navigate(typeof(Views.SettingsPage), 2);

		public async void ImportFileAsync()
		{
			//__allow user to pick file
			currentRecipeBox = await FileIOService.OpenRecipeBoxFromFileAsync();
			if (currentRecipeBox == null) return;

			//__add to current list, selection
			RecentRecipeBox rrb = await FileIOService.CreateRecentRecipeBoxAsync(currentRecipeBox);
			if (rrb == null) return;//__in case something went wrong

			if (RecentRecipeBoxes == null) RecentRecipeBoxes = new ObservableCollection<RecentRecipeBox>();
			RecentRecipeBoxes.Insert(0, rrb);

			SelectedRecentRecipeBox = rrb;
			updateDisplay();
		}

		private async Task populateRecentRecipeBoxListAsync()
		{
			List<RecentRecipeBox> boxesOnFile = await FileIOService.ListKnownRecipeBoxes();
			if (RecentRecipeBoxes != null)
			{
				RecentRecipeBoxes.Clear();
				foreach (RecentRecipeBox rrb in boxesOnFile)
				{
					RecentRecipeBoxes.Add(rrb);
				}
			}
			else RecentRecipeBoxes = new ObservableCollection<RecentRecipeBox>(boxesOnFile);

			//__trigger update of displaye
			updateDisplay();
		}

		private void updateDisplay()
		{
			RaisePropertyChanged("RecentRecipeBoxes");
			RaisePropertyChanged("ShowNoHistory");
			RaisePropertyChanged("ShowHistory");
		}

		public async Task SaveFileAsync()
		{
			if (currentRecipeBox == null) return;

			await FileIOService.SaveRecipeBoxAsync(currentRecipeBox);
		}

		public async void NewRecipeBox()
		{
			currentRecipeBox = await FileIOService.CreateNewRecipeBoxAsync();

			//__record this file or move it to top of recents list
			RecentRecipeBox rrb = await FileIOService.CreateRecentRecipeBoxAsync(currentRecipeBox);
			RecentRecipeBoxes.Insert(0, rrb);

			//__set as selected
			SelectedRecentRecipeBox = rrb;

			//__trigger update of displaye
			updateDisplay();
		}

		#endregion Methods
	}
}

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