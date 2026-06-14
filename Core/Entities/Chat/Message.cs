namespace Core.Entities.Chat
{
    public class Message
    {
        public long Id { get; set; }

        public int ChatRoomId { get; set; }
        public ChatRoom ChatRoom { get; set; } = default!;

        // FK ? AppUser (either Customer or Admin)
        public string SenderId { get; set; } = default!;
        public AppUser Sender { get; set; } = default!;

        public string Content { get; set; } = default!;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
