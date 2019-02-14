
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace RecipeMasterModels
{
	public class Recipe : Ingredient
	{

		private string description;
		private double hydration;
		private string image = "Assets/Croissant.jpg";
		private ObservableCollection<Ingredient> ingredients;
		private string name;

		private ObservableCollection<Step> steps = new ObservableCollection<Step>();

		private Guid _identifier;

		public Guid Identifier
		{
			get => _identifier;
			set => _identifier = value;
		}

		public Recipe(string name = "New Recipe")
		{
			this.name = name;
			description = "Please enter description";
			Weight = 1000;
			//__build default Lists
			steps = new ObservableCollection<Step>();
			ingredients = new ObservableCollection<Ingredient>();
		}

		public string Description
		{
			get { return description; }
			set { Set(ref description, value); }
		}


		public string Image
		{
			get { return image; }
			set { Set(ref image, value); }
		}


		public ObservableCollection<Step> Steps
		{
			get { return steps; }
			set { Set(ref steps, value); }
		}

		public string AccessToken { get; set; }
		
		public void AddStep(Step newStep)
		{
			int newOrder = newStep.Order;
			foreach (var step in steps.Where(step => step.Order >= newOrder))
			{
				step.Order++;
			}
			steps.Add(newStep);
			var orderedList = steps.OrderBy(s => s.Order).ToList();
			steps.Clear();
			steps = new ObservableCollection<Step>(orderedList);
		}




	}
}