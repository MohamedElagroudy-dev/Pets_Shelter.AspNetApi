using Core.Entities.OrderAggregate;
using Core.Entities.AdoptionApp;
using Core.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Application.SignalR;

namespace Infrastructure.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyOrderCompletedAsync(Order order)
        {
            // Implement order completed notification if needed
            await Task.CompletedTask;
        }

        public async Task NotifyApplicationRejectedAsync(AdoptionApplication application)
        {
            if (application?.Applicant?.Email == null)
                return;

            var connectionId = NotificationHub.GetConnectionIdByEmail(application.Applicant.Email);
            if (connectionId != null)
            {
                var message = new
                {
                    applicationId = application.Id,
                    status = "Rejected",
                    animalName = application.Animal?.Name ?? "Unknown",
                    adminNotes = application.AdminNotes ?? string.Empty,
                    message = $"Your application for {application.Animal?.Name} has been rejected.",
                    timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients.Client(connectionId)
                    .SendAsync("ReceiveApplicationNotification", message);
            }
        }

        public async Task NotifyApplicationAcceptedAsync(AdoptionApplication application)
        {
            if (application?.Applicant?.Email == null)
                return;

            var connectionId = NotificationHub.GetConnectionIdByEmail(application.Applicant.Email);
            if (connectionId != null)
            {
                var message = new
                {
                    applicationId = application.Id,
                    status = "Accepted",
                    animalName = application.Animal?.Name ?? "Unknown",
                    adminNotes = application.AdminNotes ?? string.Empty,
                    message = $"Your application for {application.Animal?.Name} has been accepted.",
                    timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients.Client(connectionId)
                    .SendAsync("ReceiveApplicationNotification", message);
            }
        }
    }
}
