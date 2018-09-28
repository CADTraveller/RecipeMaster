using GalaSoft.MvvmLight;


namespace RecipeMaster.Models
{
    public class Step:ObservableObject
    {
        public Step()
        {
            Description = "Please edit this description";
        }

        public Step(string about)
        {
            Description = about;
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { Set(() => Description, ref description, value); }
        }

        private int order;

        public int Order
        {
            get { return order; }
            set { Set(() => Order, ref order, value); }
        }

        private int duration;
        public int Duration
        {
            get { return duration; }
            set { Set(() => Duration, ref duration, value); }
        }



    }
}
