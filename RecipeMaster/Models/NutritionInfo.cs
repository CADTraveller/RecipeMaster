using System.Collections.ObjectModel;

namespace RecipeMaster.Models
{
	public class NutritionInfo
	{
		#region Public Properties

		public int Calcium { get; set; }
		public int Calories { get; set; }
		public int Carbohydrates { get; set; }
		public int Fiber { get; set; }
		public int Iron { get; set; }
		public int Magnesium { get; set; }
		public int Protein { get; set; }
		public int SaturatedFat { get; set; }
		public int Sodium { get; set; }
		public int Sugar { get; set; }
		public int UnsaturatedFat { get; set; }

		#endregion Public Properties

		// todo: Add ID and coordinate with Nutrient data

		#region Public Methods

		public static NutritionInfo GetNutritionInfo(ObservableCollection<Ingredient> ingredients)
		{
			NutritionInfo nutritionInfoResult = new NutritionInfo();

			return nutritionInfoResult;
		}

		#endregion Public Methods
	}
}