
using System.Dynamic;

namespace RecipeMasterModels
{
    public class Step
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
            set { description = value; }
        }

        private int order;

        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        private int duration;
        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }



    }
}
