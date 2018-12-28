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

		void UpdateToNewChildWeight(Ingredient sender, WeightChangedEventArgs weightChangedArgs);

		void UpdateSelfToNewChildWeightInEntryMode();

		void UpdateToNewChildWeightInEditMode(Ingredient sender, WeightChangedEventArgs weightChangedArgs);

		void UpdateChildrenWeightInEntryMode(double newWeight = 0);

		void UpdateChildrenWeightInEditMode(double newWeight = 0);

		void UpdateToNewChildPercent(Ingredient sender, double newPercent);

		void DeleteChild(Ingredient sender);

		void UpdateHydration();

		void SetEntryMode(bool EntryModeActive);

		void LinkChildEvents();

		event EventHandler WeightChanged;

		event EventHandler PercentageChanged;

		event EventHandler TypeChanged;
	}
}

/*
 * namespace ImplementInterfaceEvents  
{  
    public interface IDrawingObject  
    {  
        event EventHandler ShapeChanged;  
    }  
    public class MyEventArgs : EventArgs   
    {  
        // class members  
    }  
    public class Shape : IDrawingObject  
    {  
        public event EventHandler ShapeChanged;  
        void ChangeShape()  
        {  
            // Do something here before the event…  

            OnShapeChanged(new MyEventArgs(?arguments?));  

            // or do something here after the event.   
        }  
        protected virtual void OnShapeChanged(MyEventArgs e)
		{
			ShapeChanged?.Invoke(this, e);
		}  
    }  

}

*/
