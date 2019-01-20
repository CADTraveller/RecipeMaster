using RecipeMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMasterUnitTests
{
	public static class TestRecipeGenerator
	{

		public static Recipe CreateRecipeHydration50()
		{
			Recipe recipe = new Recipe("Hydration 50");
			Ingredient Flour = new Ingredient("Flour", IngredientType.Dry);

			return recipe;
		}

	}
}
