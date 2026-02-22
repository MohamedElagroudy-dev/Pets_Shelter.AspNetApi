using Core.Constants;
using Core.Entities.Plans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Config
{
    public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
        {
            builder
                .Property(p => p.Price)
                .HasPrecision(18, 2);
            builder.HasData(

                // ================= ADOPTION =================
                new SubscriptionPlan { Id = 1, Name = "Bronze Paws", Price = 300, DurationInMonths = 3, PlanType = PlanType.Adoption },
                new SubscriptionPlan { Id = 2, Name = "Silver Whiskers", Price = 450, DurationInMonths = 5, PlanType = PlanType.Adoption },
                new SubscriptionPlan { Id = 3, Name = "Golden Tails", Price = 600, DurationInMonths = 7, PlanType = PlanType.Adoption },

                // ================= FOSTER =================
                new SubscriptionPlan { Id = 4, Name = "Bronze Foster", Price = 300, DurationInMonths = 3, PlanType = PlanType.Foster },
                new SubscriptionPlan { Id = 5, Name = "Silver Foster", Price = 450, DurationInMonths = 5, PlanType = PlanType.Foster },
                new SubscriptionPlan { Id = 6, Name = "Golden Foster", Price = 600, DurationInMonths = 7, PlanType = PlanType.Foster },

                // ================= SPONSOR =================
                new SubscriptionPlan { Id = 7, Name = "Free Sponsor", Price = 0, DurationInMonths = 0, PlanType = PlanType.Sponsor }
            );
        }
    }
}
