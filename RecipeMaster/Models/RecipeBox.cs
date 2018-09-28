using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace RecipeMaster.Models
{
    public class RecipeBox : ObservableObject
    {
        private ObservableCollection<RecipeGroup> recipeGroups;
        public ObservableCollection<RecipeGroup> RecipeGroups
        {
            get { return recipeGroups; }
            set
            {
                recipeGroups = value;
                RaisePropertyChanged();
            }
        }

        private string lastPath;

        public string LastPath
        {
            get { return lastPath; }
            set { Set(ref lastPath, value); }
        }

	    public string AccessToken { get; set; }

		// todo: Add ID Guid

        private string name;
        public string Name
        {
            get { return name; }
            set { Set(ref name, value); }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { Set(ref description, value); }
        }

        public RecipeBox(string name = "RecipeBox")
        {
            recipeGroups = new ObservableCollection<RecipeGroup>();
            recipeGroups.Add(new RecipeGroup("RecipeGroup"));
            Name = name;
        }

    }
}
