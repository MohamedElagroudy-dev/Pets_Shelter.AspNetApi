using Application.Account;
using Application.Account.Services;
using Application.Admin.Services;
using Application.Cart.Services;
using Application.Categories.Services;
using Application.Orders.Services;
using Application.Payment.Services;
using Application.PetTypes.Services;
using Application.Ratings.Services;
using Core.Interfaces;
using Ecom.Application.AdoptionApplications.Services;
using Ecom.Application.Animals.Services;
using Ecom.Application.Favorites.Services;
using Ecom.Application.FosterAnimals.Services;
using Ecom.Application.Products.Services;
using Microsoft.Extensions.DependencyInjection;


namespace Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplication(this IServiceCollection services)
        {
            var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IPetTypeService, PetTypeService>();
            services.AddScoped<ICartAppService, CartAppService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IPaymentAppService, PaymentAppService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<IAdminAppService, AdminAppService>();
            services.AddScoped<IAnimalService, AnimalService>();
            services.AddScoped<IAdoptionApplicationService, AdoptionApplicationService>();
            services.AddScoped<IFosterAnimalService, FosterAnimalService>();

            services.AddHttpContextAccessor(); // needed for IHttpContextAccessor
            services.AddScoped<IUserContext, UserContext>();
        }


    }
}