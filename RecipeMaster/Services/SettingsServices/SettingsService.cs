using System;
using Template10.Common;
using Template10.Utils;
using Windows.UI.Xaml;

namespace RecipeMaster.Services.SettingsServices
{
	public class SettingsService
	{
		#region Public Properties

		public static SettingsService Instance { get; } = new SettingsService();

		public ApplicationTheme AppTheme
		{
			get
			{
				ApplicationTheme theme = ApplicationTheme.Light;
				string value = _helper.Read<string>(nameof(AppTheme), theme.ToString());
				return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
			}
			set
			{
				_helper.Write(nameof(AppTheme), value.ToString());
				(Window.Current.Content as FrameworkElement).RequestedTheme = value.ToElementTheme();
				Views.Shell.HamburgerMenu.RefreshStyles(value, true);
			}
		}

		public TimeSpan CacheMaxDuration
		{
			get { return _helper.Read<TimeSpan>(nameof(CacheMaxDuration), TimeSpan.FromDays(2)); }
			set
			{
				_helper.Write(nameof(CacheMaxDuration), value);
				BootStrapper.Current.CacheMaxDuration = value;
			}
		}

		public bool IsFullScreen
		{
			get { return _helper.Read<bool>(nameof(IsFullScreen), false); }
			set
			{
				_helper.Write(nameof(IsFullScreen), value);
				Views.Shell.HamburgerMenu.IsFullScreen = value;
			}
		}

		public bool ShowHamburgerButton
		{
			get { return _helper.Read<bool>(nameof(ShowHamburgerButton), true); }
			set
			{
				_helper.Write(nameof(ShowHamburgerButton), value);
				Views.Shell.HamburgerMenu.HamburgerButtonVisibility = value ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public bool UseShellBackButton
		{
			get { return _helper.Read<bool>(nameof(UseShellBackButton), true); }
			set
			{
				_helper.Write(nameof(UseShellBackButton), value);
				BootStrapper.Current.NavigationService.GetDispatcherWrapper().Dispatch(() =>
				{
					BootStrapper.Current.ShowShellBackButton = value;
					BootStrapper.Current.UpdateShellBackButton();
				});
			}
		}

		#endregion Public Properties

		#region Private Fields

		private Template10.Services.SettingsService.ISettingsHelper _helper;

		#endregion Private Fields

		#region Private Constructors

		private SettingsService()
		{
			_helper = new Template10.Services.SettingsService.SettingsHelper();
		}

		#endregion Private Constructors
	}
}