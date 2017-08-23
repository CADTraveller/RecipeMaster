using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMaster.Models
{
    public interface IIngredientContainer
    {
        

        System.Collections.ObjectModel.ObservableCollection<Ingredient> Ingredients { get; set; }


        void UpdateSelfToNewChildWeightInEntryMode();

        void UpdateToNewChildWeightInEditMode(Ingredient sender, double newWeight);

        void UpdateChildrenWeightInEntryMode(double newWeight);

        void UpdateChildrenWeightInEditMode(double newWeight);

        void UpdateToNewChildPercent(Ingredient sender, double newPercent);

        void DeleteChild(Ingredient sender);

        void UpdateHydration();

        void SetEntryMode(bool EntryModeActive);
    }
}
