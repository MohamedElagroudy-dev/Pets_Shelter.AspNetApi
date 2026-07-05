namespace Application.SignalR.DTOs
{
    /// <summary>
    /// Standardized notification types
    /// </summary>
    public static class NotificationType
    {
        // Order notifications
        public const string OrderPaymentReceived = "ORDER_PAYMENT_RECEIVED";
        public const string OrderPaymentFailed = "ORDER_PAYMENT_FAILED";
        public const string OrderStatusChanged = "ORDER_STATUS_CHANGED";
        public const string OrderShipped = "ORDER_SHIPPED";
        public const string OrderDelivered = "ORDER_DELIVERED";

        // Adoption application notifications
        public const string ApplicationAccepted = "APPLICATION_ACCEPTED";
        public const string ApplicationRejected = "APPLICATION_REJECTED";
        public const string ApplicationStatusChanged = "APPLICATION_STATUS_CHANGED";

        // General notifications
        public const string Info = "INFO";
        public const string Warning = "WARNING";
        public const string Error = "ERROR";
        public const string Success = "SUCCESS";
    }

    /// <summary>
    /// Notification severity levels
    /// </summary>
    public static class NotificationSeverity
    {
        public const string Success = "success";
        public const string Error = "error";
        public const string Warning = "warning";
        public const string Info = "info";
    }
}
