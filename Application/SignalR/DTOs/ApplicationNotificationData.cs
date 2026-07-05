namespace Application.SignalR.DTOs
{
    /// <summary>
    /// Adoption application notification metadata - included in NotificationDto.Data for application notifications
    /// </summary>
    public class ApplicationNotificationData
    {
        /// <summary>
        /// Adoption application identifier
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// Application status (Accepted, Rejected, Pending, etc.)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Name of the animal in the application
        /// </summary>
        public string AnimalName { get; set; }

        /// <summary>
        /// Admin notes or rejection reason
        /// </summary>
        public string? AdminNotes { get; set; }

        /// <summary>
        /// Whether the user should take action
        /// </summary>
        public bool RequiresUserAction { get; set; }
    }
}
