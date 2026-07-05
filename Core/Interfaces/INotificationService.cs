using Core.Entities.OrderAggregate;
using Core.Entities.AdoptionApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface INotificationService
    {
        Task NotifyOrderCompletedAsync(Order order);
        Task NotifyOrderFailedAsync(Order order, string errorMessage);
        Task NotifyApplicationRejectedAsync(AdoptionApplication application);
        Task NotifyApplicationAcceptedAsync(AdoptionApplication application);
    }
}
