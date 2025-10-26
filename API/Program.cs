
using API.Middleware;
using Application.Extensions;
using Core.Entities;
using Core.Extensions;
using E_commerce.Extensions;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Infrastructure.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.AddPresentation();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.Addcore();
            builder.Services.AddApplication();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }
            app.UseStaticFiles(); // Serves wwwroot


            app.UseMiddleware<ExceptionMiddleware>();
            try
            {
                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                await context.Database.MigrateAsync();
                await ApplicationDbContextSeed.SeedAsync(context,userManager);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
