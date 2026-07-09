// Application/SignalR/NotificationHub.cs
using System.Collections.Concurrent;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Application.SignalR
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly INotificationService _notificationService;

        // ConnectionId keyed by UserId (not email — more reliable)
        private static readonly ConcurrentDictionary<string, string> _connections = new();

        public NotificationHub(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier
                ?? throw new HubException("Unauthenticated");

            _connections[userId] = Context.ConnectionId;

            // Deliver everything the user missed while offline
            await _notificationService.DeliverPendingNotificationsAsync(userId);

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
                _connections.TryRemove(userId, out _);

            return base.OnDisconnectedAsync(exception);
        }

        // Called by NotificationService — looks up by UserId now
        public static string? GetConnectionIdByUserId(string userId)
            => _connections.TryGetValue(userId, out var id) ? id : null;

        // Keep backward-compatible email lookup if other code still uses it
        // (you can remove this once you update all callers to use UserId)
        [Obsolete("Use GetConnectionIdByUserId instead")]
        public static string? GetConnectionIdByEmail(string email) => null;
    }
}