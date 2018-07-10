using GalaSoft.MvvmLight;

namespace RecipeMaster.Models
{
	public class Step : ObservableObject
	{
		#region Public Constructors

		public Step()
		{
			Description = "Please edit this description";
		}

		public Step(string about)
		{
			Description = about;
		}

		#endregion Public Constructors

		#region Public Properties

		public string Description
		{
			get { return description; }
			set { Set(() => Description, ref description, value); }
		}

		public int Duration
		{
			get { return duration; }
			set { Set(() => Duration, ref duration, value); }
		}

		public int Order
		{
			get { return order; }
			set { Set(() => Order, ref order, value); }
		}

		#endregion Public Properties

		#region Private Fields

		private string description;
		private int duration;
		private int order;

		#endregion Private Fields
	}
}