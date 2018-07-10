using Windows.System;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RecipeMaster.Views
{
	public sealed partial class NewNamedItemDialog
	{
		#region Public Fields

		public string TextEntry;

		public bool WasCancelled = false;

		#endregion Public Fields

		#region Public Constructors

		public NewNamedItemDialog(string title = "Enter Name")
		{
			Title = title;
			InitializeComponent();
		}

		#endregion Public Constructors

		#region Private Methods

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			WasCancelled = true;
		}

		private void NameBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
		{
			if (e.Key == VirtualKey.Enter)
			{
				Hide();
			}
		}

		#endregion Private Methods
	}
}