using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class LaptopStoreContext : DbContext
    {
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Laptop> Laptops { get; set; }
        public DbSet<Store> Stores { get; set; }

        public LaptopStoreContext(DbContextOptions<LaptopStoreContext> options)
            : base(options)
        {
        }

    }
}
