// Infrastructure/Service/NotificationService.cs
using System.Text.Json;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Entities.AdoptionApp;
using Core.Interfaces;
using Core.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Application.SignalR;
using Application.SignalR.DTOs;
using Infrastructure.Persistence;

namespace Infrastructure.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public NotificationService(
            IHubContext<NotificationHub> hubContext,
            ApplicationDbContext db,
            UserManager<AppUser> userManager)
        {
            _hubContext = hubContext;
            _db = db;
            _userManager = userManager;
        }

        // ── Public notification methods ──────────────────────────────────────

        public async Task NotifyOrderCompletedAsync(Order order)
        {
            if (order?.BuyerEmail == null) return;

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

            await SaveAndPushAsync(order.BuyerEmail, new NotificationDto
            {
                Type = NotificationType.OrderPaymentReceived,
                Severity = NotificationSeverity.Success,
                Title = "Payment Received",
                Message = "Payment received — your order is confirmed!",
                Data = data,
                ActionUrl = $"/orders/{order.Id}",
                RequiresAction = false
            }, data);
        }

        public async Task NotifyOrderFailedAsync(Order order, string errorMessage)
        {
            if (order?.BuyerEmail == null) return;

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

            await SaveAndPushAsync(order.BuyerEmail, new NotificationDto
            {
                Type = NotificationType.OrderPaymentFailed,
                Severity = NotificationSeverity.Error,
                Title = "Payment Failed",
                Message = "Payment failed — please retry checkout or contact support.",
                Data = data,
                ActionUrl = "/checkout",
                RequiresAction = true
            }, data);
        }

        public async Task NotifyApplicationAcceptedAsync(AdoptionApplication application)
        {
            if (application?.Applicant?.Email == null) return;

            var data = new ApplicationNotificationData
            {
                ApplicationId = application.Id,
                Status = "Accepted",
                AnimalName = application.Animal?.Name ?? "Unknown",
                AdminNotes = application.AdminNotes,
                RequiresUserAction = true
            };

            await SaveAndPushAsync(application.Applicant.Email, new NotificationDto
            {
                Type = NotificationType.ApplicationAccepted,
                Severity = NotificationSeverity.Success,
                Title = "Application Accepted",
                Message = $"Congratulations! Your application for {application.Animal?.Name} has been accepted.",
                Data = data,
                ActionUrl = $"/applications/{application.Id}",
                RequiresAction = true
            }, data);
        }

        public async Task NotifyApplicationRejectedAsync(AdoptionApplication application)
        {
            if (application?.Applicant?.Email == null) return;

            var data = new ApplicationNotificationData
            {
                ApplicationId = application.Id,
                Status = "Rejected",
                AnimalName = application.Animal?.Name ?? "Unknown",
                AdminNotes = application.AdminNotes,
                RequiresUserAction = false
            };

            await SaveAndPushAsync(application.Applicant.Email, new NotificationDto
            {
                Type = NotificationType.ApplicationRejected,
                Severity = NotificationSeverity.Warning,
                Title = "Application Rejected",
                Message = $"Your application for {application.Animal?.Name} has been rejected.",
                Data = data,
                ActionUrl = $"/applications/{application.Id}",
                RequiresAction = false
            }, data);
        }

        // ── Called by NotificationHub.OnConnectedAsync ──────────────────────

        public async Task DeliverPendingNotificationsAsync(string userId)
        {
            var pending = await _db.UserNotifications
                .Where(n => n.UserId == userId && !n.IsDelivered)
                .OrderBy(n => n.CreatedAt)
                .ToListAsync();

            if (!pending.Any()) return;

            var connectionId = NotificationHub.GetConnectionIdByUserId(userId);
            if (string.IsNullOrEmpty(connectionId)) return;

            foreach (var notification in pending)
            {
                var dto = ToDto(notification);
                await _hubContext.Clients.Client(connectionId)
                    .SendAsync("Notification", dto);

                notification.IsDelivered = true;
            }

            await _db.SaveChangesAsync();
        }

        // ── Private helpers ─────────────────────────────────────────────────

        /// <summary>
        /// Core method: saves the notification to the DB, then immediately
        /// pushes via SignalR if the user is online.
        /// </summary>
        private async Task SaveAndPushAsync(
            string email,
            NotificationDto dto,
            object dataObject)
        {
            // 1. Resolve the real UserId from email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return;

            // 2. Persist to DB (IsDelivered = false by default)
            var entity = new UserNotification
            {
                UserId = user.Id,
                Type = dto.Type,
                Severity = dto.Severity,
                Title = dto.Title ?? string.Empty,
                Message = dto.Message,
                ActionUrl = dto.ActionUrl,
                RequiresAction = dto.RequiresAction,
                DataJson = JsonSerializer.Serialize(dataObject),
                CreatedAt = DateTime.UtcNow,
                IsDelivered = false,
                IsRead = false
            };

            _db.UserNotifications.Add(entity);
            await _db.SaveChangesAsync();

            // 3. Push via SignalR if user is currently online
            var connectionId = NotificationHub.GetConnectionIdByUserId(user.Id);
            if (!string.IsNullOrEmpty(connectionId))
            {
                // give the entity its real Id for the DTO
                dto.Id = entity.Id.ToString();

                await _hubContext.Clients.Client(connectionId)
                    .SendAsync("Notification", dto);

                // 4. Mark delivered since push succeeded
                entity.IsDelivered = true;
                await _db.SaveChangesAsync();
            }
            // if offline: entity stays IsDelivered=false
            // DeliverPendingNotificationsAsync will handle it on next connect
        }

        /// <summary>Rebuilds a NotificationDto from a persisted entity.</summary>
        private static NotificationDto ToDto(UserNotification n) => new()
        {
            Id = n.Id.ToString(),
            Type = n.Type,
            Severity = n.Severity,
            Title = n.Title,
            Message = n.Message,
            ActionUrl = n.ActionUrl,
            RequiresAction = n.RequiresAction,
            Timestamp = n.CreatedAt,
            // Data is stored as raw JSON — send as-is, frontend deserializes
            Data = n.DataJson != null
                ? JsonSerializer.Deserialize<object>(n.DataJson)
                : null
        };
    }
}