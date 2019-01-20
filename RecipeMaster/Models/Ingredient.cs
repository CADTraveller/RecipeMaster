using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Template10.Mvvm;
//using SourdoughMaster.Annotations;
using Windows.UI.Xaml;
using Template10.Common;
using WinRTXamlToolkit.Common;


namespace RecipeMaster.Models
{
	public class Ingredient : ObservableObject, IIngredientContainer
	{
		#region Properties

		private bool _entryModeActive;
		private ObservableCollection<Ingredient> _ingredients;
		private string _name;
		private double _percent;
		private IngredientType _type;
		private string _typeImage;


		public bool EditModeActive
		{
			get { return !_entryModeActive; }
		}

		public bool EntryModeActive
		{
			get { return _entryModeActive; }
			set { Set(ref _entryModeActive, value); }
		}

		public ObservableCollection<Ingredient> Ingredients
		{
			get { return _ingredients; }
			set
			{
				if (_ingredients == null) _ingredients = new ObservableCollection<Ingredient>();
				Set(ref _ingredients, value);
				ShowChildren = _ingredients.Count > 0;
			}
		}

		public string Name
		{
			get { return _name; }
			set { Set(ref _name, value); }
		}

		public double Percent
		{
			get { return Math.Round(_percent, 2); }
			set
			{
				if (double.IsNaN(value)) return;

				if (value > 99) value = 99;
				if (value < .001) value = .001;

				//Parent.UpdateToNewChildPercent(this, value);
				//Parent.UpdateChildrenWeightInEditMode();
				double oldPercent = _percent;
				Set(ref _percent, value);
				UpdateIngredientWeights();//__update children
				PercentageChanged?.Invoke(this, new PercentChangedEventArgs(oldPercent, _percent));
			}
		}

