
using System.Collections.ObjectModel;

namespace RecipeMasterModels
{
    public class RecipeGroup 
    { 
        private ObservableCollection<Recipe> recipes;

        public ObservableCollection<Recipe> Recipes
        {
            get { return recipes; }
            set
            {
                recipes = value;
            }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
            }
        }

        public RecipeGroup(string name = "Group Name")
        {
            recipes = new ObservableCollection<Recipe>();
            Name = name;
        }


    }
}
