using Newtonsoft.Json;
using RecipeMaster.Models;
using RecipeMaster.Views;
using System;
using System.Collections.Generic;
using System.IO;
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
					//AddToRecentsLists(file);

					rb.LastPath = file.Path;
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

		public static async Task<List<RecentRecipeBox>> ListKnownRecipeBoxes()
		{
			StorageFolder storageFolder = ApplicationData.Current.RoamingFolder;
			Windows.Storage.Search.StorageFileQueryResult query = storageFolder.CreateFileQuery();
			var files = await query.GetFilesAsync();
			List<RecentRecipeBox> recentRecipeBoxes = new List<RecentRecipeBox>();
			foreach (StorageFile file in files)
			{
				RecentRecipeBox rrb = new RecentRecipeBox(file.DisplayName);
				Windows.Storage.FileProperties.BasicProperties fileProperties = await file.GetBasicPropertiesAsync();
				rrb.LastOpened = fileProperties.DateModified.Date;
				recentRecipeBoxes.Add(rrb);
			}
			return recentRecipeBoxes;
		}


		private static void AddToRecentsLists(StorageFile file)
		{
			//var mru = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList;
			//mru.Add(file, file.Path);

			//var accessList = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList;
			//accessList.Add(file, file.Path);

			ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

		}

		public static async Task StoreRecipeBoxAsync(RecipeBox rb)
		{
			StorageFolder roaming = ApplicationData.Current.RoamingFolder;
			StorageFile file = await roaming.CreateFileAsync(rb.Name);

			string rbJson = JsonConvert.SerializeObject(rb);
			await FileIO.WriteTextAsync(file, rbJson);
		}

		public static async Task<RecipeBox> CreateNewRecipeBoxAsync(string newName = "RecipeBox")
		{


			//var savePicker = new FileSavePicker();
			//savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			//// Dropdown of file types the user can save the file as
			//savePicker.FileTypeChoices.Add("Recipe Box", new List<string>() { ".rcpbx" });
			//// Default file name if the user does not type one in or select a file to replace
			//savePicker.SuggestedFileName = "New Document";
			//StorageFile file = await savePicker.PickSaveFileAsync();

			try
			{


				// Prevent updates to the remote version of the file until
				// we finish making changes and call CompleteUpdatesAsync.
				//CachedFileManager.DeferUpdates(file);

				//// write blank RecipeBox to file
				//RecipeBox rb = new RecipeBox(file.Name);
				//rb.LastPath = file.Path;
				//string rbJson = JsonConvert.SerializeObject(rb);
				//await FileIO.WriteTextAsync(file, rbJson);
				//// Let Windows know that we're finished changing the file so
				//// the other app can update the remote version of the file.
				//// Completing updates may require Windows to ask for user input.
				//Windows.Storage.Provider.FileUpdateStatus status =
				//	await CachedFileManager.CompleteUpdatesAsync(file);
				//if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
				//{
				//	AddToRecentsLists(file);
				//	return rb;
				//}
				//else
				//{
				//	return null;
				//}
				var dialog = new NewNamedItemDialog("Enter Recipe Box Name");
				await dialog.ShowAsync();
				if (dialog.WasCancelled) return null;

				newName = dialog.TextEntry;
				StorageFolder storageFolder = ApplicationData.Current.RoamingFolder;

				StorageFile newFile = await storageFolder.CreateFileAsync(newName, CreationCollisionOption.ReplaceExisting);
				RecipeBox rb = new RecipeBox(newName);
				string recipeBoxJson = JsonConvert.SerializeObject(rb);
				await FileIO.WriteTextAsync(newFile, recipeBoxJson);
				return rb;
			}
			catch
			{
				//__could insert code here for case of picker cancelled
				return null;
			}
		}

		public static async Task<RecentRecipeBox> CreateRecentRecipeBoxAsync(RecipeBox recipeBox)
		{

			try
			{

				//_file was valid, store a copy in my storage
				string recipeBoxName = recipeBox.Name;

				// Create file; replace if exists.
				//await SaveRecipeBoxAsync(rb);
				return new RecentRecipeBox()
				{
					Name = recipeBoxName,
					LastOpened = DateTime.Now,
					Path = recipeBox.LastPath,
					Description = recipeBox.Description
				};

			}
			catch (Exception e)
			{
				return null;
				//__show the user error notice
			}
		}

		public static async Task SaveRecipeBoxAsync(RecipeBox recipeBox)
		{
			try
			{
				StorageFolder storageFolder = ApplicationData.Current.RoamingFolder;
				
				string recipeBoxName = recipeBox.Name;
				string recipeBoxJson = JsonConvert.SerializeObject(recipeBox);
				StorageFile file = await storageFolder.CreateFileAsync(recipeBoxName, CreationCollisionOption.ReplaceExisting);
				await FileIO.WriteTextAsync(file, recipeBoxJson);
				return;
			}
			catch (Exception e)
			{

			}
		}

		public static async Task ExportRecipeBoxAsync(RecipeBox rb = null, RecentRecipeBox rrb = null)
		{
			if (rb == null && rrb == null) return;

			if (rb == null && rrb != null) rb = await OpenRecipeBoxAsync(rrb);

			string fileContents = JsonConvert.SerializeObject(rb);

			var savePicker = new Windows.Storage.Pickers.FileSavePicker();
			savePicker.SuggestedStartLocation =
				Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
			// Dropdown of file types the user can save the file as
			savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".rcpbx" });
			// Default file name if the user does not type one in or select a file to replace
			savePicker.SuggestedFileName = "NewRecipeBox";

			StorageFile newFile = await savePicker.PickSaveFileAsync();
			await FileIO.WriteTextAsync(newFile, fileContents);
		}

		public static async Task<RecipeBox> OpenRecipeBoxAsync(RecentRecipeBox rrb)
		{
			StorageFolder appFolder = ApplicationData.Current.RoamingFolder;
			StorageFile file = await appFolder.GetFileAsync(rrb.Name);
			string json = await FileIO.ReadTextAsync(file);
			RecipeBox rb = JsonConvert.DeserializeObject<RecipeBox>(json);
			return rb;
		}
	}
}
