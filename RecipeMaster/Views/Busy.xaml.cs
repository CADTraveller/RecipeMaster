using Template10.Common;
using Template10.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RecipeMaster.Views
{
	public sealed partial class Busy : UserControl
	{
		#region Public Fields

		public static readonly DependencyProperty BusyTextProperty =
			DependencyProperty.Register(nameof(BusyText), typeof(string), typeof(Busy), new PropertyMetadata("Please wait..."));

		public static readonly DependencyProperty IsBusyProperty =
			DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(Busy), new PropertyMetadata(false));

		#endregion Public Fields

		#region Public Constructors

		public Busy()
		{
			InitializeComponent();
		}

		#endregion Public Constructors

		#region Public Properties

		public string BusyText
		{
			get { return (string)GetValue(BusyTextProperty); }
			set { SetValue(BusyTextProperty, value); }
		}

		public bool IsBusy
		{
			get { return (bool)GetValue(IsBusyProperty); }
			set { SetValue(IsBusyProperty, value); }
		}

		#endregion Public Properties

		#region Public Methods

		// hide and show busy dialog
		public static void SetBusy(bool busy, string text = null)
		{
			WindowWrapper.Current().Dispatcher.Dispatch(() =>
			{
				ModalDialog modal = Window.Current.Content as ModalDialog;
				Busy view = modal.ModalContent as Busy;
				if (view == null)
				{
					modal.ModalContent = view = new Busy();
				}

				modal.IsModal = view.IsBusy = busy;
				view.BusyText = text;
			});
		}

		#endregion Public Methods
	}
}