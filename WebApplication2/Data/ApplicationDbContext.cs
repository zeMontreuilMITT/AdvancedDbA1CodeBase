using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace WebApplication2.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Laptop> Laptops { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<StoreLocation> StoreLocations { get; set; }
        public DbSet<LaptopStoreLocation> LaptopStoreLocations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LaptopStoreLocation>()
          .HasKey(ls => new { ls.LaptopId, ls.StoreLocationId });

            modelBuilder.Entity<LaptopStoreLocation>()
                .HasOne(ls => ls.Laptop)
                .WithMany(l => l.LaptopStoreLocations)
                .HasForeignKey(ls => ls.LaptopId);

            modelBuilder.Entity<LaptopStoreLocation>()
                .HasOne(ls => ls.StoreLocation)
                .WithMany(s => s.LaptopStoreLocations)
                .HasForeignKey(ls => ls.StoreLocationId);


            modelBuilder.Entity<Laptop>()
              .HasKey(l => l.IdNumber);
        }






        public void SeedData()
        {
            // Check if the database exists, delete, and then recreate it
            Database.EnsureDeleted();
            Database.EnsureCreated();

            // Seeding Brands
            if (!Brands.Any())
            {
                Brands.AddRange(
                    new Brand { Name = "Asus" },
                    new Brand { Name = "Toshiba" },
                    new Brand { Name = "Apple" },
                    new Brand { Name = "Acer" }
                );
                SaveChanges();
            }

            // Getting Brand IDs
            var asusId = Brands.FirstOrDefault(b => b.Name == "Asus").Id;
            var toshibaId = Brands.FirstOrDefault(b => b.Name == "Toshiba").Id;
            var appleId = Brands.FirstOrDefault(b => b.Name == "Apple").Id;
            var acerId = Brands.FirstOrDefault(b => b.Name == "Acer").Id;

            // Seeding Laptops
            if (!Laptops.Any())
            {
                Laptops.AddRange(

                    new Laptop(Guid.NewGuid(), "ROG Zephyrus", 1, 1600, 2023, LaptopCondition.New, 5),
                    new Laptop(Guid.NewGuid(), "ZenBook 13", 1, 1100, 2023, LaptopCondition.Refurbished, 8),
                    new Laptop(Guid.NewGuid(), "VivoBook S", 1, 800, 2023, LaptopCondition.Rental, 6),
                    new Laptop(Guid.NewGuid(), "Satellite Pro", 2, 820, 2021, LaptopCondition.New, 4),
                    new Laptop(Guid.NewGuid(), "Tecra A50", 2, 1100, 2021, LaptopCondition.Refurbished, 5),
                    new Laptop(Guid.NewGuid(), "Portege X30", 2, 1300, 2022, LaptopCondition.New, 3),
                    new Laptop(Guid.NewGuid(), "Dynabook", 2, 1050, 2020, LaptopCondition.Rental, 8),
                    new Laptop(Guid.NewGuid(), "M1 MacBook Air", 3, 1000, 2023, LaptopCondition.New, 7),
                    new Laptop(Guid.NewGuid(), "M1 MacBook Pro", 3, 1300, 2023, LaptopCondition.Refurbished, 5),
                    new Laptop(Guid.NewGuid(), "MacBook", 3, 900, 2022, LaptopCondition.Rental, 9)
                );
                SaveChanges();
            }

            // Seeding StoreLocations
            if (!StoreLocations.Any())
            {
                StoreLocations.AddRange(
                    new StoreLocation
                    {
                        StoreNumber = Guid.NewGuid(),
                        StreetNameAndNumber = "123 Main St",
                        Province = Province.AB
                    },
                    new StoreLocation
                    {
                        StoreNumber = Guid.NewGuid(),
                        StreetNameAndNumber = "456 North Ave",
                        Province = Province.BC
                    },
                    new StoreLocation
                    {
                        StoreNumber = Guid.NewGuid(),
                        StreetNameAndNumber = "789 South Blvd",
                        Province = Province.MB
                    }
                );
                SaveChanges();

            }
            if (!LaptopStoreLocations.Any())
            {
                // Create a randomizer for stock quantity
                Random random = new Random();

                // Iterate over all laptops and stores, creating stock entries
                foreach (var laptop in Laptops)
                {
                    foreach (var store in StoreLocations)
                    {
                        LaptopStoreLocations.Add(new LaptopStoreLocation
                        {
                            LaptopId = laptop.IdNumber,
                            StoreLocationId = store.StoreNumber,
                            Quantity = random.Next(1, 11)  // Random quantity between 1 to 10
                        });
                    }
                }
                SaveChanges();


            }

        }
    }
}