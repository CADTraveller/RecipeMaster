using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace RecipeMaster.Models
{
	public class RecipeGroup : ObservableObject
	{
		#region Public Constructors

		public RecipeGroup(string name = "Group Name")
		{
			recipes = new ObservableCollection<Recipe>();
			Name = name;
		}

		#endregion Public Constructors

		#region Public Properties

		public string Name
		{
			get { return name; }
			set
			{
				Set(ref name, value);
			}
		}

		public string RecipeCount
		{
			get
			{
				return "(" + recipes?.Count + ") Recipies";
			}
			set { Set(ref recipeCount, value); }
		}

		public ObservableCollection<Recipe> Recipes
		{
			get { return recipes; }
			set
			{
				recipes = value;
				Set(ref recipes, value);
			}
		}

		#endregion Public Properties

		#region Private Fields

		private string name;
		private string recipeCount;
		private ObservableCollection<Recipe> recipes;

		#endregion Private Fields
	}
}