namespace Application.SignalR.DTOs
{
    /// <summary>
    /// Order notification metadata - included in NotificationDto.Data for order notifications
    /// </summary>
    public class OrderNotificationData
    {
        /// <summary>
        /// Order identifier
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Current order status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Order total amount
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Order subtotal (before delivery)
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Delivery cost
        /// </summary>
        public decimal DeliveryPrice { get; set; }

        /// <summary>
        /// Number of items in the order
        /// </summary>
        public int ItemsCount { get; set; }

        /// <summary>
        /// Delivery method name
        /// </summary>
        public string DeliveryMethod { get; set; }

        /// <summary>
        /// Estimated delivery time
        /// </summary>
        public string DeliveryTime { get; set; }

        /// <summary>
        /// Error message if payment failed (optional)
        /// </summary>
        public string? ErrorDetails { get; set; }
    }
}
