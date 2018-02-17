using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;

namespace RecipeMaster.Models
{
	[JsonObject (MemberSerialization.OptIn)]
    public class RecentRecipeBox : ObservableObject
    {

        public RecentRecipeBox(string name = "Name")
        {
            this.name = name;
			LastOpened = new DateTime();
        }
        private string recipeBoxImagePath = "/Assets/RecipeBoxReal.jpg";
		[JsonProperty]
        public string RecipeBoxImagePath { get => recipeBoxImagePath; }

        private string name;
		[JsonProperty]
		public string Name
        {
            get { return name; }
            set { Set(ref name, value); }
        }

        private string description;
		[JsonProperty]
        public string Description
		{
            get { return description; }
            set { Set(ref description, value); }
        }

        private string path;
		[JsonProperty]
        public string Path
		{
            get { return path; }
            set { Set(ref path, value); }
        }

		private DateTime lastOpened;
		[JsonProperty]
		//[JsonConverter(typeof())]
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
