using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using WebApplication2.Models;




var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.SeedData();
};

app.MapFallback(() =>
{
    return Results.NotFound(new { message = "Write correct information!" });
});
//An endpoint that shows all of the laptops available in a store with stores/{storeNumber}/inventory.
//Laptops with 0 or less quantity should not be shown.

//This endpoint will by default show all laptops in the database

app.MapGet("/laptops/search", async (ApplicationDbContext db) =>
{
    var laptops = db.Laptops.Include(l => l.Brand).AsQueryable();

    return await laptops.Select(l => new
    {
        l.IdNumber,
        l.Model,
        l.Price,
        l.YearOfMake,
        l.Condition,
        Brand = new { l.Brand.Id, l.Brand.Name }
    }).ToListAsync();
});
//Price above [amount}

app.MapGet("/laptops/search/priceAbove/{amount}", async (ApplicationDbContext db, decimal amount) =>
{
    var laptops = db.Laptops.Include(l => l.Brand)
                             .Where(l => l.Price > amount)
                             .AsQueryable();

    return await laptops.Select(l => new
    {
        l.IdNumber,
        l.Model,
        l.Price,
        l.YearOfMake,
        l.Condition,
        Brand = new { l.Brand.Id, l.Brand.Name }
    }).ToListAsync();
});

//Price above [amount}

app.MapGet("/laptops/search/priceBelow/{amount}", async (ApplicationDbContext db, decimal amount) =>
{
    var laptops = db.Laptops.Include(l => l.Brand)
                             .Where(l => l.Price < amount)
                             .AsQueryable();

    return await laptops.Select(l => new
    {
        l.IdNumber,
        l.Model,
        l.Price,
        l.YearOfMake,
        l.Condition,
        Brand = new { l.Brand.Id, l.Brand.Name }
    }).ToListAsync();
});
//Has stock greater than zero at store
//[store number] OR a stock greater than zero in any store in [province] 
//laptops/instock
app.MapGet("/laptops/instock", async (ApplicationDbContext db) =>
{
    var inStockLaptops = await db.LaptopStoreLocations
                                 .Include(ls => ls.Laptop)
                                 .ThenInclude(l => l.Brand)
                                 .Include(ls => ls.StoreLocation)
                                 .Where(ls => ls.Quantity > 0)
                                 .Select(ls => new
                                 {
                                     ls.Laptop.IdNumber,
                                     ls.Laptop.Model,
                                     ls.Laptop.Price,
                                     ls.Laptop.YearOfMake,
                                     ls.Laptop.Condition,
                                     Brand = new { ls.Laptop.Brand.Id, ls.Laptop.Brand.Name },
                                     Store = new
                                     {
                                         ls.StoreLocation.StoreNumber,
                                         Province = ((Province)ls.StoreLocation.Province).ToString()
                                     },
                                     InStockQuantity = ls.Quantity
                                 })
                                 .ToListAsync();

    return inStockLaptops;
});

// laptops -in-condition / New
app.MapGet("/laptops-in-condition/{condition}", async (ApplicationDbContext db, LaptopCondition condition) =>
{
    var laptopsInSpecifiedCondition = await db.Laptops
                                              .Include(l => l.Brand)
                                              .Where(l => l.Condition == condition)  // Filtering based on provided condition.
                                              .Select(l => new
                                              {
                                                  l.IdNumber,
                                                  l.Model,
                                                  l.Price,
                                                  l.YearOfMake,
                                                  Condition = l.Condition.ToString(),  // Return the string representation of the condition.
                                                  Brand = new { l.Brand.Id, l.Brand.Name },
                                                  InStock = l.Quantity
                                              })
                                              .ToListAsync();

    return laptopsInSpecifiedCondition;
});


///laptops-by-brand/2
app.MapGet("/laptops-by-brand/{brandId}", async (ApplicationDbContext db, int brandId) =>
{
    var laptopsByBrand = await db.Laptops
                                .Include(l => l.Brand)
                                .Where(l => l.Brand.Id == brandId)
                                .Select(l => new
                                {
                                    l.IdNumber,
                                    l.Model,
                                    l.Price,
                                    l.YearOfMake,
                                    Condition = l.Condition.ToString(),
                                    Brand = new { l.Brand.Id, l.Brand.Name },
                                    InStock = l.Quantity
                                })
                                .ToListAsync();

    return laptopsByBrand;
});


