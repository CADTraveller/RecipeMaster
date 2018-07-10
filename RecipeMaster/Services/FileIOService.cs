using Newtonsoft.Json;
using RecipeMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template10.Common;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Search;

namespace RecipeMaster.Services
{
	public static class FileIOService
	{
		//private static StorageFolder localFolder = ApplicationData.Current.LocalFolder;

		#region Public Methods

		public static async Task ClearHistoryAsync()
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			Windows.Storage.Search.StorageFileQueryResult query = localFolder.CreateFileQuery();
			IReadOnlyList<StorageFile> files = await query.GetFilesAsync();
			foreach (StorageFile storageFile in files)
			{
				storageFile.DeleteAsync();
			}
		}

		public static async Task<RecipeBox> CreateNewRecipeBoxAsync(string newName = "RecipeBox")
		{
			try
			{
				FileSavePicker savePicker = new FileSavePicker();
				savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
				// Dropdown of file types the user can save the file as
				savePicker.FileTypeChoices.Add("Recipe Box", new List<string>() { ".rcpbx" });
				// Default file name if the user does not type one in or select a file to replace
				savePicker.SuggestedFileName = "MyRecipeBox";
				StorageFile file = await savePicker.PickSaveFileAsync();

				if (file == null)
				{
					return null;
				}

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

				//__remove file extension if it is present
				if (recipeBoxName.EndsWith(".rcpbx"))
				{
					recipeBoxName = recipeBoxName.Remove(recipeBoxName.Length - 6);
				}

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
			if (rb == null && rrb == null)
			{
				return;
			}

			string fileContents = JsonConvert.SerializeObject(rb);

			FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
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
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;

			List<string> typeFilters = new List<string>() { ".rcpbx" };
			QueryOptions options = new QueryOptions(CommonFileQuery.OrderByName, typeFilters);

			Windows.Storage.Search.StorageFileQueryResult query = localFolder.CreateFileQuery();
			query.ApplyNewQueryOptions(options);

			IReadOnlyList<StorageFile> files = await query.GetFilesAsync();
			List<RecentRecipeBox> recentRecipeBoxes = new List<RecentRecipeBox>();
			foreach (StorageFile file in files)
			{
				//if(file.FileType != ".rcpbx")
				RecentRecipeBox rrb = new RecentRecipeBox(file.DisplayName);
				Windows.Storage.FileProperties.BasicProperties fileProperties = await file.GetBasicPropertiesAsync();
				rrb.LastOpened = fileProperties.DateModified.Date;
				recentRecipeBoxes.Add(rrb);
			}
			return recentRecipeBoxes;
		}

		public static async Task<RecentRecipeBox> OpenRecipeBoxFromFileAsync(RecentRecipeBox rrb = null, bool needToRecordAccess = false)
		{
			string token = rrb?.Token;
			StorageFile file = null;
			RecipeBox rb;

			if (rrb == null || string.IsNullOrEmpty(token))
			{
				FileOpenPicker picker = new FileOpenPicker();
				picker.FileTypeFilter.Add(".rcpbx");

				file = await picker.PickSingleFileAsync();

				/// Todo: test to see if this succeeded, if not remove rrb from storage
				needToRecordAccess = true;
			}
			else
			{
				file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
				//file = await StorageFile.GetFileFromPathAsync(rrb.Path);
			}

			try
			{
				string contents = await FileIO.ReadTextAsync(file);
				rb = JsonConvert.DeserializeObject<RecipeBox>(contents);
				string recipeBoxName = rb.Name;

				//__remove file extension if it is present
				if (recipeBoxName.EndsWith(".rcpbx"))
				{
					recipeBoxName = recipeBoxName.Remove(recipeBoxName.Length - 6);
					rb.Name = recipeBoxName;
				}
				BootStrapper.Current.SessionState[recipeBoxName] = rb;
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
			rb.LastPath = file?.Path;

			if (file != null)
			{
				// Add to FA without metadata
				string faToken = StorageApplicationPermissions.FutureAccessList.Add(file);
				rrb.Token = faToken;
			}

			//__store a record of this access
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			string name = rrb.Name;
			string contents = JsonConvert.SerializeObject(rrb);
			StorageFile newRecord = await localFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteTextAsync(newRecord, contents);

			//__store an instance of the rb if it doesn't already exist
			BootStrapper.Current.SessionState[rb.Name] = rb;

			return rrb;
		}

		public static async Task RemoveRecentRecipeBoxAsync(RecentRecipeBox rrb)
		{
			StorageFolder localFolder = ApplicationData.Current.LocalFolder;
			Windows.Storage.Search.StorageFileQueryResult query = localFolder.CreateFileQuery();
			IReadOnlyList<StorageFile> files = await query.GetFilesAsync();

			StorageFile fileToDelete = files.FirstOrDefault(f => f.Name == rrb.Name);
			if (fileToDelete != null)
			{
				await fileToDelete.DeleteAsync();
			}
		}

		public static async Task SaveRecipeBoxAsync(RecipeBox rb, bool doSaveAs = false)
		{
			FileSavePicker savePicker = new FileSavePicker();
			string lastSavePath = rb.LastPath;
			string accessToken = rb.AccessToken;
			StorageFile targetFile = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(accessToken);

			//if (targetFile != null && !doSaveAs)
			//{
			//	StorageFolder targetFolder = await StorageFolder.GetFolderFromPathAsync(lastSavePath);
			//	targetFile = await targetFolder.CreateFileAsync(rb.Name, CreationCollisionOption.ReplaceExisting);
			//}
			//else
			if (targetFile == null || doSaveAs)
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

		#endregion Public Methods

		#region Private Fields

		private static ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

		#endregion Private Fields
	}
}