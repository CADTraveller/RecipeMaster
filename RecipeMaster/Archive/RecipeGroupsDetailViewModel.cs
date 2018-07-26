//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Command;

using RecipeMaster.Models;
using RecipeMaster.Mvvm;
using System.Windows.Input;
using Windows.UI.Xaml;

//using Template10.Services.NavigationService;

namespace RecipeMaster.ViewModels
{
	public class RecipeGroupsDetailViewModel : ViewModelBase
	{
		#region Public Constructors

		public RecipeGroupsDetailViewModel()
		{
			StateChangedCommand = new Template10.Mvvm.DelegateCommand<VisualStateChangedEventArgs>(OnStateChanged);
		}

		#endregion Public Constructors

		#region Public Properties

		public Order Item
		{
			get { return _item; }
			set { Set(ref _item, value); }
		}

		public ICommand StateChangedCommand { get; private set; }

		#endregion Public Properties

		#region Private Fields

		//public NavigationService NavigationService
		//{
		//	get
		//	{
		//		return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationService>();
		//	}
		//}
		private const string NarrowStateName = "NarrowState";

		private const string WideStateName = "WideState";
		private Order _item;

		#endregion Private Fields

		#region Private Methods

		private void OnStateChanged(VisualStateChangedEventArgs args)
		{
			if (args.OldState.Name == NarrowStateName && args.NewState.Name == WideStateName)
			{
				NavigationService.GoBack();
			}
		}

		#endregion Private Methods
	}
}