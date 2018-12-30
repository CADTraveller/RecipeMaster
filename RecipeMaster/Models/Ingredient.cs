﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Template10.Mvvm;
//using SourdoughMaster.Annotations;
using Windows.UI.Xaml;
using Template10.Common;


namespace RecipeMaster.Models
{
	public class Ingredient : ObservableObject, IIngredientContainer
    {

		#region Properties

		private bool entryModeActive;
		private ObservableCollection<Ingredient> ingredients;
		private string name;
		private double percent;
		private IngredientType type;
		private string typeImage;

		private double weight;

		public bool EditModeActive
		{
			get { return !entryModeActive; }

		}

		public bool EntryModeActive
		{
			get { return entryModeActive; }
			set { Set(ref entryModeActive, value); }
		}

		public ObservableCollection<Ingredient> Ingredients
		{
			get { return ingredients; }
			set
			{
				if (ingredients == null) ingredients = new ObservableCollection<Ingredient>();
				Set(ref ingredients, value);
				ShowChildren = ingredients.Count > 0;
			}
		}

		public string Name
		{
			get { return name; }
			set { Set(ref name, value); }
		}

		public double Percent
		{
			get { return Math.Round(percent, 2); }
			set
			{
				if (double.IsNaN(value)) return;

				if (value > 99) value = 99;
				if (value < .001) value = .001;

				Parent.UpdateToNewChildPercent(this, value);
				Parent.UpdateChildrenWeightInEditMode();
				Set(ref percent, value);
				UpdateIngredientWeights();
			}
		}

		public IngredientType Type
		{
            get { return type; }
            set /// ToDo: raise event to re-evaluate parent type when children are edited
            {
                //__handle simple, most common case of no child ingredients
                if (ingredients?.Count == 0 || ingredients == null)
                {
                    Set(ref type, value);
                }
                else
                {
                    SetTypeFromChildren();
                }

                RaisePropertyChanged("TypeImage");
                Parent.UpdateHydration();
                return;
            }
        }


		public string TypeImage
		{
			get
			{
				switch (Type)
				{
					case IngredientType.Dry:
						return "..\\Assets\\dry.png";

					case IngredientType.Fat:
						return "..\\Assets\\fat.png";

					case IngredientType.Complex:
						return "..\\Assets\\complex.png";

					case IngredientType.Salt:
						return "..\\Assets\\salt.png";

					case IngredientType.Spice:
						return "..\\Assets\\spice.png";

					case IngredientType.Sweet:
						return "..\\Assets\\sugar.png";

					case IngredientType.Wet:
						return "..\\Assets\\wet.png";
				}
				return "..\\Assets\\unknown.png";
			}
		}

		public double Weight
		{
			get { return Math.Round(weight, 1); }
			set
			{
				if (double.IsNaN(value)) return;
				if (EntryModeActive)
				{
					//EntryModeWeightChanged();//__ignore percentages and set total as sum 
					//__in Entry Mode child weights may have changed without updating percentages, do that now
					UpdateChildrenWeightInEntryMode(value);
					Parent.UpdateSelfToNewChildWeightInEntryMode();
				}
				else
				{
					//WeightChanged?.Invoke(this, value);//__adjust total by percentages while in Edit mode
					//__in edit mode
					UpdateChildrenWeightInEditMode(value);
					Parent.UpdateToNewChildWeightInEditMode(this, value);
				}
				Parent.UpdateHydration();
				Set(ref weight, value);
				// if (EntryModeActive) return;
			}
		}

		public void SetEntryMode(bool entryModeEnabled)
		{
			EntryModeActive = entryModeEnabled;
			foreach (Ingredient item in Ingredients)
			{
				item.SetEntryMode(entryModeEnabled);
			}
		}

	    public void LinkParentsToChildren(IIngredientContainer myParent)
	    {
		    

		    foreach (Ingredient ingredient in Ingredients)
		    {
			    ingredient.LinkParentsToChildren(this);
		    }
		}

	    public void SetTypeFromChildren()
		{
            if (ingredients == null || ingredients.Count == 0) return;

            //__call this recursively on children to maintain consistency
            foreach (Ingredient i in ingredients)
            {
                i.SetTypeFromChildren();
            }

            //__need to check for any children which aren't of same type as this might change
            if (ingredients.Any(i => i.type != type))
            {
                type = IngredientType.Complex;
            }

            //__also use child type if all are same
            if (Ingredients.All(i => ingredients[0].Type == i.Type))
            {
                type = Ingredients[0].Type;
            }
            RaisePropertyChanged("Type");
            RaisePropertyChanged("TypeImage");
        }
        public void UpdateToNewChildPercent(Ingredient sender, double newPercent)
        {
            double oldOthersPercentTotal = ingredients.Where(i => i != sender).Sum(i => i.percent);
            double newOthersPercentTotal = 100 - newPercent;
            double deltaPercent = newOthersPercentTotal / oldOthersPercentTotal;
            foreach (Ingredient i in ingredients)
            {
                if (i == sender) continue;
                i.AdjustPercent(i.percent * deltaPercent);
            }
            RaisePropertyChanged("Percent");
        }
		#endregion


		bool _hasChildren = default(bool);

		private IIngredientContainer parent;

		bool ratioLocked = default(bool);

		private bool showChildren;

		public event EventHandler ChildWeightChanged;
		public event EventHandler ChildPercentageChanged;

		public Ingredient(string n, IngredientType t, IIngredientContainer parent)
		{
			Parent = parent;
			Name = n;
			Percent = 1;
			Type = t;
			ingredients = new ObservableCollection<Ingredient>();
		}

