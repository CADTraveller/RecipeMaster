using Newtonsoft.Json;
using RecipeMaster.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace RecipeMaster.Services
{
    public static class FileIOService
    {
        public static async Task<RecipeBox> OpenRecipeBoxFromFileAsync(string path = null)
        {
            StorageFile file;

            if (path == null)
            {

                FileOpenPicker picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".rcpbx");

                file = await picker.PickSingleFileAsync();

            }
            else
            {
                file = await StorageFile.GetFileFromPathAsync(path);
            }

            try
            {
                string contents = await FileIO.ReadTextAsync(file);
                RecipeBox rb = JsonConvert.DeserializeObject<RecipeBox>(contents);
                if (rb != null)
                {
                    rb.LastPath = file.Path;
					AddToRecentsLists(file);

                    return rb;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
                //__show the user error notice
            }

        }


        private static void AddToRecentsLists(StorageFile file)
        {
			//var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
			//mru.Add(file, file.Path);

			//var accessList = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList;
			//accessList.Add(file, file.Path);

			ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

		}

		public static async Task<RecipeBox> CreateNewRecipeBoxAsync()
        {
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Recipe Box", new List<string>() { ".rcpbx" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";
            StorageFile file = await savePicker.PickSaveFileAsync();

            if (file != null)
            {


                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);

                // write blank RecipeBox to file
                RecipeBox rb = new RecipeBox(file.Name);
                rb.LastPath = file.Path;
                string rbJson = JsonConvert.SerializeObject(rb);
                await FileIO.WriteTextAsync(file, rbJson);
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    AddToRecentsLists(file);
                    return rb;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                //__could insert code here for case of picker cancelled
                return null;
            }
        }

        public static async Task SaveRecipeBox(RecipeBox recipeBox)
        {
            string path = recipeBox.LastPath;

            try
            {
                string recipeBoxJson = JsonConvert.SerializeObject(recipeBox);
                StorageFile file = await StorageFile.GetFileFromPathAsync(path);
                await FileIO.WriteTextAsync(file, recipeBoxJson);
                return;
            }
            catch (Exception e)
            {

            }
        }
    }
}
