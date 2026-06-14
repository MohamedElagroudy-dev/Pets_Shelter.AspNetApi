using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Application.Account;
using System.Collections.Concurrent;

namespace Application.SignalR
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> UserConnections = new();
        private readonly IUserContext _userContext;

        public NotificationHub(IUserContext userContext)
        {
            _userContext = userContext;
        }

        public override Task OnConnectedAsync()
        {
            var user = _userContext.GetCurrentUser();
            if (user == null)
                throw new Exception();

            if (!string.IsNullOrEmpty(user.Email))
                UserConnections[user.Email] = Context.ConnectionId;

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var user = _userContext.GetCurrentUser();
            if (user == null)
                throw new Exception();

            if (!string.IsNullOrEmpty(user.Email))
                UserConnections.TryRemove(user.Email, out _);

            return base.OnDisconnectedAsync(exception);
        }

        public static string? GetConnectionIdByEmail(string email)
        {
            UserConnections.TryGetValue(email, out var connectionId);

            return connectionId;
        }
    }
}
