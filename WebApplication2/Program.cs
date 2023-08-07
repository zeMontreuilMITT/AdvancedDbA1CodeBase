using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication2.Data;
using WebApplication2.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebApplication2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<LaptopStoreContext>();

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                SeedData.Initialize(context);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void ConfigureEndpoints(IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/laptops/search", async context =>
                {
                    List<Laptop> laptops = SearchLaptops(
                        context.RequestServices.GetRequiredService<LaptopStoreContext>(),
                        context.Request.Query["priceAbove"].FirstOrDefault(),
                        context.Request.Query["priceBelow"].FirstOrDefault(),
                        context.Request.Query["storeNumber"].FirstOrDefault(),
                        context.Request.Query["province"].FirstOrDefault(),
                        context.Request.Query["condition"].FirstOrDefault(),
                        context.Request.Query["brandId"].FirstOrDefault(),
                        context.Request.Query["searchPhrase"].FirstOrDefault()
                    );
                    await context.Response.WriteAsync(JsonSerializer.Serialize(laptops));
                });

                endpoints.MapGet("/stores/{storeNumber}/inventory", async context =>
                {
                    Guid storeNumberValue = Guid.Parse(context.Request.RouteValues["storeNumber"].ToString());
                    List<Laptop> laptopsInStore = GetLaptopsInStore(
                        context.RequestServices.GetRequiredService<LaptopStoreContext>(),
                        storeNumberValue
                    );
                    await context.Response.WriteAsync(JsonSerializer.Serialize(laptopsInStore));
                });

                endpoints.MapPost("/stores/{storeNumber}/{laptopNumber}/changeQuantity", async context =>
                {
                    Guid storeNumberValue = Guid.Parse(context.Request.RouteValues["storeNumber"].ToString());
                    Guid laptopNumberValue = Guid.Parse(context.Request.RouteValues["laptopNumber"].ToString());
                    int amountValue = int.Parse(context.Request.Query["amount"].FirstOrDefault());
                    await ChangeLaptopQuantityInStore(
                        context.RequestServices.GetRequiredService<LaptopStoreContext>(),
                        storeNumberValue,
                        laptopNumberValue,
                        amountValue
                    );
                    await context.Response.WriteAsync("Quantity changed successfully.");
                });

                endpoints.MapGet("/brands/{brandId}/averagePrice", async context =>
                {
                    int brandIdValue = int.Parse(context.Request.RouteValues["brandId"].ToString());
                    (int LaptopCount, double AveragePrice) averagePrice = GetAveragePriceByBrand(
                        context.RequestServices.GetRequiredService<LaptopStoreContext>(),
                        brandIdValue
                    );
                    var result = new { LaptopCount = averagePrice.LaptopCount, AveragePrice = averagePrice.AveragePrice };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(result));
                });

                endpoints.MapGet("/stores/groupedByProvince", async context =>
                {
                    Dictionary<CanadianProvince, List<Store>> groupedStores = GroupStoresByProvince(
                        context.RequestServices.GetRequiredService<LaptopStoreContext>()
                    );
                    await context.Response.WriteAsync(JsonSerializer.Serialize(groupedStores));
                });
            });
        }

        public static List<Laptop> SearchLaptops(
            LaptopStoreContext context,
            string priceAbove = null,
            string priceBelow = null,
            string storeNumber = null,
            string province = null,
            string condition = null,
            string brandId = null,
            string searchPhrase = null)
        {
            var laptops = context.Laptops.AsQueryable();

            if (!string.IsNullOrWhiteSpace(priceAbove))
            {
                decimal priceAboveValue = decimal.Parse(priceAbove);
                laptops = laptops.Where(l => l.Price > priceAboveValue);
            }

            if (!string.IsNullOrWhiteSpace(priceBelow))
            {
                decimal priceBelowValue = decimal.Parse(priceBelow);
                laptops = laptops.Where(l => l.Price < priceBelowValue);
            }

            if (!string.IsNullOrWhiteSpace(storeNumber))
            {
                Guid storeNumberValue = Guid.Parse(storeNumber);
                laptops = laptops.Where(l => l.Store.StoreNumber == storeNumberValue);
            }
            else if (!string.IsNullOrWhiteSpace(province))
            {
                CanadianProvince provinceValue = Enum.Parse<CanadianProvince>(province);
                laptops = laptops.Where(l => l.Store.Province == provinceValue && l.Quantity > 0);
            }

            if (!string.IsNullOrWhiteSpace(condition))
            {
                LaptopCondition conditionValue = Enum.Parse<LaptopCondition>(condition);
                laptops = laptops.Where(l => l.Condition == conditionValue);
            }

            if (!string.IsNullOrWhiteSpace(brandId))
            {
                int brandIdValue = int.Parse(brandId);
                laptops = laptops.Where(l => l.BrandId == brandIdValue);
            }

            if (!string.IsNullOrWhiteSpace(searchPhrase))
            {
                laptops = laptops.Where(l => l.Model.Contains(searchPhrase));
            }

            return laptops.ToList();
        }

        public static List<Laptop> GetLaptopsInStore(LaptopStoreContext context, Guid storeNumber)
        {
            return context.Laptops.Where(l => l.Store.StoreNumber == storeNumber && l.Quantity > 0).ToList();
        }

        public static async Task ChangeLaptopQuantityInStore(LaptopStoreContext context, Guid storeNumber, Guid laptopNumber, int amount)
        {
            var laptop = context.Laptops.FirstOrDefault(l => l.Number == laptopNumber && l.Store.StoreNumber == storeNumber);
            if (laptop != null)
            {
                laptop.Quantity += amount;
                await context.SaveChangesAsync();
            }
        }

        public static (int LaptopCount, double AveragePrice) GetAveragePriceByBrand(LaptopStoreContext context, int brandId)
        {
            var laptops = context.Laptops.Where(l => l.BrandId == brandId).ToList();
            var laptopCount = laptops.Count;
            var averagePrice = laptopCount > 0 ? (double)laptops.Average(l => (double)l.Price) : 0;
            return (laptopCount, averagePrice);
        }


        public static Dictionary<CanadianProvince, List<Store>> GroupStoresByProvince(LaptopStoreContext context)
        {
            return context.Stores
                .GroupBy(s => s.Province)
                .Where(g => g.Any())
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}
