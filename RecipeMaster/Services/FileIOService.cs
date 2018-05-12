using Newtonsoft.Json;
using RecipeMaster.Models;
using RecipeMaster.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace RecipeMaster.Services
{
	public static class FileIOService
	{
		private static StorageItemAccessList accessList =
			Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList;

		private static StorageFolder localFolder = ApplicationData.Current.LocalFolder;

		private static ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

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

		public static async Task<List<RecentRecipeBox>> ListKnownRecipeBoxes()
		{
			Windows.Storage.Search.StorageFileQueryResult query = localFolder.CreateFileQuery();
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

		public static async Task<RecipeBox> OpenRecipeBoxAsync(RecentRecipeBox rrb)
		{
			RecipeBox rb;

			try
			{
				rb = await OpenRecipeBoxFromFileAsync(rrb.Path, false);
			}
			catch (Exception e)
			{
				//__if this open failed, remove RecentRecipeBox from storage
				await RemoveRecentRecipeBoxAsync(rrb);
				Console.WriteLine(e);
				return null;
			}

			return rb;
		}

		public static async Task<RecipeBox> OpenRecipeBoxFromFileAsync(string path = null, bool needToRecordAccess = true)
		{
			StorageFile file;
			RecipeBox rb;

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
				rb = JsonConvert.DeserializeObject<RecipeBox>(contents);
				if (rb != null)
				{
					//_enable future access
					string faToken = accessList.Add(file);

					//__make a record of this recipe box
					rb.LastPath = file.Path;
					await RecordRecentRecipeBoxAsync(rb);
				}
				else return null;
			}
			catch (Exception e)
			{
				return null;
				//__show the user error notice
			}

			if (needToRecordAccess)
			{
				await RecordRecentRecipeBoxAsync(rb);
			}

			return rb;

		}

		public static async Task RecordRecentRecipeBoxAsync(RecipeBox rb)
		{
			RecentRecipeBox rrb = await CreateRecentRecipeBoxAsync(rb);
			string name = rrb.Name;
			string contents = JsonConvert.SerializeObject(rrb);
			StorageFile newRecord = await localFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteTextAsync(newRecord, contents);
		}

		public static async Task RemoveRecentRecipeBoxAsync(RecentRecipeBox rrb)
		{
			Windows.Storage.Search.StorageFileQueryResult query = localFolder.CreateFileQuery();
			var files = await query.GetFilesAsync();

			StorageFile fileToDelete = files.FirstOrDefault(f => f.Name == rrb.Name);
			if (fileToDelete != null)
			{
				await fileToDelete.DeleteAsync();
			}
		}

		public static async Task SaveRecipeBoxAsync(RecipeBox rb, bool doSaveAs = false)
		{
			var savePicker = new FileSavePicker();
			string lastSavePath = rb.LastPath;
			StorageFile targetFile;

			if (File.Exists(lastSavePath) && !doSaveAs)
			{
				StorageFolder targetFolder = await StorageFolder.GetFolderFromPathAsync(lastSavePath);
				targetFile = await targetFolder.CreateFileAsync(rb.Name, CreationCollisionOption.ReplaceExisting);
			}
			else
			{
				savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

				// Dropdown of file types the user can save the file as
				savePicker.FileTypeChoices.Add("Recipe Box", new List<string>() { ".rcpbx" });

				// Default file name if the user does not type one in or select a file to replace
				savePicker.SuggestedFileName = rb.Name;
				targetFile = await savePicker.PickSaveFileAsync();
			}

			string rbJson = JsonConvert.SerializeObject(rb);
			await FileIO.WriteTextAsync(targetFile, rbJson);
		}

		public static async Task ClearHistory()
		{
			Windows.Storage.Search.StorageFileQueryResult query = localFolder.CreateFileQuery();
			var files = await query.GetFilesAsync();
			foreach (StorageFile storageFile in files)
			{
				storageFile.DeleteAsync();
			}
		}
	}
}