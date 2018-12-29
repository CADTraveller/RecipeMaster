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



		void UpdateToNewChildWeight(object sender, WeightChangedEventArgs weightChangedArgs);

		void UpdateSelfToNewChildWeightInEntryMode();

		void UpdateToNewChildWeightInEditMode(object sender, WeightChangedEventArgs weightChangedArgs);

		void UpdateChildrenWeightInEntryMode(double newWeight = 0);

		void UpdateChildrenWeightInEditMode(double newWeight = 0);

		void UpdateToNewChildPercent(object sender, PercentChangedEventArgs percentChangedArgs);

		void OnTypeChanged(object sender, EventArgs args);

		void OnHydrationChanged(object sender, EventArgs args);

		void DeleteChild(Ingredient sender);

		void UpdateHydration();

		void SetEntryMode(bool EntryModeActive);

		void LinkAllChildEvents();

		event EventHandler<WeightChangedEventArgs> WeightChanged;

		event EventHandler<PercentChangedEventArgs> PercentageChanged;

		event EventHandler TypeChanged;

		event EventHandler HydrationChanged;
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