		public Ingredient()
		{
			ingredients = new ObservableCollection<Ingredient>();

		}

		public bool hasChildren { get { return _hasChildren; } set { Set(ref _hasChildren, value); } }

		public IIngredientContainer Parent { get { return parent; } set { parent = value; } }

		public bool RatioLocked
		{
			get { return ratioLocked; }
			set
			{
				Set(ref ratioLocked, value);
			}
		}

		public bool ShowChildren
		{
			get { return showChildren; }
			set { Set(() => ShowChildren, ref showChildren, value); }
		}

		public bool AddIngredient(Ingredient add = null)
		{
			if (ingredients == null) ingredients = new ObservableCollection<Ingredient>();

			if (add == null)//__create new ingredient with defaults
			{
				add = new Ingredient("Ingredient", IngredientType.Complex, this);
			}

			if (ingredients.Any(i => i.Name == add.Name)) add.Name += "_1";

			add.Parent = this;

			Ingredients.Add(add);
			BalancePercentages();
			ShowChildren = true;
			RaisePropertyChanged("Ingredients");
			hasChildren = true;
			return true;
		}

		public void AdjustIngredientPercentages(object senderObj, double newPercent)
		{
			if (ingredients == null) return;
			ValueAdjusters.AdjustIngredientPercentages(senderObj, newPercent, ingredients);
			UpdateIngredientWeights(senderObj, newPercent);
		}

		public void AdjustPercent(double value)
		{
			if (RatioLocked) return;
			if (value > 100) value = 100;
			if (value < .001) value = .001;
			percent = value;
			RaisePropertyChanged("Percent");
		}

		public void AdjustWeight(double newWeight)
		{
			weight = newWeight;
			RaisePropertyChanged("Weight");
		}

		public void BalancePercentages()
		{
			if (ingredients == null) return;
			double currentTotal = Ingredients.Sum(i => i.Percent);

			double percentLeft = 100;
			for (int i = 0; i < ingredients.Count; i++)
			{
				Ingredient ingredient = ingredients[i];
				if (i == ingredients.Count - 1) ingredient.AdjustPercent(percentLeft);
				else
				{
					double newPercent = (ingredient.Percent / currentTotal) * 100;
					percentLeft -= newPercent;
					ingredient.AdjustPercent(newPercent);
				}
			}
		}

		public void DeleteChild(Ingredient child)
		{
			if (ingredients.Contains(child))
			{
				ingredients.Remove(child);
				BalancePercentages();
			}
		}

		public void FreezeChildren()
		{
			foreach (var ingredient in ingredients)
			{
				ingredient.FreezeChildren();
			}
		}

		public double getDryWeight()
		{
			double dryWeight = type == IngredientType.Dry ? weight : 0;
			if (ingredients == null || ingredients.Count == 0) return dryWeight;
			return ingredients.Sum(i => i.getDryWeight());
		}

		public double GetExactPercent()
		{
			return percent;
		}

		public double GetExactWeight()
		{
            return weight;
        }
		public double getWetWeight()
		{
			double wetWeight = type == IngredientType.Wet ? weight : 0;
			if (ingredients == null || ingredients.Count == 0) return wetWeight;
			return ingredients.Sum(i => i.getWetWeight());
		}

		public void UnFreezeChildren()
		{
            foreach (var ingredient in ingredients)
            {
                ingredient.UnFreezeChildren();
                ingredient.AdjustWeight(weight * ingredient.GetExactPercent() / 100);
            }
        }
		public void UpdateChildrenWeightInEditMode(double newWeight)
		{
			if (ingredients == null || ingredients?.Count == 0) return;

			foreach (Ingredient i in ingredients)
			{
				i.weight = newWeight * i.percent;
			}

			RaisePropertyChanged("Ingredients");
		}

		public void UpdateChildrenWeightInEntryMode(double newWeight)
		{
			if (Ingredients != null && Ingredients?.Count > 0)
			{
				//__in Entry Mode, child weights may have changed, first update current percent
				double dCurrentTotalWeight = ingredients.Sum(i => i.weight);
				foreach (Ingredient i in ingredients)
				{
					i.percent = i.weight / dCurrentTotalWeight;
					i.weight = newWeight * i.percent;
				}
				RaisePropertyChanged("Ingredients");
			}
		}

		public void UpdateHydration()
		{
			Parent.UpdateHydration();
		}

		public void UpdateIngredientWeights(object senderObj, double newPercent)
		{
			if (ingredients == null) return;
			foreach (Ingredient ingredient in ingredients)
			{
				if (ingredient.Equals(senderObj)) ingredient.AdjustWeight(newPercent * weight / 100);
				else ingredient.AdjustWeight(ingredient.GetExactPercent() * weight / 100);
			}
		}

		public void UpdateIngredientWeights()
		{
			if (ingredients == null) return;
			foreach (Ingredient ingredient in ingredients)
			{
				ingredient.AdjustWeight(ingredient.GetExactPercent() * weight / 100);
				ingredient.UpdateIngredientWeights();
			}
		}

		public void UpdateSelfToNewChildWeightInEntryMode()
		{
            weight = ingredients.Sum(i => i.weight);
            RaisePropertyChanged("Weight");
            Parent.UpdateSelfToNewChildWeightInEntryMode();
        }

        public void UpdateToNewChildWeightInEditMode(Ingredient sender, double newWeight)
        {
            double newChildTotalWeight = newWeight * (1 / sender.percent);
            foreach (Ingredient i in ingredients)
            {
                i.weight = i.percent * newChildTotalWeight;
            }
            RaisePropertyChanged("Parent");
        }
    }
}