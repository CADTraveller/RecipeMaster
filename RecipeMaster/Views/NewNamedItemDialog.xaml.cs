using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RecipeMaster.Views
{
    public sealed partial class NewNamedItemDialog : ContentDialog
    {
        public NewNamedItemDialog(string title = "Enter Name")
        {
            Title = title;
            InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            WasCancelled = true;
        }

        public string TextEntry;
        public bool WasCancelled = false;

    }
}
