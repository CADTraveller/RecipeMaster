using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using RecipeMaster.Models;
using RecipeMaster.Services.SettingsServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Controls;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RecipeMaster
{
	/// Documentation on APIs used in this page:
	/// https://github.com/Windows-XAML/Template10/wiki

	[Bindable]
	public sealed partial class App : BootStrapper
	{
		#region Public Constructors

		public App()
		{
			InitializeComponent();
			SplashFactory = (e) => new Views.Splash(e);
			AppCenter.Start("8c9025c6-f84a-46cf-907b-1cdca720e459", typeof(Analytics));

			#region app settings

			// some settings must be set in app.constructor
			SettingsService settings = SettingsService.Instance;
			RequestedTheme = settings.AppTheme;
			CacheMaxDuration = settings.CacheMaxDuration;
			ShowShellBackButton = settings.UseShellBackButton;

			//BootStrapper.Current.

			#endregion app settings
		}

		#endregion Public Constructors

		#region Global string constants

		public const string _activeRecipeBoxKey = "ActiveRecipeBox";
		public const string _selectedRecipeGroupKey = "SelectedRecipeGroup";
		public static string ActiveRecipeBoxKey => _activeRecipeBoxKey;
		public static string SelectedRecipeGroupKey => _selectedRecipeGroupKey;
		public static string SelectedRecipeKey => _selectedRecipeKey;
		private const string _selectedRecipeKey = "SelectedRecipe";

		#endregion Global string constants

		#region Public Properties

		public Dictionary<string, RecipeBox> OpenRecipeBoxes { get; set; }

		#endregion Public Properties

		#region Public Methods

		public override UIElement CreateRootElement(IActivatedEventArgs e)
		{
			Template10.Services.NavigationService.INavigationService service = NavigationServiceFactory(BackButton.Attach, ExistingContent.Exclude);
			return new ModalDialog
			{
				DisableBackButtonWhenModal = true,
				Content = new Views.Shell(service),
				ModalContent = new Views.Busy(),
			};
		}

		public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
		{
			// TODO: add your long-running task here
			await NavigationService.NavigateAsync(typeof(Views.MainPage));
		}

		#endregion Public Methods
	}
}