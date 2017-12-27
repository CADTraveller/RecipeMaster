using GalaSoft.MvvmLight;
using System;

namespace RecipeMaster.Models
{
    public class RecentRecipeBox : ObservableObject
    {

        public RecentRecipeBox(string name = "Name")
        {
            this.name = name;
        }
        private string recipeBoxImagePath = "/Assets/RecipeBoxReal.jpg";
        public string RecipeBoxImagePath { get => recipeBoxImagePath; }

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

        private string path;
        public string Path
        {
            get { return path; }
            set { Set(ref path, value); }
        }

		private DateTime lastOpened;
		public DateTime LastOpened
		{
			get { return lastOpened; }
			set
			{
				Set(ref lastOpened, value);
			}
		}

    }
}
