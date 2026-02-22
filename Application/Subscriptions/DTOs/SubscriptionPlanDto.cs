using Core.Constants;

namespace Application.Subscriptions.DTOs;

public class SubscriptionPlanDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationInMonths { get; set; }
    public PlanType? PlanType { get; set; }
    public List<string> Features { get; set; } = new();
}