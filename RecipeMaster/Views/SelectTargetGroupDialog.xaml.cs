using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RecipeMaster.Views
{
	public sealed partial class SelectTargetGroupDialog : ContentDialog
	{
		#region Public Fields

		public bool WasCancelled = false;

		#endregion Public Fields

		#region Public Constructors

		public SelectTargetGroupDialog(IEnumerable<string> groupNames)
		{
			groups = new ObservableCollection<string>(groupNames);
			InitializeComponent();
		}

		#endregion Public Constructors

		#region Public Properties

		public ObservableCollection<string> Groups => groups;

		public string SelectedGroupName
		{
			get => selectedGroupName;
			set
			{
				selectedGroupName = value;
				Hide();
			}
		}

		#endregion Public Properties

		#region Private Fields

		private ObservableCollection<string> groups;
		private string selectedGroupName = "";

		#endregion Private Fields

		#region Private Methods

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			WasCancelled = true;
			Hide();
		}

		#endregion Private Methods
	}
}