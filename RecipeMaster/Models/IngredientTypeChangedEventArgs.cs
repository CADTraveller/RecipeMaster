using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMaster.Models
{
	public class IngredientTypeChangedEventArgs : EventArgs
	{
		private IngredientType _newIngredientType;

		public IngredientType NewIngredientType
		{
			get => _newIngredientType;
			set => _newIngredientType = value;

		}

		private IngredientType _oldIngredientType;

		public IngredientType OldIngredientType
		{
			get => _oldIngredientType;
			set => _oldIngredientType = value;

		}
	}
}
