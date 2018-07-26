using System;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml;

namespace RecipeMaster.ViewModels
{
	public class AboutPartViewModel : ViewModelBase
	{
		#region Public Properties

		public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;
		public Uri Logo => Windows.ApplicationModel.Package.Current.Logo;
		public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

		public Uri RateMe => new Uri("http://aka.ms/template10");

		public string Version
		{
			get
			{
				Windows.ApplicationModel.PackageVersion v = Windows.ApplicationModel.Package.Current.Id.Version;
				return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
			}
		}

		#endregion Public Properties
	}

	public class SettingsPageViewModel : ViewModelBase
	{
		#region Public Properties

		public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();
		public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();

		#endregion Public Properties
	}

	public class SettingsPartViewModel : ViewModelBase
	{
		#region Public Constructors

		public SettingsPartViewModel()
		{
			if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				// designtime
			}
			else
			{
				_settings = Services.SettingsServices.SettingsService.Instance;
			}
		}

		#endregion Public Constructors

		#region Public Properties

		public string BusyText
		{
			get { return _BusyText; }
			set
			{
				Set(ref _BusyText, value);
				_ShowBusyCommand.RaiseCanExecuteChanged();
			}
		}

		public bool IsFullScreen
		{
			get { return _settings.IsFullScreen; }
			set
			{
				_settings.IsFullScreen = value;
				base.RaisePropertyChanged();
				if (value)
				{
					ShowHamburgerButton = false;
				}
				else
				{
					ShowHamburgerButton = true;
				}
			}
		}

		public DelegateCommand ShowBusyCommand
			=> _ShowBusyCommand ?? (_ShowBusyCommand = new DelegateCommand(async () =>
			{
				Views.Busy.SetBusy(true, _BusyText);
				await Task.Delay(5000);
				Views.Busy.SetBusy(false);
			}, () => !string.IsNullOrEmpty(BusyText)));

		public bool ShowHamburgerButton
		{
			get { return _settings.ShowHamburgerButton; }
			set { _settings.ShowHamburgerButton = value; base.RaisePropertyChanged(); }
		}

		public bool UseLightThemeButton
		{
			get { return _settings.AppTheme.Equals(ApplicationTheme.Light); }
			set { _settings.AppTheme = value ? ApplicationTheme.Light : ApplicationTheme.Dark; base.RaisePropertyChanged(); }
		}

		public bool UseShellBackButton
		{
			get { return _settings.UseShellBackButton; }
			set { _settings.UseShellBackButton = value; base.RaisePropertyChanged(); }
		}

		#endregion Public Properties

		#region Private Fields

		private string _BusyText = "Please wait...";
		private Services.SettingsServices.SettingsService _settings;
		private DelegateCommand _ShowBusyCommand;

		#endregion Private Fields
	}
}