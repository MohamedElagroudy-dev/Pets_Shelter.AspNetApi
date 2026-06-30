using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Application.Chat.DTOs;
using Application.Chat.Interfaces;

namespace Application.SignalR
{
    [Authorize]
    public sealed class ChatHub : Hub
    {
        private readonly IChatService _chat;

        public ChatHub(IChatService chat)
        {
            _chat = chat;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            var isAdmin = Context.User!.IsInRole("Admin");

            if (isAdmin)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "all-admins");   

                var rooms = await _chat.GetAllRoomsAsync();
                foreach (var room in rooms)
                    await Groups.AddToGroupAsync(Context.ConnectionId, RoomGroup(room.Id));

                Context.Items["IsAdmin"] = true;
            }
            else
            {
                var room = await _chat.GetOrCreateRoomAsync(userId);
                await Groups.AddToGroupAsync(Context.ConnectionId, RoomGroup(room.Id));

                Context.Items["RoomId"] = room.Id;
                Context.Items["IsAdmin"] = false;
            }

            await base.OnConnectedAsync();
        }
        /// <summary>
        /// Admin only — starts (or opens) a conversation with a customer.
        /// Creates the room if it doesn't exist, joins the admin's connection
        /// to that room's group, and returns the room info.
        /// </summary>
        public async Task<ChatRoomDto> StartConversation(string customerIdOrUserName)
        {
            var isAdmin = (bool)Context.Items["IsAdmin"]!;
            if (!isAdmin)
                throw new HubException("Only admins can start a conversation.");

            var room = await _chat.GetOrCreateRoomAsync(customerIdOrUserName);

            await Groups.AddToGroupAsync(Context.ConnectionId, RoomGroup(room.Id));

            return room;
        }

        public async Task SendMessage(int chatRoomId, string content)
        {
            var userId = GetUserId();
            var isAdmin = (bool)Context.Items["IsAdmin"]!;

            int resolvedRoomId;

            if (isAdmin)
            {
                if (chatRoomId <= 0)
                    throw new HubException("Admin must specify a chatRoomId.");

                resolvedRoomId = chatRoomId;
            }
            else
            {
                resolvedRoomId = (int)Context.Items["RoomId"]!;
            }

            var cmd = new Application.Chat.DTOs.SendMessageCommand(resolvedRoomId, userId, content);
            var message = await _chat.SaveMessageAsync(cmd);

            if (!isAdmin)
            {
                await Clients.Group("all-admins").SendAsync("JoinRoomSilently", resolvedRoomId);
            }

            await Clients
                .Group(RoomGroup(resolvedRoomId))
                .SendAsync("ReceiveMessage", message);

            if (isAdmin)
                await Groups.AddToGroupAsync(Context.ConnectionId, RoomGroup(resolvedRoomId));
        }
        public Task JoinRoom(int roomId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, RoomGroup(roomId));
        }
        private string GetUserId() =>
            Context.UserIdentifier
            ?? throw new HubException("Unauthenticated connection.");

        private static string RoomGroup(int roomId) => $"room-{roomId}";
    }
}
