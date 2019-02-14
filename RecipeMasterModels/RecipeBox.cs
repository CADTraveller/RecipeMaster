
using System.Collections.ObjectModel;

namespace RecipeMasterModels
{
    public class RecipeBox 
    {

		#region Private Fields

		private string description;
		private string lastPath;
		private string name;
		private ObservableCollection<RecipeGroup> recipeGroups;

		#endregion Private Fields


		#region Public Constructors

		public RecipeBox(string name = "RecipeBox")
		{
			recipeGroups = new ObservableCollection<RecipeGroup>();
			recipeGroups.Add(new RecipeGroup("RecipeGroup"));
			Name = name;
		}

		#endregion Public Constructors


		#region Public Properties

		public string AccessToken { get; set; }

		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		public string LastPath
		{
			get { return lastPath; }
			set { lastPath = value; }
		}

		// todo: Add ID Guid
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public ObservableCollection<RecipeGroup> RecipeGroups
		{
            get { return recipeGroups; }
            set{recipeGroups = value;}
		}

		#endregion Public Properties


		#region Public Methods

		public void ConnectParentsToChildren()
	    {
		    foreach (RecipeGroup recipeGroup in RecipeGroups)
		    {
			    foreach (Recipe recipe in recipeGroup.Recipes)
			    {
				    recipe.LinkAllChildEvents();
			    }
		    }
		}

		#endregion Public Methods

	}
}