		public IngredientType Type
		{
			get { return _type; }
			set /// ToDo: raise event to re-evaluate parent _type when children are edited
			{
				//__handle simple, most common case of no child _ingredients
				if (_ingredients?.Count == 0 || _ingredients == null)
				{
					Set(ref _type, value);
				}
				else
				{
					SetTypeFromChildren();
				}

				RaisePropertyChanged("TypeImage");
				//Parent.UpdateHydration();
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
			get { return Math.Round(_weight, 1); }
			set
			{
				if (double.IsNaN(value)) return;
				if (_weight == value) return;
				if (EntryModeActive)
				{
					//EntryModeWeightChanged();//__ignore percentages and set total as sum 
					//__in Entry Mode child weights may have changed without updating percentages, do that now
					UpdateChildrenWeightInEntryMode(value);

				}
				else
				{
					//WeightChanged?.Invoke(this, value);//__adjust total by percentages while in Edit mode
					//__in edit mode
					UpdateChildrenWeightInEditMode(value);

				}

				double oldWeight = _weight;
				//Parent.UpdateHydration();
				Set(ref _weight, value);
				// if (EntryModeActive) return;
				WeightChanged?.Invoke(this, new WeightChangedEventArgs() { OldWeight = oldWeight, NewWeight = _weight });
				HydrationChanged?.Invoke(this, new EventArgs());
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

			RaisePropertyChanged(nameof(Type));
			RaisePropertyChanged(nameof(TypeImage));
		}

		#endregion


		bool _hasChildren = default(bool);


		bool ratioLocked = default(bool);

		private bool showChildren;
		private double _weight;

		public event EventHandler<WeightChangedEventArgs> WeightChanged;
		public event EventHandler<PercentChangedEventArgs> PercentageChanged;
		public event EventHandler TypeChanged;
		public event EventHandler HydrationChanged;

		public void UpdateToNewChildPercent(object sender, PercentChangedEventArgs percentChangedArgs)
		{
			Ingredient senderIngredient = (Ingredient) sender;
			if(senderIngredient is null) return;
			double newPercent = percentChangedArgs.NewPercent;
			double oldOthersPercentTotal = _ingredients.Where(i => i != sender).Sum(i => i._percent);
			double newOthersPercentTotal = 100 - newPercent;
			double deltaPercent = newOthersPercentTotal / oldOthersPercentTotal;
			foreach (Ingredient i in _ingredients)
			{
				if (i == sender) continue;
				i.AdjustPercent(i._percent * deltaPercent);
			}
		}

		public void UpdateChildrenWeightInEditMode(double newWeight)
		{
			if (_ingredients == null || _ingredients?.Count == 0) return;

			foreach (Ingredient i in _ingredients)
			{
				i._weight = newWeight * i._percent;
			}

			RaisePropertyChanged("Ingredients");
		}

		public void UpdateChildrenWeightInEntryMode(double newWeight)
		{
			if (Ingredients != null && Ingredients?.Count > 0)
			{
				//__in Entry Mode, child weights may have changed, first update current _percent
				double dCurrentTotalWeight = _ingredients.Sum(i => i._weight);
				foreach (Ingredient i in _ingredients)
				{
					i._percent = i._weight / dCurrentTotalWeight;
					i._weight = newWeight * i._percent;
				}

				RaisePropertyChanged("Ingredients");
			}
		}

		public Ingredient(string n, IngredientType t)
		{
			Name = n;
			Percent = 1;
			Type = t;
			_ingredients = new ObservableCollection<Ingredient>();
		}

		public Ingredient()
		{
			_ingredients = new ObservableCollection<Ingredient>();
		}

		public bool hasChildren
		{
			get { return _hasChildren; }
			set { Set(ref _hasChildren, value); }
		}

	

		public bool RatioLocked
		{
			get { return ratioLocked; }
			set { Set(ref ratioLocked, value); }
		}

		public bool ShowChildren
		{
			get { return showChildren; }
			set { Set(() => ShowChildren, ref showChildren, value); }
		}

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
			RaisePropertyChanged(nameof(Ingredients));
			hasChildren = true;
			return true;
		}

		public void AdjustIngredientPercentages(object senderObj, double newPercent)
		{
			if (_ingredients == null) return;
			ValueAdjusters.AdjustIngredientPercentages(senderObj, newPercent, _ingredients);
			UpdateIngredientWeights(senderObj, newPercent);
		}

		public void AdjustPercent(double value)
		{
			if (RatioLocked) return;
			if (value > 100) value = 100;
			if (value < .001) value = .001;
			_percent = value;
			RaisePropertyChanged("Percent");
		}

		public void AdjustWeight(double newWeight)
		{
			_weight = newWeight;
			RaisePropertyChanged("Weight");
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
		}

		public void DeleteChild(Ingredient child)
		{
			if (_ingredients.Contains(child))
			{
				_ingredients.Remove(child);
				BalancePercentages();
			}
		}

		public void FreezeChildren()
		{
			foreach (var ingredient in _ingredients)
			{
				ingredient.FreezeChildren();
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

		public void UnFreezeChildren()
		{
			foreach (var ingredient in _ingredients)
			{
				ingredient.UnFreezeChildren();
				ingredient.AdjustWeight(_weight * ingredient.GetExactPercent() / 100);
			}
		}


		public void UpdateHydration()
		{
			HydrationChanged?.Invoke(this, EventArgs.Empty);
		}

		public void UpdateIngredientWeights(object senderObj, double newPercent)
		{
			if (_ingredients == null) return;
			foreach (Ingredient ingredient in _ingredients)
			{
				if (ingredient.Equals(senderObj)) ingredient.AdjustWeight(newPercent * _weight / 100);
				else ingredient.AdjustWeight(ingredient.GetExactPercent() * _weight / 100);
			}
		}

		public void UpdateIngredientWeights()
		{
			if (_ingredients == null) return;
			foreach (Ingredient ingredient in _ingredients)
			{
				ingredient.AdjustWeight(ingredient.GetExactPercent() * _weight / 100);
				ingredient.UpdateIngredientWeights();
			}
		}

		public void OnTypeChanged(Object sender, EventArgs args)
		{
			if(Ingredients.Count == 0) return;
			IngredientType sampleType = Ingredients[0].Type;
			Type = Ingredients.All(i => i.Type == sampleType) ? sampleType : IngredientType.Complex;
		}

		public void UpdateToNewChildWeight(object sender, WeightChangedEventArgs args)
		{
			IIngredientContainer ingredientContainer = (IIngredientContainer) sender;
			if (EditModeActive) UpdateToNewChildWeightInEditMode(ingredientContainer, args);
			if (EntryModeActive)
			{
				UpdateSelfToNewChildWeightInEntryMode();
				UpdateHydration();
			}

			RaisePropertyChanged(nameof(Weight));
		}

		public void UpdateSelfToNewChildWeightInEntryMode()
		{
			_weight = Ingredients.Sum(i => i.GetExactWeight());
			UpdateHydration();
		}

		public void UpdateToNewChildWeightInEditMode(object senderObject, WeightChangedEventArgs weightChangedArgs)
		{
			Ingredient sender = (Ingredient) senderObject;
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
			RaisePropertyChanged(nameof(Ingredients));
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

		private void LinkChildEvents(Ingredient ingredient)
		{
//__ todo: add weakevent link
			ingredient.WeightChanged += UpdateToNewChildWeight;
			ingredient.PercentageChanged += UpdateToNewChildPercent;
			ingredient.TypeChanged += OnTypeChanged;
			ingredient.HydrationChanged += OnHydrationChanged;
		}

		public void OnHydrationChanged(object sender, EventArgs args)
		{
			throw new NotImplementedException();
		}
	}
}