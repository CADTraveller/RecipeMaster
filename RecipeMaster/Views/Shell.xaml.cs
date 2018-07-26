using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RecipeMaster.Views
{
	public sealed partial class Shell : Page
	{
		#region Public Constructors

		public Shell()
		{
			Instance = this;
			InitializeComponent();
			_settings = Services.SettingsServices.SettingsService.Instance;
		}

		public Shell(INavigationService navigationService) : this()
		{
			SetNavigationService(navigationService);
		}

		#endregion Public Constructors

		#region Public Properties

		public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;
		public static Shell Instance { get; set; }

		#endregion Public Properties

		#region Public Methods

		public void SetNavigationService(INavigationService navigationService)
		{
			MyHamburgerMenu.NavigationService = navigationService;
			HamburgerMenu.RefreshStyles(_settings.AppTheme, true);
			HamburgerMenu.IsFullScreen = _settings.IsFullScreen;
			HamburgerMenu.HamburgerButtonVisibility = _settings.ShowHamburgerButton ? Visibility.Visible : Visibility.Collapsed;
		}

		#endregion Public Methods

		#region Private Fields

		private Services.SettingsServices.SettingsService _settings;

		#endregion Private Fields
	}
}