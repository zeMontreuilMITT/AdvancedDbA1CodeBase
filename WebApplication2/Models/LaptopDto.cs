namespace WebApplication2.Models
{
    public class LaptopDto
    {
        public Guid Number { get; set; }
        public string Model { get; set; }
        public decimal Price { get; set; }
        public LaptopCondition Condition { get; set; }
        public Brand Brand { get; set; }
    }
}
