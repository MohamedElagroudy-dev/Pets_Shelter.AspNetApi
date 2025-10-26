using Core.Entities;
using Core.Entities.Product;
using Ecom.Core.Entities.Product;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace Infrastructure.Seeders
{
    public class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any(x => x.UserName == "admin@test.com"))
            {
                var user = new AppUser
                {
                    UserName = "admin@test.com",
                    Email = "admin@test.com"
                };


                await userManager.CreateAsync(user, "Password@123");
                await userManager.AddToRoleAsync(user, "Admin");
            }

            if (!context.Products.Any())
            {
                var productsData = await File.ReadAllTextAsync("../Infrastructure/Seeders/SeedData/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                if (products != null && products.Count > 0)
                {
                    context.Products.AddRange(products);
                    await context.SaveChangesAsync();
                }
            }

            if (!context.DeliveryMethods.Any())
            {
                var dmData = await File.ReadAllTextAsync("../Infrastructure/Seeders/SeedData/delivery.json");
                var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(dmData);

                if (methods != null && methods.Count > 0)
                {
                    context.DeliveryMethods.AddRange(methods);
                    await context.SaveChangesAsync();
                }
            }

            if (!context.Categories.Any())
            {
                var catData = await File.ReadAllTextAsync("../Infrastructure/Seeders/SeedData/categories.json");
                var categories = JsonSerializer.Deserialize<List<Category>>(catData);

                if (categories != null && categories.Count > 0)
                {
                    context.Categories.AddRange(categories);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
