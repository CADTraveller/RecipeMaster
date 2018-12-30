using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMaster.Models
{
	public class IngredientChangedEventArgs : EventArgs
	{
		public double NewPercentage { get; set; }
		public double  NewWeight { get; set; }

		public bool EntryModeActive { get; set; }
	}
}
