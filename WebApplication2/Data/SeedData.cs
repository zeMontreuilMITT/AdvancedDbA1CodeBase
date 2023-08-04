using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class SeedData
    {
        public async static Task Initialize(IServiceProvider serviceProvider)
        {
            LaptopStoreContext db = new LaptopStoreContext(serviceProvider.GetRequiredService<DbContextOptions<LaptopStoreContext>>());

            db.Database.EnsureDeleted();
            db.Database.Migrate();

            // BRAND
            Brand brandOne = new Brand { Name = "Dell" };
            Brand brandTwo = new Brand { Name = "HP" };
            Brand brandThree = new Brand { Name = "Apple" };

            if (!db.Brand.Any())
            {
                db.Add(brandOne);
                db.Add(brandTwo);
                db.Add(brandThree);
                db.SaveChanges();
            };

            // LAPTOP
            Laptop laptopOne = new Laptop
            {
                LaptopId = Guid.NewGuid(),
                BrandId = brandOne.BrandId,
                Model = "XPS",
                Price = 1200,
                Condition = LaptopCondition.New,
            };

            Laptop laptopTwo = new Laptop
            {
                LaptopId = Guid.NewGuid(),
                BrandId = brandTwo.BrandId,
                Model = "EliteBook",
                Price = 1350,
                Condition = LaptopCondition.Rental,

            };

            Laptop laptopThree = new Laptop
            {
                LaptopId = Guid.NewGuid(),
                BrandId = brandTwo.BrandId,
                Model = "Spectre",
                Price = 1000,
                Condition = LaptopCondition.Refurbished,

            };

            Laptop laptopFour = new Laptop
            {
                LaptopId = Guid.NewGuid(),
                BrandId = brandThree.BrandId,
                Model = "MacBook",
                Price = 1800,
                Condition = LaptopCondition.New,

            };

            Laptop laptopFive = new Laptop
            {
                LaptopId = Guid.NewGuid(),
                BrandId = brandThree.BrandId,
                Model = "MacBookAir",
                Price = 2200,
                Condition = LaptopCondition.New,
            };

            if (!db.Laptop.Any())
            {
                db.Add(laptopOne);
                db.Add(laptopTwo);
                db.Add(laptopThree);
                db.Add(laptopFour);
                db.Add(laptopFive);
                db.SaveChanges();
            }

            // STORELOCATION
            StoreLocation storeOne = new StoreLocation
            {
                StoreNumber = Guid.NewGuid(),
                StreetNameAndNumber = "810 St. James St",
                Province = Province.Manitoba
            };

            StoreLocation storeTwo = new StoreLocation
            {
                StoreNumber = Guid.NewGuid(),
                StreetNameAndNumber = "695 Wilson Ave.",
                Province = Province.Ontario
            };

            StoreLocation storeThree = new StoreLocation
            {
                StoreNumber = Guid.NewGuid(),
                StreetNameAndNumber = "470 Rue Ste. Catherine O",
                Province = Province.Quebec
            };

            if (!db.StoreLocation.Any())
            {
                db.Add(storeOne);
                db.Add(storeTwo);
                db.Add(storeThree);
                db.SaveChanges();
            }

            // LAPTOP_STORE (LINKING TABLE)

            Laptop_Store laptopStoreOne = new Laptop_Store
            {
                Id = Guid.NewGuid(),
                StoreNumber = storeOne.StoreNumber,
                LaptopId = laptopOne.LaptopId,
                Quantity = 10
            };

            Laptop_Store laptopStoreTwo = new Laptop_Store
            {
                Id = Guid.NewGuid(),
                StoreNumber = storeTwo.StoreNumber,
                LaptopId = laptopOne.LaptopId,
                Quantity = 5
            };

            Laptop_Store laptopStoreThree = new Laptop_Store
            {
                Id = Guid.NewGuid(),
                StoreNumber = storeTwo.StoreNumber,
                LaptopId = laptopTwo.LaptopId,
                Quantity = 10
            };

            Laptop_Store laptopStoreFour = new Laptop_Store
            {
                Id = Guid.NewGuid(),
                StoreNumber = storeThree.StoreNumber,
                LaptopId = laptopFour.LaptopId,
                Quantity = 8
            };

            Laptop_Store laptopStoreFive = new Laptop_Store
            {
                Id = Guid.NewGuid(),
                StoreNumber = storeThree.StoreNumber,
                LaptopId = laptopFive.LaptopId,
                Quantity = 15
            };

            Laptop_Store laptopStoreSix = new Laptop_Store
            {
                Id = Guid.NewGuid(),
                StoreNumber = storeThree.StoreNumber,
                LaptopId = laptopFive.LaptopId,
                Quantity = 15
            };

            if (!db.Laptop_Stores.Any())
            {
                db.Add(laptopStoreOne);
                db.Add(laptopStoreTwo);
                db.Add(laptopStoreThree);
                db.Add(laptopStoreFour);
                db.Add(laptopStoreFive);
                db.Add(laptopStoreSix);
                db.SaveChanges();
            }
        }
     }
 } 

