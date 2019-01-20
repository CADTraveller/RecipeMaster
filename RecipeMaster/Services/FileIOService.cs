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
using Windows.Storage.Search;
using Template10.Common;

namespace RecipeMaster.Services
{
	public static class FileIOService
	{
		//private static StorageFolder localFolder = ApplicationData.Current.LocalFolder;

		private static ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

		private static SettingsServices.SettingsService _settings = SettingsServices.SettingsService.Instance;

		public static async Task<RecipeBox> CreateNewRecipeBoxAsync(string newName = "RecipeBox")
		{
			try
			{
				var savePicker = new FileSavePicker();
				savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
				// Dropdown of file types the user can save the file as
				savePicker.FileTypeChoices.Add("Recipe Box", new List<string>() { ".rcpbx" });
				// Default file name if the user does not type one in or select a file to replace
				savePicker.SuggestedFileName = "MyRecipeBox";
				StorageFile file = await savePicker.PickSaveFileAsync();

				if (file == null) return null;

				newName = file.DisplayName;

				RecipeBox rb = new RecipeBox(newName);
				rb.LastPath = file.Path;

				string recipeBoxJson = JsonConvert.SerializeObject(rb);
				await FileIO.WriteTextAsync(file, recipeBoxJson);
				await RecordRecentRecipeBoxAsync(rb, file);
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
			List<RecentRecipeBox> recentRecipeBoxes = new List<RecentRecipeBox>();
			var mru = StorageApplicationPermissions.MostRecentlyUsedList;
			foreach (Windows.Storage.AccessCache.AccessListEntry entry in mru.Entries)
			{
				string mruToken = entry.Token;
				string mruMetadata = entry.Metadata;
				//Windows.Storage.IStorageItem item = await mru.GetItemAsync(mruToken);
				
			}
			return recentRecipeBoxes;
		}



		public static async Task<RecentRecipeBox> OpenRecipeBoxFromFileAsync(RecentRecipeBox rrb = null, bool needToRecordAccess = false)
		{
			// TODO: this needs to be revised to utilize MRU and metadata storage
			string token = rrb?.Token;
			StorageFile file = null;
			RecipeBox rb;

			if (rrb == null || string.IsNullOrEmpty(token))
			{
				FileOpenPicker picker = new FileOpenPicker();
				picker.FileTypeFilter.Add(".rcpbx");

				file = await picker.PickSingleFileAsync();
				needToRecordAccess = true;
			}
			else
			{
				file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
			}

			try
			{
				string contents = await FileIO.ReadTextAsync(file);
				rb = JsonConvert.DeserializeObject<RecipeBox>(contents);
				rb.ConnectParentsToChildren();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return null;
				//__show the user error notice
			}

			if (needToRecordAccess)
			{
				rrb = await RecordRecentRecipeBoxAsync(rb, file);
			}

			return rrb;

		}

		public static async Task<RecentRecipeBox> RecordRecentRecipeBoxAsync(RecipeBox rb, StorageFile file)
		{
			RecentRecipeBox rrb = await CreateRecentRecipeBoxAsync(rb);
			rb.LastPath = file.Path;
			string metaData = JsonConvert.SerializeObject(rb);
			if (file != null)
			{
				// Add to FA without metadata
				//string faToken = StorageApplicationPermissions.FutureAccessList.Add(file);
				string mruToken = StorageApplicationPermissions.MostRecentlyUsedList.Add(file, metaData);
				//rrb.Token = faToken;
				//rb.AccessToken = faToken;
				//_settings.AccessTokens.Add(faToken);
			}
			
			//__store a record of this access
			//StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			//string name = rrb.Name;
			//string contents = JsonConvert.SerializeObject(rrb);
			//StorageFile newRecord = await localFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
			//await FileIO.WriteTextAsync(newRecord, contents);

			//__store an instance of the rb if it doesn't already exist
			BootStrapper.Current.SessionState[rb.Name] = rb;

			return rrb;
		}

		public static async Task RemoveRecentRecipeBoxAsync(RecentRecipeBox rrb)
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
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
			string accessToken = rb.AccessToken;
			StorageFile targetFile = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(accessToken);
			
			//if (targetFile != null && !doSaveAs)
			//{
			//	StorageFolder targetFolder = await StorageFolder.GetFolderFromPathAsync(lastSavePath);
			//	targetFile = await targetFolder.CreateFileAsync(rb.Name, CreationCollisionOption.ReplaceExisting);
			//}
			//else
			if(targetFile == null || doSaveAs)
			{
				savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

				// Dropdown of file types the user can save the file as
				savePicker.FileTypeChoices.Add("Recipe Box", new List<string>() { ".rcpbx" });

				// Default file name if the user does not type one in or select a file to replace
				savePicker.SuggestedFileName = rb.Name;
				targetFile = await savePicker.PickSaveFileAsync();
			}

			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
			string rbJson = JsonConvert.SerializeObject(rb, settings);
			await FileIO.WriteTextAsync(targetFile, rbJson);
		}

		public static async Task ClearHistoryAsync()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			Windows.Storage.Search.StorageFileQueryResult query = localFolder.CreateFileQuery();
			var files = await query.GetFilesAsync();
			foreach (StorageFile storageFile in files)
			{
				storageFile.DeleteAsync();
			}
		}
	}
}