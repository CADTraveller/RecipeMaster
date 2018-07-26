using System;

namespace RecipeMaster.Models
{
	// TODO UWPTemplates: This is used by the Sample Grid Data. Remove this once your grid page is displaying real data
	public class Order
	{
		#region Public Properties

		public string Company { get; set; }
		public DateTime OrderDate { get; set; }
		public long OrderId { get; set; }
		public double OrderTotal { get; set; }
		public string ShipTo { get; set; }
		public string Status { get; set; }

		#endregion Public Properties
	}
}