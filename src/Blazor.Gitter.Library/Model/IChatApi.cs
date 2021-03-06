﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazor.Gitter.Library
{
    public interface IChatApi
    {
        Task<IChatUser> GetCurrentUser();
        Task<IChatUser> GetChatUser(string UserId);
        Task<IEnumerable<IChatUser>> GetChatRoomUsers(string RoomId, IRoomUserOptions Options);
        Task<IChatRoom> GetChatRoom(string UserId, string RoomId);
        Task<IEnumerable<IChatRoom>> GetChatRooms(string UserId);
        Task<IChatMessage> GetChatMessage(string RoomId, string MessageId);
        Task<IEnumerable<IChatMessage>> GetChatMessages(string RoomId, IChatMessageOptions Options);
        Task<IEnumerable<IChatMessage>> SearchChatMessages(string RoomId, IChatMessageOptions Options);
        Task<IChatMessage> SendChatMessage(string RoomId, string Message);
        Task<IChatMessage> EditChatMessage(string RoomId, string MessageId, string Message);
        Task<bool> MarkChatMessageAsRead(string UserId, string RoomId, string MessageId);
        void SetAccessToken(string Token);
        IChatMessageOptions GetNewOptions();
    }
}