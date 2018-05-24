using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using Template10.Mvvm;

namespace RecipeMaster.Models
{
	[JsonObject(MemberSerialization.OptIn)]
	public class RecentRecipeBox : BindableBase
	{
		#region Public Constructors

		public RecentRecipeBox(string name = "Name")
		{
			this.name = name;
			LastOpened = new DateTime();
		}

		#endregion Public Constructors

		#region Public Properties

		[JsonProperty]
		public string Description
		{
			get { return description; }
			set { Set(ref description, value); }
		}

		public string FutureAccessToken
		{
			get => _futureAccessToken;
			set => Set(ref _futureAccessToken, value);
		}

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

		[JsonProperty]
		public string Name
		{
			get { return name; }
			set { Set(ref name, value); }
		}

		[JsonProperty]
		public string Path
		{
			get { return path; }
			set { Set(ref path, value); }
		}

		[JsonProperty]
		public string RecipeBoxImagePath { get => recipeBoxImagePath; }

		#endregion Public Properties

		#region Private Fields

		private string _futureAccessToken;
		private string description;
		private DateTime lastOpened;
		private string name;
		private string path;
		private string recipeBoxImagePath = "/Assets/RecipeBoxReal.jpg";

		#endregion Private Fields
	}
}