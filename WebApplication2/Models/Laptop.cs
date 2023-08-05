using System.Reflection.Metadata.Ecma335;

namespace WebApplication2.Models
{
    public class Laptop
    {
        public Guid Number { get; set; }

        private string _model;

        public string Model
        {
            get => _model;
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length < 3)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Laptop model name must be at least three characters in length.");
                }
                // set model to value to assign values to property
                _model = value;
            }
        }

        public int Quantity { get; set; }

        private double _price;

        public double Price
        {
            get => _price;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Price cannot be less than zero.");
                }

                _price = value;
            }
        }

        public LaptopCondition Condition { get; set; }

        public int BrandId { get; set; }

        public Brand Brand { get; set; }

        public List<Store> Stores { get; set; }

        public Laptop()
        {

        }

        public Laptop(Guid number, string model, double price, LaptopCondition condition,
            Brand brand, int quantity)
        {
            Number = number;
            Model = model;
            Price = price;
            Condition = condition;
            Brand = brand;
            Stores = new List<Store>();
            Quantity = quantity;
        }
    }

    public enum LaptopCondition
    {
        New,
        Refurbished,
        Rental
    }

    
}
