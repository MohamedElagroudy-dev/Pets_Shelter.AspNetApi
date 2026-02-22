using Application.Subscriptions.DTOs;
using Application.Subscriptions.Mappings;
using Core.Constants;
using Core.Entities.Plans;
using Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Subscriptions.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IUnitOfWork _unitOfWork;

    public SubscriptionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<SubscriptionPlanDto>> GetSubscriptionsByTypeAsync(PlanType? type)
    {
        var plans = await _unitOfWork.Repository<SubscriptionPlan>().GetAllAsync(
            p => p.PlanType == type,
            p => p.Features
        );

        return plans.Select(p => p.ToDto()).ToList();
    }
}