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
    public class PlanFeatureConfiguration : IEntityTypeConfiguration<PlanFeature>
    {
        public void Configure(EntityTypeBuilder<PlanFeature> builder)
        {
            builder.HasData(

                // ===== Adoption Bronze (1)
                new PlanFeature { Id = 1, SubscriptionPlanId = 1, FeatureText = "3 months unlimited profiles" },
                new PlanFeature { Id = 2, SubscriptionPlanId = 1, FeatureText = "2 Instagram & Facebook stories per month" },

                // ===== Adoption Silver (2)
                new PlanFeature { Id = 3, SubscriptionPlanId = 2, FeatureText = "5 months unlimited profiles" },
                new PlanFeature { Id = 4, SubscriptionPlanId = 2, FeatureText = "2 Instagram & Facebook stories per month" },

                // ===== Adoption Golden (3)
                new PlanFeature { Id = 5, SubscriptionPlanId = 3, FeatureText = "7 months unlimited profiles" },
                new PlanFeature { Id = 6, SubscriptionPlanId = 3, FeatureText = "2 Instagram & Facebook stories per month" },
                new PlanFeature { Id = 7, SubscriptionPlanId = 3, FeatureText = "Free consultation" },

                // ===== Foster Bronze (4)
                new PlanFeature { Id = 8, SubscriptionPlanId = 4, FeatureText = "3 months unlimited foster listings" },
                new PlanFeature { Id = 9, SubscriptionPlanId = 4, FeatureText = "2 social media stories per month" },

                // ===== Foster Silver (5)
                new PlanFeature { Id = 10, SubscriptionPlanId = 5, FeatureText = "5 months unlimited foster listings" },
                new PlanFeature { Id = 11, SubscriptionPlanId = 5, FeatureText = "2 social media stories per month" },

                // ===== Foster Golden (6)
                new PlanFeature { Id = 12, SubscriptionPlanId = 6, FeatureText = "7 months unlimited foster listings" },
                new PlanFeature { Id = 13, SubscriptionPlanId = 6, FeatureText = "2 social media stories per month" },
                new PlanFeature { Id = 14, SubscriptionPlanId = 6, FeatureText = "Homepage highlight" },

                // ===== Sponsor Free (7)
                new PlanFeature { Id = 15, SubscriptionPlanId = 7, FeatureText = "Upload case for sponsorship" }
            );
        }
    }
}
