using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.Gitter.Library;

namespace Blazor.Gitter.Core.Components.Shared
{
    public interface IAppState
    {
        Action GotApiKey { get; set; }
        Action GotChatUser { get; set; }
        Action GotChatRooms { get; set; }
        event EventHandler ActivityTimeout;
        event EventHandler ActivityResumed;
        event EventHandler<DateTime> TimeoutChanged;
        bool HasApiKey { get; }
        bool HasChatRooms { get; }
        bool HasChatUser { get; }
        bool Initialised { get; }
        string GetApiKey();
        List<IChatRoom> GetMyRooms();
        IChatUser GetMyUser();
        DateTime GetTimeoutTime();
        string GetLocalTime(DateTime dateTime,string format);
        Task Initialise();
        Task SaveApiKey();
        void SetApiKey(string value);
        void SetMyRooms(List<IChatRoom> value);
        void SetMyUser(IChatUser value);
        void RecordActivity();
    }
}