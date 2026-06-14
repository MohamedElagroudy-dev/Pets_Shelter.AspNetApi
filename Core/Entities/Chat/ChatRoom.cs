namespace Core.Entities.Chat
{
    public class ChatRoom
    {
        public int Id { get; set; }

        // FK ? AppUser (Customer only)
        public string CustomerId { get; set; } = default!;
        public AppUser Customer { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
