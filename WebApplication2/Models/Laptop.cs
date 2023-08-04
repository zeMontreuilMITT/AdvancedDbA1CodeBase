using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Collections.Generic;


namespace WebApplication2.Models
{
    public class Laptop
    {
        [Key]
        public Guid IdNumber { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }




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
                _model = value;
            }
        }

        private decimal _price;

        public decimal Price
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


        public int YearOfMake { get; set; }
        public int Quantity { get; set; }
        public int ViewCount { get; set; } = 0;
        public ICollection<LaptopStoreLocation> LaptopStoreLocations { get; set; }

        public Laptop(Guid IdNumber, string model, int brandId, decimal price, int yearOfMake, LaptopCondition condition, int quantity)
        {
            this.IdNumber = IdNumber;
            Model = model;
            BrandId = brandId;
            Price = price;
            YearOfMake = yearOfMake;
            Condition = condition;
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
