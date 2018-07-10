using Newtonsoft.Json;
using RecipeMaster.Models;
using RecipeMaster.Mvvm;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace RecipeMaster.ViewModels
{
	public class DetailPageViewModel : ViewModelBase
	{
		#region Public Constructors

		public DetailPageViewModel()
		{
			if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				Value = "Designtime value";
			}
		}

		#endregion Public Constructors

		#region Public Properties

		public RecipeBox RecipeBox
		{
			get => recipeBox;
			set => Set(ref recipeBox, value);
		}

		public RecipeGroup SelectedRecipeGroup
		{
			get => selectedRecipeGroup;
			set => Set(ref selectedRecipeGroup, value);
		}

		public string Value { get { return _Value; } set { Set(ref _Value, value); } }

		#endregion Public Properties

		#region Public Methods

		public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
		{
			if (suspending)
			{
				suspensionState[nameof(Value)] = Value;
			}
			await Task.CompletedTask;
		}

		public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
		{
			//Value = (suspensionState.ContainsKey(nameof(Value))) ? suspensionState[nameof(Value)]?.ToString() : parameter?.ToString();
			//await Task.CompletedTask;
			RecentRecipeBox rrb = parameter as RecentRecipeBox;
			OpenRecipeBox(rrb);
		}

		public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
		{
			args.Cancel = false;
			await Task.CompletedTask;
		}

		#endregion Public Methods

		#region Private Fields

		private string _Value = "Default";
		private RecipeBox recipeBox;
		private RecipeGroup selectedRecipeGroup;

		#endregion Private Fields

		#region Private Methods

		private void OpenRecipeBox(RecentRecipeBox rrb)
		{
			string path = rrb.Path;
			string json = File.ReadAllText(path);
			RecipeBox = JsonConvert.DeserializeObject<RecipeBox>(json);
		}

		#endregion Private Methods
	}
}