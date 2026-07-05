namespace Application.SignalR.DTOs
{
    /// <summary>
    /// Unified notification DTO sent to frontend via SignalR
    /// All notifications follow this shape for consistent frontend handling
    /// </summary>
    public class NotificationDto
    {
        /// <summary>
        /// Unique notification identifier
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Type of notification (e.g., "ORDER_STATUS", "APPLICATION_STATUS", "PAYMENT_FAILED")
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Severity level: "success", "error", "warning", "info"
        /// </summary>
        public string Severity { get; set; } = "info";

        /// <summary>
        /// User-friendly message to display
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Title for the notification (optional)
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Type-specific metadata and context data
        /// Structure varies based on notification type
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// UTC timestamp when notification was created
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Whether the notification requires user action
        /// </summary>
        public bool RequiresAction { get; set; } = false;

        /// <summary>
        /// Optional action URL or identifier for the notification
        /// </summary>
        public string? ActionUrl { get; set; }
    }
}
