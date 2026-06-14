namespace Application.Chat.DTOs
{
    // ---------- outbound ----------

    public record MessageDto(
        long Id,
        int ChatRoomId,
        string SenderId,
        string SenderName,
        string Content,
        DateTime SentAt,
        bool IsFromAdmin
    );

    public record ChatRoomDto(
        int Id,
        string CustomerId,
        string CustomerName,
        string PersonalPicUrl,
        DateTime CreatedAt
    );

    public record ChatHistoryDto(
        ChatRoomDto Room,
        IReadOnlyList<MessageDto> Messages
    );

    // ---------- inbound ----------

    /// <summary>Sent by either role over SignalR or REST to post a new message.</summary>
    public record SendMessageRequest(
        string Content
    );

    /// <summary>
    /// Used internally by the Service — enriched after auth resolves the sender.
    /// </summary>
    public record SendMessageCommand(
        int ChatRoomId,
        string SenderId,
        string Content
    );
}
