using System.ComponentModel.DataAnnotations;
using WebApplication2.Models;

public class LaptopStoreLocation
{
    public Guid LaptopId { get; set; }
    public Laptop Laptop { get; set; }

    public Guid StoreLocationId { get; set; }
    public StoreLocation StoreLocation { get; set; }

    public int Quantity { get; set; }  
}
