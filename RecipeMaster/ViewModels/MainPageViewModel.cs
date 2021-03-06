﻿//using RecipeMaster.Mvvm;
using GalaSoft.MvvmLight.Messaging;
using RecipeMaster.Helpers;
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
				//FileIOService.ClearHistoryAsync();
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
				 await FileIOService.OpenRecipeBoxFromFileAsync(SelectedRecentRecipeBox);
			}
			
			BootStrapper.Current.SessionState[App.ActiveRecipeBoxKey] = recipeBoxName;

			NavigationService.Navigate(typeof(Views.RecipeGroupsView));
			//Messenger.Default.Send(new RecipeBoxSelectedMessage(recipeBoxName));
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
			RecentRecipeBox rrb= await FileIOService.OpenRecipeBoxFromFileAsync(null, true);
			if(rrb is null) return;//user cancelled or encountered some error
			if (RecentRecipeBoxes == null) RecentRecipeBoxes = new ObservableCollection<RecentRecipeBox>();
			RecentRecipeBoxes.Insert(0, rrb);
			RaisePropertyChanged(nameof(RecentRecipeBoxes));

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
			//RaisePropertyChanged("RecentRecipeBoxes");
			//RaisePropertyChanged("ShowNoHistory");
			//RaisePropertyChanged("ShowHistory");
			RaisePropertyChanged();
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
			RaisePropertyChanged(nameof(RecentRecipeBoxes));

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