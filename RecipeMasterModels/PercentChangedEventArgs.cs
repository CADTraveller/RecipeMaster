using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMasterModels
{
	public class PercentChangedEventArgs
	{
		private double _oldPercent;

		public double OldPercent
		{
			get => _oldPercent;
			set => _oldPercent = value;
		}

		private double _newPercent;

		public double NewPercent
		{
			get => _newPercent;
			set => _newPercent = value;
		}

		public PercentChangedEventArgs(double oldPercent, double newPercent)
		{
			OldPercent = oldPercent;
			NewPercent = newPercent;
		}
	}
}
