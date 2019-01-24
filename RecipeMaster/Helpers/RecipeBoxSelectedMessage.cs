using RecipeMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMaster.Helpers
{
    class RecipeBoxSelectedMessage
    {
        public RecipeBox SelectedRecipeBox;

		RecipeBoxSelectedMessage(RecipeBox rb)
		{
			SelectedRecipeBox = rb;
		}
    }
}
