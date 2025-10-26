using Application.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace API.SignalR
{

    [Authorize]
    public class NotificationHub(IUserContext _userContext) : Hub
    {
        private static readonly ConcurrentDictionary<string, string> UserConnections = new();

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
