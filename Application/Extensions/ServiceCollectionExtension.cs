


using Application.Account;
using Application.Account.Services;
using Application.Cart.Services;
using Application.Categories.Services;
using Application.Orders.Services;
using Application.Payment.Services;
using Application.Ratings.Services;
using Core.Interfaces;
using Ecom.Application.Products.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;


namespace Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplication(this IServiceCollection services)
        {
            var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICartAppService, CartAppService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IPaymentAppService, PaymentAppService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IRatingService, RatingService>();

            services.AddHttpContextAccessor(); // needed for IHttpContextAccessor
            services.AddScoped<IUserContext, UserContext>();
        }


    }
}