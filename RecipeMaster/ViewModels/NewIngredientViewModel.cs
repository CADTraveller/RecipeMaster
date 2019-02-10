using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMaster.ViewModels
{
	using System.Collections.ObjectModel;
	using Models;
	using Template10.Mvvm;

	public class NewIngredientViewModel : ViewModelBase
	{

		private string _name;

		public string Name
		{
			get { return _name; }
			set { Set(ref _name, value); }
		}


		public IngredientType ConvertedType
		{
			get { return (IngredientType)Enum.Parse(typeof(IngredientType), _selectedType); }

		}

		private string _selectedType;

		public string SelectedType
		{
			get { return _selectedType; }
			set { Set(ref _selectedType, value); }
		}

		private ObservableCollection<string> _types;

		public ObservableCollection<string> Types
		{
			get { return _types; }
			set { Set(ref _types, value); }
		}

		public NewIngredientViewModel()
		{
			var ingredientTypes = Enum.GetValues(typeof(IngredientType)) as IEnumerable<string>;
			Types = new ObservableCollection<string>(ingredientTypes);
		}
	}
}
