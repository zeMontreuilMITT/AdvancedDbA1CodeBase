namespace WebApplication2.Models
{
    public class Laptop_Store
    {
        public Guid Id { get; set; }

        public StoreLocation StoreLocation { get; set; } 
        public Guid StoreNumber { get; set; }

        public Laptop Laptop { get; set; }
        public Guid LaptopId { get; set; }

        public int Quantity { get; set; }
        // Quantity (stock) can be negative (back-order)
     
    }
}
