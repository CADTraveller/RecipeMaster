using Windows.UI.Xaml;
using System.Threading.Tasks;
using RecipeMaster.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using RecipeMaster.Models;

namespace RecipeMaster
{
	/// Documentation on APIs used in this page:
	/// https://github.com/Windows-XAML/Template10/wiki

	[Bindable]
	sealed partial class App : BootStrapper
	{
		public App()
		{
			InitializeComponent();
			SplashFactory = (e) => new Views.Splash(e);

			#region app settings

			// some settings must be set in app.constructor
			var settings = SettingsService.Instance;
			RequestedTheme = settings.AppTheme;
			CacheMaxDuration = settings.CacheMaxDuration;
			ShowShellBackButton = settings.UseShellBackButton;
			//BootStrapper.Current.
			#endregion
		}

		#region Global string constants

		public const string _activeRecipeBoxKey = "ActiveRecipeBox";
		public const string _selectedRecipeGroupKey = "SelectedRecipeGroup";
		private const string _selectedRecipeKey = "SelectedRecipe";

		public static string SelectedRecipeKey => _selectedRecipeKey;
		public static string SelectedRecipeGroupKey => _selectedRecipeGroupKey;
		public static string ActiveRecipeBoxKey => _activeRecipeBoxKey;

		#endregion

		public Dictionary<string, RecipeBox> OpenRecipeBoxes { get; set; }


		public override UIElement CreateRootElement(IActivatedEventArgs e)
		{
			var service = NavigationServiceFactory(BackButton.Attach, ExistingContent.Exclude);
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
	}
}
