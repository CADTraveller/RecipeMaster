using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Template10.Utils;

namespace RecipeMaster.Models
{
	public class Recipe : Ingredient
	{
		public bool EntryModeActive;
		private string description;
		private double hydration;
		private string image = "Assets/Croissant.jpg";
		private ObservableCollection<Ingredient> ingredients;
		private string name;
		private RecipeBox parentRecipeBox; //todo Use this to enable Save from detail views
		private ObservableCollection<Step> steps = new ObservableCollection<Step>();

		private double totalWeight;

		public event EventHandler ChildWeightChanged;
		public event EventHandler ChildPercentageChanged;

		public Recipe(string name = "New Recipe")
		{
			this.name = name;
			description = "Please enter description";
			totalWeight = 1000;
			//__build default Lists
			steps = new ObservableCollection<Step>();
			ingredients = new ObservableCollection<Ingredient>();
		}

		// todo: Add ID Guid
		public string Description
		{
			get { return description; }
			set { Set(ref description, value); }
		}

		public double Hydration
		{
			get { return Math.Round(hydration, 1); }
			set
			{
				if (value < 0) value = 0;
				if (double.IsNaN(value)) value = 0;
				Set(ref hydration, value);
			}
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
		//public double TotalWeight
		//{
		//	get { return Math.Round(totalWeight); }
		//	set
		//	{
		//		Set(ref totalWeight, value);
		//		UpdateIngredientWeights();
		//	}
		//}


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