using Application.Subscriptions.DTOs;
using Core.Entities.Plans;
using Core.Constants;
using System.Linq;
using System.Collections.Generic;

namespace Application.Subscriptions.Mappings;

public static class SubscriptionMappingExtensions
{
    public static SubscriptionPlanDto ToDto(this SubscriptionPlan plan)
    {
        return new SubscriptionPlanDto
        {
            Id = plan.Id,
            Name = plan.Name,
            Price = plan.Price,
            PlanType = plan.PlanType,
            Features = plan.Features?.Select(f => f.FeatureText).ToList() ?? new List<string>(),
            DurationInMonths = plan.DurationInMonths
        };
    }
}