﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.Gitter.Library;

namespace Blazor.Gitter.Core.Components.Shared
{
    public interface IAppState
    {
        event EventHandler GotApiKey;
        event EventHandler GotChatUser;
        event EventHandler GotChatRooms;
        event EventHandler<IChatMessageFilter> GotMessageFilter;
        event EventHandler OnChange;
        event EventHandler<ChatMessageEventArgs> GotMessageToQuote;
        event EventHandler ActivityTimeout;
        event EventHandler ActivityResumed;
        event EventHandler<DateTime> TimeoutChanged;
        event EventHandler<IChatMessage> GotMessageUpdate;
        event EventHandler MenuToggled;
        event EventHandler SearchMenuToggled;
        event EventHandler RoomUserSearchCancelled;
        event EventHandler<IEnumerable<IChatUser>> RoomUserSearchPerformed;
        bool HasApiKey { get; }
        bool HasChatRooms { get; }
        bool HasChatUser { get; }
        bool Initialised { get; }
        void ToggleMenu();
        void ToggleSearchMenu();
        void CancelRoomUserSearch();
        void ShowRoomUserSearchResults(IEnumerable<IChatUser> results);
        string GetApiKey();
        List<IChatRoom> GetMyRooms();
        IChatRoom GetRoom(string RoomId);
        IChatUser GetMyUser();
        DateTime GetTimeoutTime();
        string GetLocalTime(DateTime dateTime,string format);
        Task Initialise();
        Task SaveApiKey();
        void SetApiKey(string value);
        void SetMyRooms(List<IChatRoom> value);
        void SetMyUser(IChatUser value);
        void SetMessageFilter(IChatMessageFilter value);
        void RecordActivity();
        void QuoteMessage(IChatMessage message);
        void ReplyMessage(IChatMessage message);
        void UpdateMessage(IChatMessage message);
    }
}