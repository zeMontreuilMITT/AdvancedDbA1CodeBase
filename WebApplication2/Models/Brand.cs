using WebApplication2.Models;

public class Brand
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Laptop> Laptops { get; set; } = new HashSet<Laptop>();
}
