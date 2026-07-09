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

        private static readonly ConcurrentDictionary<string, string> _connections = new();

        public NotificationHub(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // Pull the real Id straight from the "uid" claim
        private string? UserId => Context.User?.FindFirst("uid")?.Value;

        public override async Task OnConnectedAsync()
        {
            var userId = UserId
                ?? throw new HubException("User Id not found in token");

            _connections[userId] = Context.ConnectionId;

            await _notificationService.DeliverPendingNotificationsAsync(userId);

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = UserId;
            if (userId != null)
                _connections.TryRemove(userId, out _);

            return base.OnDisconnectedAsync(exception);
        }

        public static string? GetConnectionIdByUserId(string userId)
            => _connections.TryGetValue(userId, out var id) ? id : null;
    }
}