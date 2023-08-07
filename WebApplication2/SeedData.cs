using WebApplication2.Data;
using WebApplication2.Models;
using System;
using System.Collections.Generic;

namespace WebApplication2
{
    public static class SeedData
    {
        public static void Initialize(LaptopStoreContext context)
        {
            // Seed data logic...
            var brand1 = new Brand { Name = "Dell" };
            var brand2 = new Brand { Name = "HP" };
            var brand3 = new Brand { Name = "Lenovo" };
            var brand4 = new Brand { Name = "Asus" };

            var store1 = new Store
            {
                StoreNumber = Guid.NewGuid(),
                StreetName = "Main Street",
                StreetNumber = "123",
                Province = CanadianProvince.Ontario
            };

            var store2 = new Store
            {
                StoreNumber = Guid.NewGuid(),
                StreetName = "Maple Avenue",
                StreetNumber = "456",
                Province = CanadianProvince.Quebec
            };

            var laptops = new List<Laptop>
            {
                new Laptop { Model = "Dell XPS 13", Price = 1299.99m, Condition = LaptopCondition.New, Brand = brand1, Store = store1, Quantity = 5 },
                new Laptop { Model = "Dell Inspiron 15", Price = 699.99m, Condition = LaptopCondition.Refurbished, Brand = brand1, Store = store1, Quantity = 3 },
                new Laptop { Model = "Dell Latitude 14", Price = 1599.99m, Condition = LaptopCondition.New, Brand = brand1, Store = store2, Quantity = 7 },
                new Laptop { Model = "Dell Alienware m15", Price = 2499.99m, Condition = LaptopCondition.New, Brand = brand1, Store = store2, Quantity = 4 },

                new Laptop { Model = "HP Spectre x360", Price = 1199.99m, Condition = LaptopCondition.New, Brand = brand2, Store = store1, Quantity = 6 },
                new Laptop { Model = "HP Envy 13", Price = 799.99m, Condition = LaptopCondition.Refurbished, Brand = brand2, Store = store1, Quantity = 2 },
                new Laptop { Model = "HP Pavilion 15", Price = 999.99m, Condition = LaptopCondition.New, Brand = brand2, Store = store2, Quantity = 8 },
                new Laptop { Model = "HP Omen 17", Price = 1799.99m, Condition = LaptopCondition.New, Brand = brand2, Store = store2, Quantity = 5 },

                new Laptop { Model = "Lenovo ThinkPad X1 Carbon", Price = 1399.99m, Condition = LaptopCondition.New, Brand = brand3, Store = store1, Quantity = 7 },
                new Laptop { Model = "Lenovo Ideapad 5", Price = 699.99m, Condition = LaptopCondition.Refurbished, Brand = brand3, Store = store1, Quantity = 4 },
                new Laptop { Model = "Lenovo Yoga C940", Price = 1299.99m, Condition = LaptopCondition.New, Brand = brand3, Store = store2, Quantity = 9 },
                new Laptop { Model = "Lenovo Legion 5", Price = 1699.99m, Condition = LaptopCondition.New, Brand = brand3, Store = store2, Quantity = 3 },

                new Laptop { Model = "Asus ZenBook 14", Price = 999.99m, Condition = LaptopCondition.New, Brand = brand4, Store = store1, Quantity = 4 },
                new Laptop { Model = "Asus VivoBook S15", Price = 599.99m, Condition = LaptopCondition.Refurbished, Brand = brand4, Store = store1, Quantity = 6 },
                new Laptop { Model = "Asus ROG Zephyrus G14", Price = 1499.99m, Condition = LaptopCondition.New, Brand = brand4, Store = store2, Quantity = 10 },
                new Laptop { Model = "Asus TUF Gaming A15", Price = 1299.99m, Condition = LaptopCondition.New, Brand = brand4, Store = store2, Quantity = 5 }
            };

            context.Brands.AddRange(brand1, brand2, brand3, brand4);
            context.Stores.AddRange(store1, store2);
            context.Laptops.AddRange(laptops);

            context.SaveChanges();
        }
    }
}
