using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMaster.Models
{
    public class NutritionInfo
    {
        public int Calories { get; set; }
        public int Protein { get; set; }
        public int Sugar { get; set; }
        public int Iron { get; set; }
        public int Calcium { get; set; }
        public int Fiber { get; set; }
        public int Carbohydrates { get; set; }
        public int Magnesium { get; set; }
        public int Sodium { get; set; }
        public int UnsaturatedFat { get; set; }
        public int SaturatedFat { get; set; }

        public static NutritionInfo GetNutritionInfo(ObservableCollection<Ingredient> ingredients)
        {
            NutritionInfo nutritionInfoResult = new NutritionInfo();

            return nutritionInfoResult;
        }

    }


}
