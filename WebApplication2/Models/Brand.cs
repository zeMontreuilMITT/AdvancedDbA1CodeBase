namespace WebApplication2.Models
{
    public class Brand
    {
        public int Id { get; set; }
        
        public string _name;
        
        public string Name { get => _name;
            set
            {
                if (string.IsNullOrEmpty(value) || value.Length < 3 )
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Brand name must be at least three characters in length.");
                }
                _name = value;
            }

        }

        public string Model { get; set; }

        public HashSet<Laptop> Laptops = new HashSet<Laptop>();

        private void SetBrand(string name)
        {
            string[] laptopBrands =
            {
                "Dell",
                "Asus",
                "Lenovo"
            };
            if (!laptopBrands.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid brand. Please choose a valid brand.", nameof(name));
            }
            Name = name;
        }

        public Brand() 
        {
        
        }

        public Brand(int id, string name, string model)
        {
            Id = id;
            Name = name;
            Model = model;
        }
    }
}
