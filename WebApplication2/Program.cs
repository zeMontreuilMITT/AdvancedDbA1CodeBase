using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApplication2.Data;
using Microsoft.AspNetCore.Http.Json;
using WebApplication2.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<LaptopStoreContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("LaptopStoreConnectionString"));
});

builder.Services.Configure<JsonOptions>(options => {
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider serviceProvider = scope.ServiceProvider;

    await SeedData.Initialize(serviceProvider);
}

// ENDPOINTS

app.MapGet("/laptops/search", (LaptopStoreContext db, decimal? priceAbove,
    decimal? priceBelow, Guid? store, Province? province, LaptopCondition? condition,
    int? brandId, string? searchPhrase) =>
{
    try
    {
        if (priceAbove < 0 || priceBelow < 0)
        {
            return Results.BadRequest("Price cannot be less than zero. ");
        }

        if (priceBelow != null && priceAbove != null)
        {
            // Find laptops in the database
            HashSet<Laptop> laptopsFounded = db.Laptop.Where(l => l.Price >= priceAbove && l.Price <= priceBelow)
            .Include(l => l.Brand)
            .Include(l => l.LaptopStores)
            .ThenInclude(l => l.StoreLocation)
            .ToHashSet();

            return Results.Ok(laptopsFounded);
        }


        if (priceAbove != null)
        {
            // Find laptops in the database
            HashSet<Laptop> laptopsFounded = db.Laptop.Where(l => l.Price >= priceAbove)
            .Include(l => l.Brand)
            .Include(l => l.LaptopStores)
            .ThenInclude(l => l.StoreLocation)
            .ToHashSet();
            return Results.Ok(laptopsFounded);
        }

        if (priceBelow != null)
        {
            // Find laptops in the database
            HashSet<Laptop> laptopsFounded = db.Laptop.Where(l => l.Price <= priceBelow)
            .Include(l => l.Brand)
            .Include(l => l.LaptopStores)
            .ThenInclude(l => l.StoreLocation)
            .ToHashSet();
            return Results.Ok(laptopsFounded);
        }

        if (store != null)
        {
            HashSet<Laptop_Store> laptopsFounded = db.Laptop_Stores.Where(l => l.StoreNumber == store && l.Quantity > 0)
            .Include(l => l.Laptop)
            .ThenInclude(l => l.Brand)
            .ToHashSet();
            ;
            return Results.Ok(laptopsFounded);
        }

        if (province.HasValue && Enum.IsDefined(typeof(Province), province.Value))
        {

            HashSet<Laptop_Store> laptopsFounded = db.Laptop_Stores.Where(ls => ls.StoreLocation.Province.Equals(province) && ls.Quantity > 0)
            .Include(l => l.Laptop)
            .ThenInclude(l => l.Brand)
            .Include(l => l.StoreLocation)
            .ToHashSet();

            return Results.Ok(laptopsFounded);
        }

        if (condition.HasValue && Enum.IsDefined(typeof(LaptopCondition), condition.Value))
        {

            HashSet<Laptop> laptopsFounded = db.Laptop.Where(ls => ls.Condition.Equals(condition))
            .Include(l => l.Brand)
            .Include(l => l.LaptopStores)
            .ThenInclude(l => l.StoreLocation)
            .ToHashSet();

            return Results.Ok(laptopsFounded);
        }

        if (brandId.HasValue && int.TryParse(brandId.ToString(), out int intValue))
        {
            // Find laptops in the database
            HashSet<Laptop> laptopsFounded = db.Laptop.Where(l => l.BrandId == brandId)
            .Include(l => l.Brand)
            .Include(l => l.LaptopStores)
            .ThenInclude(l => l.StoreLocation)
            .ToHashSet();

            return Results.Ok(laptopsFounded);
        }

        if (!String.IsNullOrEmpty(searchPhrase))
        {
            // Find laptops in the database
            HashSet<Laptop> laptopsFounded = db.Laptop.Where(l => l.Model.Contains(searchPhrase))
            .Include(l => l.Brand)
            .Include(l => l.LaptopStores)
            .ThenInclude(l => l.StoreLocation)
            .ToHashSet();

            return Results.Ok(laptopsFounded);
        }

        else
        {
            return Results.Ok(db.Laptop
                .Include(l => l.Brand)
                .Include(l => l.LaptopStores)
                .ThenInclude(l => l.StoreLocation)
                .ToHashSet());
        }
    }
    catch (InvalidOperationException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// An endpoint that shows all of the laptops available in a store.
// Laptops with 0 or less quantity should not be shown.
app.MapGet("/stores/{storenumber}/inventory", (LaptopStoreContext db, Guid storeNumber) =>
{
    try
    {
        // Find data in the database
        HashSet<Laptop_Store> laptops = db.Laptop_Stores.Where
        (ls => ls.StoreNumber == storeNumber && ls.Quantity > 0)
        .Include(ls => ls.Laptop)
        .ThenInclude(ls => ls.Brand)
        .ToHashSet();

        return Results.Ok(laptops); ;
    }
    catch (InvalidOperationException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});


// An endpoint for posting a new Quantity for a Laptop at a specific store
app.MapPost("stores/{storeNumber}/{laptopNumber}/changeQuantity", (LaptopStoreContext db,
    Guid storeNumber, Guid laptopNumber, int quantity) =>
{
    try
    {
        Laptop_Store laptop = db.Laptop_Stores.FirstOrDefault(
            ls => ls.LaptopId == laptopNumber && ls.StoreNumber == storeNumber);

        laptop.Quantity = quantity;
        db.SaveChanges();

        return Results.Ok(laptop);

    }
    catch (InvalidOperationException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// An endpoint for getting the average price of all laptops among a specific
// brand, returned as { LaptopCount: [value], AveragePrice: [value]}
app.MapGet("/laptops/average", (LaptopStoreContext db, int brandId) =>
{
    try
    {
        // Find laptops in the database
        HashSet<Laptop> laptops = db.Laptop.Where(ls => ls.BrandId == brandId).ToHashSet();

        int laptopCount = laptops.Count();
        decimal averagePrice = laptops.Average(ls => ls.Price);

        return Results.Ok(new
        {
            LaptopCunt = laptopCount,
            AveragePrince = averagePrice
        }
        );

    }
    catch (InvalidOperationException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// An endpoint which dynamically groups and returns all Stores according to the
// Province in which they are in. 
app.MapGet("/stores/show", (LaptopStoreContext db) =>
{
    try
    {
        // Get stores from database
        List<StoreLocation> allStoreLocations = db.StoreLocation
        .Include(sl => sl.LaptopStores)
        .ThenInclude(sl => sl.Laptop)
        .ToList();

        // Group by Province 
        Dictionary<Province, HashSet<StoreLocation>> storesByProvince = allStoreLocations
            .GroupBy(sl => sl.Province)
            .ToDictionary(g => g.Key, g => g.ToHashSet());

        return Results.Ok(storesByProvince);

    }
    catch (InvalidOperationException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();