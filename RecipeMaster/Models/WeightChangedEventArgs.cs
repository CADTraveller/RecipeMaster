using System;

namespace RecipeMaster.Models
{
	public class WeightChangedEventArgs : EventArgs
	{
		private double _oldWeight;
		public double OldWeight
		{
			get => _oldWeight;
			set => _oldWeight = value;

		}

		private double _newWeight;

		public double NewWeight
		{
			get => _newWeight;
			set => _newWeight = value;

		}
		

	}
}