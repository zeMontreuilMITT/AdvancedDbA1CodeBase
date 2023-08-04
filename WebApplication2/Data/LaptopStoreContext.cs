using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using System.Net;

namespace WebApplication2.Data
{
    public class LaptopStoreContext:DbContext
    {
         
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Specify the data type for decimal property "Price" on entity
            // "Laptop"
            modelBuilder.Entity<Laptop>()
                .Property(l => l.Price)
                .HasColumnType("decimal(18, 2)");

             // Define the PK for StoreLocation (name is not Id).

            modelBuilder.Entity<StoreLocation>().HasKey(s => s.StoreNumber);

            // Composite key
            modelBuilder.Entity<Laptop_Store>().HasKey(ls => new { ls.Id, ls.StoreNumber, ls.LaptopId});

            modelBuilder.Entity<Laptop_Store>()
                .HasOne(ls => ls.StoreLocation)
                .WithMany(s => s.LaptopStores)
                .HasForeignKey(ls => ls.StoreNumber);

            modelBuilder.Entity<Laptop_Store>()
                .HasOne(ls => ls.Laptop)
                .WithMany(l => l.LaptopStores)
                .HasForeignKey(ls => ls.LaptopId);
        }

       
        public LaptopStoreContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Brand> Brand { get; set; } = null!;
        public DbSet<Laptop> Laptop { get; set; } = null!;
        public DbSet<StoreLocation> StoreLocation { get; set; } = null!;
        public DbSet<Laptop_Store> Laptop_Stores { get; set; } = null!;
    }
}