//laptops-search/Zephyrus
app.MapGet("/laptops-search/{searchPhrase}", async (ApplicationDbContext db, string searchPhrase) =>
{
    var laptopsWithSearchPhrase = await db.Laptops
                                          .Include(l => l.Brand)
                                          .Where(l => l.Model.Contains(searchPhrase))
                                          .Select(l => new
                                          {
                                              l.IdNumber,
                                              l.Model,
                                              l.Price,
                                              l.YearOfMake,
                                              Condition = l.Condition.ToString(),
                                              Brand = new { l.Brand.Id, l.Brand.Name },
                                              InStock = l.Quantity
                                          })
                                          .ToListAsync();

    return laptopsWithSearchPhrase;
});
;
//An endpoint that shows all of the laptops available in a store with stores/{storeNumber}/inventory.
//Laptops with 0 or less quantity should not be shown.
app.MapGet("/stores/{storeNumber:guid}/inventory", async (ApplicationDbContext db, Guid storeNumber) =>
{
    var laptopsInStore = await db.LaptopStoreLocations
                                 .Where(lsl => lsl.StoreLocationId == storeNumber && lsl.Quantity > 0)
                                 .Include(lsl => lsl.Laptop)
                                 .ThenInclude(laptop => laptop.Brand)
                                 .Select(lsl => new
                                 {
                                     IdNumber = lsl.Laptop.IdNumber,
                                     Model = lsl.Laptop.Model,
                                     Price = lsl.Laptop.Price,
                                     YearOfMake = lsl.Laptop.YearOfMake,
                                     Condition = lsl.Laptop.Condition,
                                     Brand = new
                                     {
                                         Id = lsl.Laptop.Brand.Id,
                                         Name = lsl.Laptop.Brand.Name
                                     },
                                     InStockQuantity = lsl.Quantity
                                 })
                                 .ToListAsync();

    return laptopsInStore;
});

app.MapPost("/stores/{storeNumber:guid}/{laptopNumber:guid}/changeQuantity", async (ApplicationDbContext db, Guid storeNumber, Guid laptopNumber, int amount) =>
{
    var laptopStoreEntry = await db.LaptopStoreLocations
                                   .Where(lsl => lsl.StoreLocationId == storeNumber && lsl.LaptopId == laptopNumber)
                                   .FirstOrDefaultAsync();

    if (laptopStoreEntry != null)
    {
        laptopStoreEntry.Quantity = amount;
        await db.SaveChangesAsync();

        return Results.Ok(new { Message = "Quantity updated successfully!", UpdatedQuantity = amount });
    }
    else
    {
        return Results.NotFound(new { Message = "The specified laptop-store relation was not found!" });
    }
});
app.MapGet("/brands/{brandId:int}/averagePrice", async (int brandId) =>
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var result = await dbContext.Laptops
                                .Where(l => l.Brand.Id == brandId)
                                .GroupBy(l => l.Brand.Id)
                                .Select(g => new
                                {
                                    LaptopCount = g.Count(),
                                    AveragePrice = g.Average(l => l.Price)
                                })
                                .FirstOrDefaultAsync();

    if (result == null)
    {
        return Results.NotFound($"No laptops found for brand ID {brandId}.");
    }

    return Results.Json(new
    {
        LaptopCount = result.LaptopCount,
        AveragePrice = result.AveragePrice
    });
});
app.MapGet("/stores/groupedByProvince", async (ApplicationDbContext db) =>
{
    var groupedStores = await db.StoreLocations
                                .GroupBy(store => store.Province)
                                .Where(group => group.Any()) // This ensures that only provinces with stores are included
                                .Select(group => new 
                                {
                                    Province = group.Key.ToString(),
                                    Stores = group.Select(store => new 
                                    {
                                        StoreNumber = store.StoreNumber,
                                        StreetNameAndNumber = store.StreetNameAndNumber
                                    }).ToList()
                                })
                                .ToListAsync();

    return Results.Json(groupedStores);
});
app.MapGet("/storesByProvince", async (ApplicationDbContext db) =>
{
    var groupedStoresByProvince = await db.StoreLocations
                                          .GroupBy(s => s.Province)
                                          .Where(group => group.Any())
                                          .Select(group => new
                                          {
                                              Province = group.Key.ToString(),
                                              Stores = group.Select(store => new
                                              {
                                                  StoreNumber = store.StoreNumber,
                                                  StreetNameAndNumber = store.StreetNameAndNumber
                                              }).ToList()
                                          })
                                          .ToListAsync();

    return Results.Json(groupedStoresByProvince);
});




app.Run();