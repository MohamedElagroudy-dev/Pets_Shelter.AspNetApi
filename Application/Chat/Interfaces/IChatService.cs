using Application.Chat.DTOs;

namespace Application.Chat.Interfaces
{
    public interface IChatService
    {
        /// <summary>
        /// Returns the customer's room, creating one if it does not yet exist.
        /// Call this when a Customer connects.
        /// </summary>
        Task<ChatRoomDto> GetOrCreateRoomAsync(string customerId, CancellationToken ct = default);

        /// <summary>Loads full history for a room (customer sees their own; admin sees any).</summary>
        Task<ChatHistoryDto> GetHistoryAsync(int chatRoomId, CancellationToken ct = default);

        /// <summary>
        /// Admin only — returns all rooms with at least one message so the admin
        /// can pick a conversation to open.
        /// </summary>
        Task<IReadOnlyList<ChatRoomDto>> GetAllRoomsAsync(CancellationToken ct = default);

        /// <summary>
        /// Persists the message and returns the full DTO (including generated Id / SentAt).
        /// Throws UnauthorizedAccessException if the sender is not a participant.
        /// </summary>
        Task<MessageDto> SaveMessageAsync(SendMessageCommand cmd, CancellationToken ct = default);

        /// <summary>Resolves which room a customer belongs to (throws if not found).</summary>
        Task<int> GetRoomIdForCustomerAsync(string customerId, CancellationToken ct = default);
    }
}
