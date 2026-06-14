using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Chat.DTOs;
using Application.Chat.Interfaces;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class ChatController : ControllerBase
    {
        private readonly IChatService _chat;

        public ChatController(IChatService chat) => _chat = chat;
        private string GetCurrentUserId()
        {
            var userId = User.FindFirst("uid")?.Value
                         ?? throw new InvalidOperationException("User Id not found in token");

            return userId;
        }

        // ── GET /api/chat/room ───────────────────────────────────────────────────
        /// <summary>
        /// Customer: get (or create) their own room.
        /// Returns room metadata. Used on page load before SignalR connects.
        /// </summary>
        [HttpGet("room")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<ChatRoomDto>> GetMyRoom(CancellationToken ct)
        {
            var UserId = GetCurrentUserId();
            var room = await _chat.GetOrCreateRoomAsync(UserId, ct);
            return Ok(room);
        }

        // ── POST /api/chat/rooms/by-customer/{customerId} (Admin only) ────────────
        /// <summary>
        /// Admin: create (or get) a room for a given customer id.
        /// </summary>
        [HttpPost("rooms/by-customer/{customerId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ChatRoomDto>> GetOrCreateRoomForCustomer(string customerId, CancellationToken ct)
        {
            var room = await _chat.GetOrCreateRoomAsync(customerId, ct);
            return Ok(room);
        }

        // ── GET /api/chat/rooms ──────────────────────────────────────────────────
        /// <summary>
        /// Admin: list all rooms that have at least one message (conversation list).
        /// </summary>
        [HttpGet("rooms")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<ChatRoomDto>>> GetAllRooms(
            CancellationToken ct)
        {
            var rooms = await _chat.GetAllRoomsAsync(ct);
            return Ok(rooms);
        }

        // ── GET /api/chat/rooms/{id}/history ─────────────────────────────────────
        /// <summary>
        /// Load full message history for a room.
        /// Customer can only read their own room; Admin can read any room.
        /// </summary>
        [HttpGet("rooms/{id:int}/history")]
        public async Task<ActionResult<ChatHistoryDto>> GetHistory(
            int id, CancellationToken ct)
        {
            var UserId = GetCurrentUserId();
            var history = await _chat.GetHistoryAsync(id, ct);

            // Authorization: customer must own this room
            if (!IsAdmin && history.Room.CustomerId != UserId)
                return Forbid();

            return Ok(history);
        }

        // ── POST /api/chat/rooms/{id}/messages ───────────────────────────────────
        /// <summary>
        /// REST fallback for sending a message (use SignalR in production).
        /// Kept here for testing / Swagger convenience.
        /// </summary>
        [HttpPost("rooms/{id:int}/messages")]
        public async Task<ActionResult<MessageDto>> SendMessage(
            int id, [FromBody] SendMessageRequest request, CancellationToken ct)
        {
            var UserId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Content is required.");

            // Customer can only post to their own room
            if (!IsAdmin)
            {
                var roomId = await _chat.GetRoomIdForCustomerAsync(UserId, ct);
                if (roomId != id) return Forbid();
            }

            var cmd = new SendMessageCommand(id, UserId, request.Content);
            var message = await _chat.SaveMessageAsync(cmd, ct);
            return Ok(message);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private string UserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        private bool IsAdmin =>
            User.IsInRole("Admin");
    }
}
