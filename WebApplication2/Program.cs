using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("SlnDbContext");

builder.Services.AddDbContext<SlnDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

// Getting

app.MapGet("/laptops/search", (double? priceAbove, double? priceBelow, Guid? storeId,
                               string? province, LaptopCondition? condition, int? brandId, string? searchPhrase) =>
{
    try
    {
        // Start with all laptops in the database
        HashSet<Laptop> laptops = InternalDatabase.Laptops.ToHashSet();

        // Apply filters based on query parameters
        if (priceAbove.HasValue)
        {
            laptops = laptops.Where(laptop => laptop.Price > priceAbove.Value).ToHashSet();
        }

        if (priceBelow.HasValue)
        {
            laptops = laptops.Where(laptop => laptop.Price < priceBelow.Value).ToHashSet();
        }

        if (storeId.HasValue || !string.IsNullOrEmpty(province))
        {
            laptops = laptops.Where(laptop => laptop.Stores.Any(store =>
                (storeId.HasValue && store.StoreId == storeId.Value) ||
                (!string.IsNullOrEmpty(province) && store.Province == province))).ToHashSet();
        }

        if (condition.HasValue)
        {
            laptops = laptops.Where(laptop => laptop.Condition == condition.Value).ToHashSet();
        }

        if (brandId.HasValue)
        {
            laptops = laptops.Where(laptop => laptop.BrandId == brandId.Value).ToHashSet();
        }

        if (!string.IsNullOrEmpty(searchPhrase))
        {
            laptops = laptops.Where(laptop => laptop.Model.Contains(searchPhrase)).ToHashSet();
        }

        return Results.Ok(laptops);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/stores", ()=>
{
    return InternalDatabase.Stores;
});

app.MapGet("/stores/{storeNumber}/inventory", (Guid storeNumber) =>
{
    try
    {
        Store store = InternalDatabase.Stores.FirstOrDefault(s => s.StoreId == storeNumber);
        if (store == null)
        {
            return Results.NotFound($"Store with Store Number '{storeNumber}' not found.");
        }

        List<Laptop> laptopsInStore = store.Laptops.Where(laptop => laptop.Quantity > 0).ToList();

        return Results.Ok(laptopsInStore);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/brands", () =>
{
    return InternalDatabase.Brands;
});

app.MapGet("/brands/{brandName}/averagePrice", (string brandName) =>
{
    try
    {
        // Find brand with queried brandName
        Brand brand = InternalDatabase.Brands.FirstOrDefault(b => b.Name.Equals(brandName, StringComparison.OrdinalIgnoreCase));
        if (brand == null)
        {
            return Results.NotFound($"Brand '{brandName}' not found.");
        }

        // Get the laptops of said brand
        HashSet<Laptop> laptopsForBrand = InternalDatabase.Laptops.Where(l => 
            l.Brand.Name.Equals(brandName, StringComparison.OrdinalIgnoreCase)).ToHashSet();

        // Calculate average price and count of laptops
        int laptopCount = laptopsForBrand.Count();
        double averagePrice = laptopCount > 0 ? laptopsForBrand.Average(l => l.Price) : 0;

        Object result = new
        {
            LaptopCount = laptopCount,
            AveragePrice = averagePrice
        };

        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/stores/province", () =>
{
    try
    {
        // Group stores by province
        string[] provinces = InternalDatabase.Stores
            .Select(store => store.Province)
            .Distinct()
            .ToArray();

        // Filter out provinces with no stores
        HashSet<object> result = new HashSet<object>();
        foreach (string province in provinces)
        {
            HashSet<Store> storesInProvince = new HashSet<Store>();
            foreach (Store store in InternalDatabase.Stores)
            {
                if (store.Province == province)
                {
                    storesInProvince.Add(store);
                }
            }

            if (storesInProvince.Count > 0)
            {
                result.Add(new
                {
                    Province = province,
                    Stores = storesInProvince
                });
            }
        }

        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// Posting

app.MapPost("/stores/{storeNumber}/{laptopNumber}/changeQuantity", (Guid storeNumber, Guid laptopNumber, int amount) =>
{
    try
    {
        Store store = InternalDatabase.Stores.FirstOrDefault(s => s.StoreId == storeNumber);
        if (store == null)
        {
            return Results.NotFound($"Store {storeNumber} not found.");
        }

        // Find laptop with given laptopNumber in the store
        Laptop laptop = store.Laptops.FirstOrDefault(l => l.Number == laptopNumber);
        if (laptop == null)
        {
            return Results.NotFound($"Laptop with Laptop Number '{laptopNumber}' not found in Store '{storeNumber}'.");
        }

        // Update laptop's quantity with new amount
        laptop.Quantity = amount;

        return Results.Ok("Laptop quantity updated.");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();



static class InternalDatabase
{

    public static HashSet<Brand> Brands = new HashSet<Brand>();

    public static HashSet<Laptop> Laptops = new HashSet<Laptop>();

    public static HashSet<Store> Stores = new HashSet<Store>();

    private static LaptopCondition Refurbished;
    private static LaptopCondition New;
    private static LaptopCondition Rental;

    static InternalDatabase()
    {
        _seedMethodBrand();
        _seedMethodLaptop();
        _seedMethodStore();
    }

    public static void CreateBrand(int id, string name, string model)
    {
        if (String.IsNullOrEmpty(name) && String.IsNullOrEmpty(model))
        {
            throw new ArgumentOutOfRangeException("Brand name cannot be empty.");
        }
            Brand newBrand = new Brand(id, name, model);
            Brands.Add(newBrand);
        
    }

    private static void _seedMethodBrand()
    {
        CreateBrand(1, "Dell", "Inspirion");
        CreateBrand(2, "Dell", "XPS 15");
        CreateBrand(3, "Dell", "G15");
        CreateBrand(4, "Asus", "VivoBook");
        CreateBrand(5, "Asus", "ZenBook");
        CreateBrand(6, "Asus", "Zephyrus");
        CreateBrand(7, "Lenovo", "ThinkPad");
        CreateBrand(8, "Lenovo", "IdeaPad");
        CreateBrand(9, "Lenovo", "Legion");
    }


    public static void CreateLaptop(Guid number, string model, double price, 
        LaptopCondition condition, Brand brand, int quantity)
    {
        if (String.IsNullOrEmpty(model))
        {
            throw new ArgumentOutOfRangeException("Laptop must have a name");
        }

        Laptop newLaptop = new Laptop(number, model, price, condition, brand, quantity);
        Laptops.Add(newLaptop);
    }

    private static void _seedMethodLaptop()
    {
        // messy i know sorry. unsure of how to link the brand names and guids from other class
        Guid laptopGuid1 = Guid.NewGuid();
        Guid laptopGuid2 = Guid.NewGuid();
        Guid laptopGuid3 = Guid.NewGuid();
        Guid laptopGuid4 = Guid.NewGuid();
        Guid laptopGuid5 = Guid.NewGuid();
        Guid laptopGuid6 = Guid.NewGuid();
        Guid laptopGuid7 = Guid.NewGuid();
        Guid laptopGuid8 = Guid.NewGuid();
        Guid laptopGuid9 = Guid.NewGuid();

        Brand Dell = new Brand { Name = "Dell"};
        Brand Asus = new Brand { Name = "Asus" };
        Brand Lenovo = new Brand { Name = "Lenovo" };
    
        CreateLaptop(laptopGuid1, "Inspirion", 749.50, LaptopCondition.Refurbished, Dell, 10);
        CreateLaptop(laptopGuid2, "XPS 15", 1899.99, LaptopCondition.New, Dell, 22);
        CreateLaptop(laptopGuid3, "G15", 1999.99, LaptopCondition.New, Dell, 13);
        CreateLaptop(laptopGuid4, "VivoBook", 899.99, LaptopCondition.Rental, Asus, -2);
        CreateLaptop(laptopGuid5, "ZenBook", 999.99, LaptopCondition.Refurbished, Asus, 4);
        CreateLaptop(laptopGuid6, "Zephyrus", 1499.99, LaptopCondition.New, Asus, 33);
        CreateLaptop(laptopGuid7, "ThinkPad", 949.50, LaptopCondition.New, Lenovo, 14);
        CreateLaptop(laptopGuid8, "IdeaPad", 764.50, LaptopCondition.Rental, Lenovo, 19);
        CreateLaptop(laptopGuid9, "Legion", 1329.99, LaptopCondition.New, Lenovo, -3);
    }

    public static void _seedMethodStore()
    {
        // Creating stores without create method
        Store store1 = new Store(Guid.NewGuid(), "123 Main St", "Ontario");
        Store store2 = new Store(Guid.NewGuid(), "456 Elm St", "Quebec");

        // Add laptops to hashset
        Laptop laptop1 = new Laptop(Guid.NewGuid(), "Inspirion", 749.50, LaptopCondition.Refurbished, GetBrandByName("Dell"), 10);
        Laptop laptop2 = new Laptop(Guid.NewGuid(), "XPS 15", 1899.99, LaptopCondition.New, GetBrandByName("Dell"), 22);
        Laptop laptop3 = new Laptop(Guid.NewGuid(), "G15", 1999.99, LaptopCondition.New, GetBrandByName("Dell"), 13);
        Laptop laptop4 = new Laptop(Guid.NewGuid(), "VivoBook", 899.99, LaptopCondition.Rental, GetBrandByName("Asus"), -2);
        Laptop laptop5 = new Laptop(Guid.NewGuid(), "ZenBook", 999.99, LaptopCondition.Refurbished, GetBrandByName("Asus"), 4);
        Laptop laptop6 = new Laptop(Guid.NewGuid(), "Zephyrus", 1499.99, LaptopCondition.New, GetBrandByName("Asus"), 33);
        Laptop laptop7 = new Laptop(Guid.NewGuid(), "ThinkPad", 949.50, LaptopCondition.New, GetBrandByName("Lenovo"), 14);
        Laptop laptop8 = new Laptop(Guid.NewGuid(), "IdeaPad", 764.50, LaptopCondition.Rental, GetBrandByName("Lenovo"), 19);
        Laptop laptop9 = new Laptop(Guid.NewGuid(), "Legion", 1329.99, LaptopCondition.New, GetBrandByName("Lenovo"), -3);

        store1.Laptops.Add(laptop2);
        store1.Laptops.Add(laptop3);
        store1.Laptops.Add(laptop5);
        store1.Laptops.Add(laptop6);
        store1.Laptops.Add(laptop7);
        store1.Laptops.Add(laptop8);


        store2.Laptops.Add(laptop1);
        store2.Laptops.Add(laptop2);
        store2.Laptops.Add(laptop4);
        store2.Laptops.Add(laptop5);
        store2.Laptops.Add(laptop8);
        store2.Laptops.Add(laptop9);

        Stores.Add(store1);
        Stores.Add(store2);
    }

    private static Brand GetBrandByName(string name)
    {
        return Brands.FirstOrDefault(brand => brand.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}