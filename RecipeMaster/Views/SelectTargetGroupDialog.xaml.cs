using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Template10.Mvvm;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RecipeMaster.Views
{
	public sealed partial class SelectTargetGroupDialog : ContentDialog
	{
		public SelectTargetGroupDialog(IEnumerable<string> groupNames)
		{
			groups = new ObservableCollection<string>(groupNames);
			this.InitializeComponent();
		}

		public bool WasCancelled = false;

		private ObservableCollection<string> groups;

		public ObservableCollection<string> Groups => groups;

		private string selectedGroupName = "";

		public string SelectedGroupName
		{
			get => selectedGroupName;
			set
			{
				selectedGroupName = value;
				Hide();
			}
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			WasCancelled = true; 
			Hide();
		}
	}
}
