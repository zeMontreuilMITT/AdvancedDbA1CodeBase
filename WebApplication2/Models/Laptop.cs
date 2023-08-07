using WebApplication2.Models;
using System.ComponentModel.DataAnnotations;


public class Laptop
{
    [Key]
    public Guid Number { get; set; }
    public string Model { get; set; }
    public decimal Price { get; set; }
    public LaptopCondition Condition { get; set; }

    public int BrandId { get; set; }
    public Brand Brand { get; set; }
    public Guid StoreNumber { get; set; }
    public Store Store { get; set; }
    public int Quantity { get;  set; }
}

public enum LaptopCondition
{
    New,
    Refurbished,
    Rental
}
