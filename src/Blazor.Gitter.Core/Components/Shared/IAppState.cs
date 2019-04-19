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
        bool HasApiKey { get; }
        bool HasChatRooms { get; }
        bool HasChatUser { get; }
        bool Initialised { get; }
        string GetApiKey();
        List<IChatRoom> GetMyRooms();
        IChatUser GetMyUser();
        Task Initialise();
        Task SaveApiKey();
        void SetApiKey(string value);
        void SetMyRooms(List<IChatRoom> value);
        void SetMyUser(IChatUser value);
    }
}