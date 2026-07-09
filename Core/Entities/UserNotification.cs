using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class UserNotification
    {
        public int Id { get; set; }

        // who it's for — FK to AspNetUsers
        public string UserId { get; set; } = default!;
        public AppUser User { get; set; } = default!;

        // mirrors NotificationDto fields exactly
        public string Type { get; set; } = default!;
        public string Severity { get; set; } = "info";
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string? ActionUrl { get; set; }
        public bool RequiresAction { get; set; }

        // Data (OrderNotificationData / ApplicationNotificationData) stored as JSON
        public string? DataJson { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // false = not yet pushed to the client over SignalR
        public bool IsDelivered { get; set; } = false;

        // false = user hasn't opened/seen it yet
        public bool IsRead { get; set; } = false;
    }
}
