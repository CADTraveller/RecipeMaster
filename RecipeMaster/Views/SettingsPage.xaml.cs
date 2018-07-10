using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RecipeMaster.Views
{
	public sealed partial class SettingsPage : Page
	{
		#region Public Constructors

		public SettingsPage()
		{
			InitializeComponent();
			NavigationCacheMode = NavigationCacheMode.Required;
			_SerializationService = Template10.Services.SerializationService.SerializationService.Json;
		}

		#endregion Public Constructors

		#region Protected Methods

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			int index = int.Parse(_SerializationService.Deserialize(e.Parameter?.ToString()).ToString());
			MyPivot.SelectedIndex = index;
		}

		#endregion Protected Methods

		#region Private Fields

		private Template10.Services.SerializationService.ISerializationService _SerializationService;

		#endregion Private Fields
	}
}