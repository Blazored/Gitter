using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using System;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class RoomSendBase : ComponentBase
    {
        [Inject] IChatApi GitterApi { get; set; }
        [Inject] IAppState State { get; set; }

        [Parameter] internal IChatRoom ChatRoom { get; set; }
        [Parameter] internal IChatUser User { get; set; }


        internal string NewMessage;
        internal string NewMessageStyle;
        internal void SendMessage(UIEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(NewMessage))
            {
                GitterApi.SendChatMessage(ChatRoom.Id, NewMessage);
                NewMessage = "";
            }
            return;
        }

        internal void HandleSizing(UIChangeEventArgs args)
        {
            State.RecordActivity();
            
            string value = args.Value.ToString();
            var lines = Math.Max(value.Split('\n').Length, value.Split('\r').Length);
            NewMessageStyle = lines > 1 ? $"--box-height: {2 + ((lines - 1) * 1.48)}rem;" : "";
        }
    }
}
