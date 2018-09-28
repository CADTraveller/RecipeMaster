using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using RecipeMaster.Models;


namespace RecipeMaster.Models
{
    public class RecipeGroup : ObservableObject
    { 
        private ObservableCollection<Recipe> recipes;

        public ObservableCollection<Recipe> Recipes
        {
            get { return recipes; }
            set
            {
                recipes = value;
                Set(ref recipes, value);
            }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                Set(ref name, value);
            }
        }

        public RecipeGroup(string name = "Group Name")
        {
            recipes = new ObservableCollection<Recipe>();
            Name = name;
        }

        private string recipeCount ;
        public string RecipeCount
        {
            get
            {
                return "(" + recipes?.Count + ") Recipies";
            }
            set { Set(ref recipeCount, value); }
        }

    }
}
