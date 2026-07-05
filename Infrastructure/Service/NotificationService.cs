using Core.Entities.OrderAggregate;
using Core.Entities.AdoptionApp;
using Core.Interfaces;
using Core.Constants;
using Microsoft.AspNetCore.SignalR;
using Application.SignalR;
using Application.SignalR.DTOs;

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
            if (order?.BuyerEmail == null)
                return;

            var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);

            if (!string.IsNullOrEmpty(connectionId))
            {
                var data = new OrderNotificationData
                {
                    OrderId = order.Id,
                    Status = order.Status.ToString(),
                    Total = order.GetTotal(),
                    Subtotal = order.Subtotal,
                    DeliveryPrice = order.DeliveryMethod?.Price ?? 0,
                    ItemsCount = order.OrderItems?.Sum(i => i.Quantity) ?? 0,
                    DeliveryMethod = order.DeliveryMethod?.ShortName ?? string.Empty,
                    DeliveryTime = order.DeliveryMethod?.DeliveryTime ?? string.Empty
                };

                var notification = new NotificationDto
                {
                    Type = NotificationType.OrderPaymentReceived,
                    Severity = NotificationSeverity.Success,
                    Title = "Payment Received",
                    Message = "? Payment received — your order is confirmed!",
                    Data = data,
                    ActionUrl = $"/orders/{order.Id}",
                    RequiresAction = false
                };

                await _hubContext.Clients.Client(connectionId).SendAsync("Notification", notification);
            }
        }

        public async Task NotifyOrderFailedAsync(Order order, string errorMessage)
        {
            if (order?.BuyerEmail == null)
                return;

            var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);

            if (!string.IsNullOrEmpty(connectionId))
            {
                var data = new OrderNotificationData
                {
                    OrderId = order.Id,
                    Status = order.Status.ToString(),
                    Total = order.GetTotal(),
                    Subtotal = order.Subtotal,
                    DeliveryPrice = order.DeliveryMethod?.Price ?? 0,
                    ItemsCount = order.OrderItems?.Sum(i => i.Quantity) ?? 0,
                    DeliveryMethod = order.DeliveryMethod?.ShortName ?? string.Empty,
                    DeliveryTime = order.DeliveryMethod?.DeliveryTime ?? string.Empty,
                    ErrorDetails = errorMessage
                };

                var notification = new NotificationDto
                {
                    Type = NotificationType.OrderPaymentFailed,
                    Severity = NotificationSeverity.Error,
                    Title = "Payment Failed",
                    Message = "? Payment failed — please retry checkout or contact support.",
                    Data = data,
                    ActionUrl = $"/checkout",
                    RequiresAction = true
                };

                await _hubContext.Clients.Client(connectionId).SendAsync("Notification", notification);
            }
        }

        public async Task NotifyApplicationRejectedAsync(AdoptionApplication application)
        {
            if (application?.Applicant?.Email == null)
                return;

            var connectionId = NotificationHub.GetConnectionIdByEmail(application.Applicant.Email);
            if (connectionId != null)
            {
                var data = new ApplicationNotificationData
                {
                    ApplicationId = application.Id,
                    Status = "Rejected",
                    AnimalName = application.Animal?.Name ?? "Unknown",
                    AdminNotes = application.AdminNotes,
                    RequiresUserAction = false
                };

                var notification = new NotificationDto
                {
                    Type = NotificationType.ApplicationRejected,
                    Severity = NotificationSeverity.Warning,
                    Title = "Application Rejected",
                    Message = $"Your application for {application.Animal?.Name} has been rejected.",
                    Data = data,
                    ActionUrl = $"/applications/{application.Id}",
                    RequiresAction = false
                };

                await _hubContext.Clients.Client(connectionId).SendAsync("Notification", notification);
            }
        }

        public async Task NotifyApplicationAcceptedAsync(AdoptionApplication application)
        {
            if (application?.Applicant?.Email == null)
                return;

            var connectionId = NotificationHub.GetConnectionIdByEmail(application.Applicant.Email);
            if (connectionId != null)
            {
                var data = new ApplicationNotificationData
                {
                    ApplicationId = application.Id,
                    Status = "Accepted",
                    AnimalName = application.Animal?.Name ?? "Unknown",
                    AdminNotes = application.AdminNotes,
                    RequiresUserAction = true
                };

                var notification = new NotificationDto
                {
                    Type = NotificationType.ApplicationAccepted,
                    Severity = NotificationSeverity.Success,
                    Title = "Application Accepted",
                    Message = $"Congratulations! Your application for {application.Animal?.Name} has been accepted.",
                    Data = data,
                    ActionUrl = $"/applications/{application.Id}",
                    RequiresAction = true
                };

                await _hubContext.Clients.Client(connectionId).SendAsync("Notification", notification);
            }
        }
    }
}
