using Core.Entities;
using Core.Entities.Animal;
using Core.Entities.Chat;
using Core.Entities.OrderAggregate;
using Core.Entities.Plans;
using Core.Entities.Product;
using Ecom.Core.Entities.Product;
using Infrastructure.Config;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Infrastructure.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<AppUser>(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<Order>  Orders{ get; set; }
        public DbSet<OrderItem>  OrdersItems{ get; set; }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<PetType> PetTypes { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public DbSet<Favorite> Favorites { get; set; } 
        public DbSet<BaseAnimal> Animals { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public DbSet<PlanFeature> PlanFeatures { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

        public DbSet<ChatRoom> ChatRooms => Set<ChatRoom>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<UserNotification> UserNotifications => Set<UserNotification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfiguration).Assembly);

          
        }
    }
}
