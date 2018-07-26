namespace RecipeMaster.Models
{
	public interface IIngredientContainer
	{
		#region Public Properties

		System.Collections.ObjectModel.ObservableCollection<Ingredient> Ingredients { get; set; }

		#endregion Public Properties

		#region Public Methods

		void DeleteChild(Ingredient sender);

		void SetEntryMode(bool EntryModeActive);

		void UpdateChildrenWeightInEditMode(double newWeight = 0);

		void UpdateChildrenWeightInEntryMode(double newWeight = 0);

		void UpdateHydration();

		void UpdateSelfToNewChildWeightInEntryMode();

		void UpdateToNewChildPercent(Ingredient sender, double newPercent);

		void UpdateToNewChildWeightInEditMode(Ingredient sender, double newWeight);

		#endregion Public Methods
	}
}