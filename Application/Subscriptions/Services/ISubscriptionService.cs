using Application.Subscriptions.DTOs;
using Core.Constants;

namespace Application.Subscriptions.Services;

public interface ISubscriptionService
{
    Task<List<SubscriptionPlanDto>> GetSubscriptionsByTypeAsync(PlanType? type);
}