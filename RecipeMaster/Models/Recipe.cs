using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace RecipeMaster.Models
{
	public class Recipe : ObservableObject, IIngredientContainer
	{
		private RecipeBox parentRecipeBox; //todo Use this to enable Save from detail views
		private string name;

		public string Name
		{
			get { return name; }
			set
			{
				bool nameOK = true;///todo check for duplicates
				if (nameOK)
				{
					Set(ref name, value);
				}
			}
		}

		// todo: Add ID Guid

		private string description;

		public string Description
		{
			get { return description; }
			set { Set(ref description, value); }
		}

		private double totalWeight;

		public double TotalWeight
		{
			get { return Math.Round(totalWeight); }
			set
			{
				Set(ref totalWeight, value);
				UpdateToNewRecipeWeight();
			}
		}

		private string image = "Assets/Croissant.jpg";

		public string Image
		{
			get { return image; }
			set { Set(ref image, value); }
		}

		private double hydration;

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

		public bool EntryModeActive;

		private ObservableCollection<Step> steps = new ObservableCollection<Step>();

		public ObservableCollection<Step> Steps
		{
			get { return steps; }
			set { Set(ref steps, value); }
		}

		private ObservableCollection<Ingredient> ingredients;

		public ObservableCollection<Ingredient> Ingredients
		{
			get { return ingredients; }
			set { Set(ref ingredients, value); }
		}

		public Recipe(string name = "New Recipe")
		{
			this.name = name;
			description = "Please enter description";
			totalWeight = 1000;
			//__build default Lists
			steps = new ObservableCollection<Step>();
			ingredients = new ObservableCollection<Ingredient>();
		}

		public void CalculateHydration()
		{
			double dryWeight = ingredients.Sum(i => i.getDryWeight());
			double wetWeight = ingredients.Sum(i => i.getWetWeight());
			// if (Math.Abs(wetWeight) < 1) Hydration = 0;
			if (dryWeight > 0) Hydration = Math.Round(wetWeight / dryWeight * 100, 1);
			else if (wetWeight > 0) Hydration = 100;
			else Hydration = 0;

			//__Calculate Hydration is also called when type is changed, so update those as well
			foreach (Ingredient i in Ingredients)
			{
				i.SetTypeFromChildren();
			}
		}

		public void UpdateHydration()//_this is a wrapper while the new IIngredientContainer approach is implimented
		{
			CalculateHydration();
		}

		public void AddIngredient(Ingredient i)
		{
			i.Percent = 1;
			Ingredients.Add(i);
			BalancePercentages();
			CalculateHydration();
			i.Parent = this;
		}

		public void BalancePercentages()
		{
			double currentTotal = Ingredients.Sum(i => i.Percent);
			if (TotalWeight > currentTotal) currentTotal = TotalWeight;
			double percentLeft = 100;
			for (int i = 0; i < Ingredients.Count; i++)
			{
				Ingredient ingredient = Ingredients[i];
				if (i == Ingredients.Count - 1) ingredient.AdjustPercent(percentLeft);
				else
				{
					double newPercent = (ingredient.Percent / currentTotal) * 100;
					percentLeft -= newPercent;
					ingredient.AdjustPercent(newPercent);
				}
			}
			CalculateHydration();
		}

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

		public void UpdateToNewRecipeWeight()
		{
			foreach (Ingredient item in Ingredients)
			{
				item.AdjustWeight(item.GetExactPercent() * totalWeight / 100);
				item.UpdateIngredientWeights();
			}
		}

		public void UpdateSelfToNewChildWeightInEntryMode()
		{
			totalWeight = ingredients.Sum(i => i.Weight);
			RaisePropertyChanged("TotalWeight");
		}

		public void UpdateToNewChildWeightInEditMode(Ingredient sender, double newWeight)
		{
			double newTotalWeight = totalWeight + (newWeight - sender.Weight);
			foreach (Ingredient i in ingredients)
			{
				if (i == sender) continue;
				i.AdjustWeight(i.Percent * newTotalWeight);
			}
			RaisePropertyChanged("Ingredients");
		}

		public void UpdateChildrenWeightInEntryMode(double newWeight)
		{
			//__do not support user edits of Total Weight in Entry Mode
			return;
		}

		public void UpdateChildrenWeightInEditMode(double newWeight)
		{
			foreach (Ingredient i in ingredients)
			{
				i.AdjustWeight(i.Percent * newWeight);
			}
			RaisePropertyChanged("Ingredients");
		}

		public void UpdateToNewChildPercent(Ingredient sender, double newPercent)
		{
			ValueAdjusters.AdjustIngredientPercentages(sender, newPercent, Ingredients);
			RaisePropertyChanged("Ingredients");
		}

		public void DeleteChild(Ingredient sender)
		{
			ingredients.Remove(sender);
			double currentPercentTotal = ingredients.Sum(i => i.Percent);
			foreach (Ingredient i in Ingredients)
			{
				i.Percent = i.Percent * 100 / currentPercentTotal;
			}
		}

		public void SetEntryMode(bool entryModeActive)
		{
			EntryModeActive = entryModeActive;
			foreach (Ingredient i in ingredients)
			{
				i.SetEntryMode(entryModeActive);
			}
		}
	}
}