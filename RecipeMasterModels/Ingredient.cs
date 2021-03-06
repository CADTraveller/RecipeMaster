﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RecipeMasterModels
{
	public class Ingredient : ObservableObject
	{


		#region Private Fields

		private bool _entryModeActive;
		bool _hasChildren = default(bool);
		private double _hydration;
		private ObservableCollection<Ingredient> _ingredients;
		private string _name;
		private double _percent;
		private IngredientType _type;
		private double _weight;
		bool ratioLocked = default(bool);

		private bool showChildren;

		#endregion Private Fields


		#region Private Methods

		private void setChildIngredientsEntryMode(bool entryModeActive)
		{
			foreach (Ingredient i in Ingredients)
			{
				i.EntryModeActive = entryModeActive;
			}
		}


		#endregion Private Methods


		#region Public Constructors

		public Ingredient(string n, IngredientType t)
		{
			Name = n;
			Percent = 1;
			Type = t;
			_ingredients = new ObservableCollection<Ingredient>();
		}

		public Ingredient()
		{
			Type = IngredientType.Complex;
			Name = "No Name";
		}

		#endregion Public Constructors


		#region Public Properties

		public bool EditModeActive
		{
			get { return !_entryModeActive; }
		}

		public bool EntryModeActive
		{
			get { return _entryModeActive; }
			set
			{
				_entryModeActive = value;
				setChildIngredientsEntryMode(value);
			}
		}
		public bool HasChildren
		{
			get => Convert.ToBoolean(_ingredients?.Any());
		}

		public double Hydration
		{
			get { return _hydration; }
			set { _hydration = value; }
		}

		public ObservableCollection<Ingredient> Ingredients
		{
			get { return _ingredients; }
			set
			{
				if (_ingredients == null) _ingredients = new ObservableCollection<Ingredient>();
				_ingredients = value;
			}
		}

		public string Name
		{
			get { return _name; }
			set { Name = value; }
		}

		public double Percent
		{
			get { return Math.Round(_percent, 2); }
			set
			{
				if (double.IsNaN(value)) return;

				if (value > 99) value = 99;
				if (value < .001) value = .001;



				double oldPercent = _percent;
				_percent = value;
				PercentageChanged?.Invoke(this, new PercentChangedEventArgs(oldPercent, _percent));
			}
		}

		public bool RatioLocked
		{
			get { return ratioLocked; }
			set { ratioLocked = value; }
		}

		public bool ShowChildren
		{
			get { return showChildren; }
			set { showChildren = value; }
		}

		public IngredientType Type
		{
			get { return _type; }
			set /// ToDo: raise event to re-evaluate parent _type when children are edited
			{
				//__handle simple, most common case of no child _ingredients
				if (_ingredients?.Count == 0 || _ingredients == null)
				{
					_type = value;
				}
				else
				{
					SetTypeFromChildren();
				}
				//Parent.UpdateHydration();
			}
		}



		public double Weight
		{
			get { return Math.Round(_weight, 1); }
			set
			{
				if (double.IsNaN(value)) return;
				if (_weight == value) return;

				//_in both modes, will update child weights using existing percentages

				if (EntryModeActive)
				{
					UpdateChildrenWeightInEntryMode(value);
					
				}
				else
				{
					UpdateChildrenWeightInEditMode(value);

				}

				double oldWeight = _weight;

				_weight = value;

				WeightChanged?.Invoke(this, new WeightChangedEventArgs() { OldWeight = oldWeight, NewWeight = _weight });
				HydrationChanged?.Invoke(this, new EventArgs());
			}
		}

		#endregion Public Properties


		#region Public Methods

		public bool AddIngredient(Ingredient add = null)
		{
			if (_ingredients == null) _ingredients = new ObservableCollection<Ingredient>();

			if (add == null) //__create new ingredient with defaults
			{
				add = new Ingredient("Ingredient", IngredientType.Complex);
			}

			if (_ingredients.Any(i => i.Name == add.Name)) add.Name += "_1";

			//add.Parent = this;

			Ingredients.Add(add);
			BalancePercentages();
			ShowChildren = true;
			LinkChildEvents(add);
			return true;
		}

		public void AdjustPercent(double value)
		{
			if (RatioLocked) return;
			if (value > 100) value = 100;
			if (value < .001) value = .001;
			_percent = value;
		}

		public void AdjustWeight(double newParentWeight)
		{
			_weight = newParentWeight * _percent / 100;
			if (EditModeActive) UpdateChildrenWeightInEditMode(_weight);
		}

		public void BalancePercentages()
		{
			if (_ingredients == null) return;
			double currentTotal = Ingredients.Sum(i => i.Percent);

			double percentLeft = 100;
			for (int i = 0; i < _ingredients.Count; i++)
			{
				Ingredient ingredient = _ingredients[i];
				if (i == _ingredients.Count - 1) ingredient.AdjustPercent(percentLeft);
				else
				{
					double newPercent = (ingredient.Percent / currentTotal) * 100;
					percentLeft -= newPercent;
					ingredient.AdjustPercent(newPercent);
				}
			}
			UpdateChildrenWeightInEditMode(_weight);
		}

		public void DeleteChild(Ingredient child)
		{
			if (_ingredients.Contains(child))
			{
				_ingredients.Remove(child);
				BalancePercentages();
			}
		}

		public double getDryWeight()
		{
			double dryWeight = _type == IngredientType.Dry ? _weight : 0;
			if (_ingredients == null || _ingredients.Count == 0) return dryWeight;
			return _ingredients.Sum(i => i.getDryWeight());
		}

		public double GetExactPercent()
		{
			return _percent;
		}

		public double GetExactWeight()
		{
			return _weight;
		}

		public double getWetWeight()
		{
			double wetWeight = _type == IngredientType.Wet ? _weight : 0;
			if (_ingredients == null || _ingredients.Count == 0) return wetWeight;
			return _ingredients.Sum(i => i.getWetWeight());
		}

		public void LinkAllChildEvents()
		{
			if (Ingredients is null || Ingredients?.Count == 0) return;
			foreach (Ingredient ingredient in Ingredients)
			{
				LinkChildEvents(ingredient);
				ingredient.LinkAllChildEvents();
			}
		}

		public void LinkChildEvents(Ingredient ingredient)
		{
			//__ todo: add weakevent link
			ingredient.WeightChanged += UpdateToNewChildWeight;
			ingredient.PercentageChanged += UpdateToNewChildPercent;
			ingredient.TypeChanged += OnTypeChanged;
			ingredient.HydrationChanged += OnHydrationChanged;
		}

		public void OnHydrationChanged(object sender, EventArgs args)
		{
			double dryWeight = _ingredients.Sum(i => i.getDryWeight());
			double wetWeight = _ingredients.Sum(i => i.getWetWeight());
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

		public void OnTypeChanged(Object sender, EventArgs args)
		{
			if (Ingredients.Count == 0) return;
			IngredientType sampleType = Ingredients[0].Type;
			Type = Ingredients.All(i => i.Type == sampleType) ? sampleType : IngredientType.Complex;
		}

		public void SetEntryMode(bool entryModeEnabled)
		{
			EntryModeActive = entryModeEnabled;
			foreach (Ingredient item in Ingredients)
			{
				item.SetEntryMode(entryModeEnabled);
			}
		}

		public void SetTypeFromChildren()
		{
			if (_ingredients == null || _ingredients.Count == 0) return;

			////__call this recursively on children to maintain consistency
			//foreach (Ingredient i in _ingredients)
			//{
			//	i.SetTypeFromChildren();
			//}

			//__need to check for any children which aren't of same _type as this might change
			if (_ingredients.Any(i => i._type != _type))
			{
				_type = IngredientType.Complex;
			}

			//__also use child _type if all are same
			if (Ingredients.All(i => _ingredients[0].Type == i.Type))
			{
				_type = Ingredients[0].Type;
			}
		}
		public void UnFreezeChildren()
		{
			foreach (var ingredient in _ingredients)
			{
				ingredient.UnFreezeChildren();
				ingredient.AdjustWeight(_weight * ingredient.GetExactPercent() / 100);
			}
		}

		public void UpdateChildrenWeightInEditMode(double newWeight)
		{
			if (_ingredients == null || _ingredients?.Count == 0) return;

			foreach (Ingredient i in _ingredients)
			{
				i.AdjustWeight(newWeight);
			}
		}

		public void UpdateChildrenWeightInEntryMode(double newWeight)
		{
			if (Ingredients != null && Ingredients?.Count > 0)
			{
				
				foreach (Ingredient i in _ingredients)
				{
					i.AdjustWeight(i._percent*_weight/100);
				}
			}
		}

		public void UpdateHydration()
		{
			HydrationChanged?.Invoke(this, EventArgs.Empty);
		}

		public void UpdateSelfToNewChildWeightInEntryMode()
		{
			Weight = Ingredients.Sum(i => i.GetExactWeight());
			
			UpdateHydration();
		}

		public void UpdateToNewChildPercent(object sender, PercentChangedEventArgs percentChangedArgs)
		{
			Ingredient senderIngredient = (Ingredient)sender;
			
			if (senderIngredient is null) return;
			double newPercent = percentChangedArgs.NewPercent;
			double oldOthersPercentTotal = _ingredients.Where(i => i != sender).Sum(i => i._percent);
			double newOthersPercentTotal = 100 - newPercent;
			double deltaPercent = newOthersPercentTotal / oldOthersPercentTotal;
			foreach (Ingredient i in _ingredients)
			{
				if (i == sender) continue;
				i.AdjustPercent(i._percent * deltaPercent);

			}

			//__now that percentages are set, update weights
			UpdateChildrenWeightInEditMode(Weight);
		}

		public void UpdateToNewChildWeight(object sender, WeightChangedEventArgs args)
		{
			Ingredient ingredientContainer = (Ingredient)sender;
			if (EditModeActive) UpdateToNewChildWeightInEditMode(ingredientContainer, args);
			if (EntryModeActive)
			{
				setChildPercentagesFromWeight();
				UpdateSelfToNewChildWeightInEntryMode();
				UpdateHydration();
			}
		}

		private void setChildPercentagesFromWeight()
		{
			double childWeightTotal = Ingredients.Sum(i => i.Weight);
			foreach (Ingredient ingredient in Ingredients)
			{
				ingredient.AdjustPercent(ingredient.Weight/childWeightTotal);
			}
		}

		public void UpdateToNewChildWeightInEditMode(object senderObject, WeightChangedEventArgs weightChangedArgs)
		{
			Ingredient sender = (Ingredient)senderObject;
			WeightChangedEventArgs myWeightChangedArgs = new WeightChangedEventArgs();
			myWeightChangedArgs.OldWeight = _weight;
			//__calculate new total
			_weight = weightChangedArgs.NewWeight / sender.GetExactPercent();
			myWeightChangedArgs.NewWeight = _weight;

			//__set new weight for each sibling
			IEnumerable<Ingredient> siblings = Ingredients.Where(i => i != sender);
			foreach (var ingredient in siblings)
			{
				ingredient.AdjustWeight(ingredient.GetExactPercent() * _weight);
			}
		}

		#endregion Public Methods


		#region Public Events

		public event EventHandler HydrationChanged;

		public event EventHandler<PercentChangedEventArgs> PercentageChanged;

		public event EventHandler TypeChanged;

		public event EventHandler<WeightChangedEventArgs> WeightChanged;

		/// <summary>
		/// When EntryMode is turned off, percentages need to be updated to the entered weights
		/// </summary>
		public event EventHandler EntryModeExited;

		#endregion Public Events

	}
}