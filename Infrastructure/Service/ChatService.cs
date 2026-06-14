using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Application.Chat.DTOs;
using Application.Chat.Interfaces;
using Core.Entities;
using Core.Entities.Chat;
using Infrastructure.Persistence;

namespace Infrastructure.Service
{
    public sealed class ChatService : IChatService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public ChatService(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ── GetOrCreateRoomAsync ─────────────────────────────────────────────────

        public async Task<ChatRoomDto> GetOrCreateRoomAsync(
    string customerIdOrUserName,
    CancellationToken ct = default)
        {
            // Resolve user first
            var customer = await _userManager.FindByIdAsync(customerIdOrUserName);

            if (customer is null)
                customer = await _userManager.FindByNameAsync(customerIdOrUserName);

            if (customer is null)
                customer = await _userManager.FindByEmailAsync(customerIdOrUserName);

            if (customer is null)
                throw new KeyNotFoundException(
                    $"User {customerIdOrUserName} not found.");

            // Always search by REAL UserId
            var room = await _db.ChatRooms
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(
                    r => r.CustomerId == customer.Id,
                    ct);

            if (room is null)
            {
                room = new ChatRoom
                {
                    CustomerId = customer.Id,
                    CreatedAt = DateTime.UtcNow
                };

                _db.ChatRooms.Add(room);
                await _db.SaveChangesAsync(ct);

                await _db.Entry(room)
                    .Reference(r => r.Customer)
                    .LoadAsync(ct);
            }

            return ToDto(room);
        }

        // ── GetHistoryAsync ──────────────────────────────────────────────────────

        public async Task<ChatHistoryDto> GetHistoryAsync(
            int chatRoomId, CancellationToken ct = default)
        {
            var room = await _db.ChatRooms
                .Include(r => r.Customer)
                .Include(r => r.Messages)
                    .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(r => r.Id == chatRoomId, ct)
                ?? throw new KeyNotFoundException($"ChatRoom {chatRoomId} not found.");

            var messages = room.Messages
                .OrderBy(m => m.SentAt)
                .Select(m => ToDto(m, room.CustomerId))
                .ToList();

            return new ChatHistoryDto(ToDto(room), messages);
        }

        // ── GetAllRoomsAsync ─────────────────────────────────────────────────────

        public async Task<IReadOnlyList<ChatRoomDto>> GetAllRoomsAsync(
            CancellationToken ct = default)
        {
            return await _db.ChatRooms
                .Include(r => r.Customer)
                .Where(r => r.Messages.Any())          // only rooms with messages
                .OrderByDescending(r =>
                    r.Messages.Max(m => m.SentAt))     // most recent first
                .Select(r => new ChatRoomDto(
                    r.Id,
                    r.CustomerId,
                    r.Customer.FirstName ?? r.CustomerId,
                    r.Customer.PictureUrl ?? string.Empty,
                    r.CreatedAt
                ))
                .ToListAsync(ct);
        }

        // ── SaveMessageAsync ─────────────────────────────────────────────────────

        public async Task<MessageDto> SaveMessageAsync(
            SendMessageCommand cmd, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(cmd.Content))
                throw new ArgumentException("Message content cannot be empty.");

            var room = await _db.ChatRooms
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == cmd.ChatRoomId, ct)
                ?? throw new KeyNotFoundException($"ChatRoom {cmd.ChatRoomId} not found.");

            var sender = await _userManager.FindByIdAsync(cmd.SenderId)
                ?? throw new KeyNotFoundException($"Sender {cmd.SenderId} not found.");

            // ── authorization guard ──────────────────────────────────────────────
            // Sender must be either the room's customer OR an admin.
            var roles = await _userManager.GetRolesAsync(sender);
            bool isAdmin = roles.Contains("Admin");
            bool isCustomer = room.CustomerId == cmd.SenderId;

            if (!isAdmin && !isCustomer)
                throw new UnauthorizedAccessException(
                    "You are not a participant of this chat room.");

            var message = new Message
            {
                ChatRoomId = cmd.ChatRoomId,
                SenderId = cmd.SenderId,
                Sender = sender,
                Content = cmd.Content,
                SentAt = DateTime.UtcNow
            };

            _db.Messages.Add(message);
            await _db.SaveChangesAsync(ct);

            return ToDto(message, room.CustomerId);
        }

        public async Task<int> GetRoomIdForCustomerAsync(
            string customerId, CancellationToken ct = default)
        {
            var room = await _db.ChatRooms
                .FirstOrDefaultAsync(r => r.CustomerId == customerId, ct)
                ?? throw new KeyNotFoundException(
                    $"No chat room found for customer {customerId}.");

            return room.Id;
        }

        // ── Private mappers ──────────────────────────────────────────────────────

        private static ChatRoomDto ToDto(ChatRoom r) =>
            new(r.Id, r.CustomerId, r.Customer?.FirstName ?? r.CustomerId, r.Customer?.PictureUrl ?? string.Empty, r.CreatedAt);

        /// <summary>Maps a Message entity to its DTO, resolving IsFromAdmin via the room's CustomerId.</summary>
        private static MessageDto ToDto(Message m, string roomCustomerId) =>
            new(
                m.Id,
                m.ChatRoomId,
                m.SenderId,
                m.Sender?.FirstName ?? m.SenderId,
                m.Content,
                m.SentAt,
                IsFromAdmin: m.SenderId != roomCustomerId
            );
    }
}
