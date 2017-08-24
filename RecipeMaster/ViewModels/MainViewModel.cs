
//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Command;
//using GalaSoft.MvvmLight.Messaging;
using RecipeMaster.Helpers;
using RecipeMaster.Models;
using RecipeMaster.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using System.Linq;
using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace RecipeMaster.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            recentRecipeBoxes = new ObservableCollection<RecentRecipeBox>();
            if (IsInDesignModeStatic)
            {
                RecentRecipeBoxes = SampleDataService.CreateRecentRecipeBoxes();
            }
            else
            {
                var mru = StorageApplicationPermissions.MostRecentlyUsedList;
                
                foreach (AccessListEntry item in mru.Entries)
                {
                    RecentRecipeBox rrb = new RecentRecipeBox();
                    rrb.Path = item.Metadata;
                    if (string.IsNullOrEmpty(rrb.Path)) continue;
                    RecentRecipeBoxes.Add(rrb);
                }
            }
        }

        #region Properties
        private RecipeBox selectedRecipeBox;
        public RecipeBox SelectedRecipeBox
        {
            get { return selectedRecipeBox; }
            set
            {
                Set(ref selectedRecipeBox, value);

            }
        }

        private string status = "No Recipe Box Currently Open";
        public string Status
        {
            get { return status; }
            set { Set(ref status, value); }
        }


        private ObservableCollection<RecentRecipeBox> recentRecipeBoxes;
        public ObservableCollection<RecentRecipeBox> RecentRecipeBoxes
        {
            get { return recentRecipeBoxes; }
            set { Set(ref recentRecipeBoxes, value); }
        }

        private RecentRecipeBox selectedRecentRecipeBox;
        public RecentRecipeBox SelectedRecentRecipeBox
        {
            get { return selectedRecentRecipeBox; }
            set
            {
                Set(ref selectedRecentRecipeBox, value);
                activateSelectedRecipeBoxAsync(); 
            }
        }

        
        #endregion

        #region Commands
        private RelayCommand openRecipeBoxCommand;
        public RelayCommand OpenRecipeBoxCommand =>
            openRecipeBoxCommand ??
            (openRecipeBoxCommand = new RelayCommand(async () => await (OpenRecipeBoxFromFileAsync())));        
        public async Task OpenRecipeBoxFromFileAsync(string path = null)
        {
            RecipeBox rb = await FileIOService.OpenRecipeBoxFromFileAsync(path).ConfigureAwait(false);
            if (rb == null) return;
            SelectedRecipeBox = rb;

            //__add this to the list of recents if it's not already present
            string pathUsed = rb.LastPath;
            if(recentRecipeBoxes.All(r => r.Path != pathUsed))
            {
                RecentRecipeBox rrb = new RecentRecipeBox() { Path = pathUsed, Name = rb.Name };
                RecentRecipeBoxes.Add(rrb);
            }
        }

        //private RelayCommand recipeBoxSelectedCommand;
        //public RelayCommand RecipeBoxSelectedCommand =>
        //    recipeBoxSelectedCommand ??
        //    (recipeBoxSelectedCommand = new RelayCommand(async () => await OpenRecipeBoxFromFileAsync()));

        

        private RelayCommand createRecipeBoxCommand;
        public RelayCommand CreateRecipeBoxCommand =>
            createRecipeBoxCommand ??
            (createRecipeBoxCommand = new RelayCommand(async () => await CreateNewRecipeBoxAsync()));

        public async Task CreateNewRecipeBoxAsync()
        {
            SelectedRecipeBox = await FileIOService.CreateNewRecipeBoxAsync();
            RecentRecipeBox rrb = new RecentRecipeBox();
            rrb.Path = SelectedRecipeBox.LastPath;
            rrb.Name = SelectedRecipeBox.Name;

            RecentRecipeBoxes.Add(rrb);
            SelectedRecentRecipeBox = rrb;
   
        }

        private RelayCommand saveRecipeBoxCommand;
        public RelayCommand SaveRecipeBoxCommand =>
            saveRecipeBoxCommand ??
            (saveRecipeBoxCommand = new RelayCommand(async () => await SaveCurrentRecipeBoxAsync()));
        public async Task SaveCurrentRecipeBoxAsync()
        {
            await FileIOService.SaveRecipeBox(SelectedRecipeBox);
        }

        #endregion

        #region Methods
        public async Task activateSelectedRecipeBoxAsync()
        {
            if (selectedRecentRecipeBox == null) return;
            await OpenRecipeBoxFromFileAsync(selectedRecentRecipeBox.Path);
            var message = new RecipeBoxSelectedMessage() { SelectedRecipeBox = SelectedRecipeBox };
            Messenger.Default.Send(message);
            Status = "Using: " + SelectedRecentRecipeBox.Name;
        }
#endregion
    }
}
