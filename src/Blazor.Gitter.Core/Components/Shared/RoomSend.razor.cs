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

        protected override void OnInit()
        {
            base.OnInit();
            State.GotMessageToQuote += QuoteMessage;
        }

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

        void QuoteMessage(IChatMessage message)
        {
            Console.WriteLine($"Send:Quoting {message.FromUser.DisplayName}");
            if (string.IsNullOrWhiteSpace(NewMessage))
            {
                NewMessage = $"> {message.Text}\r\r@{message.FromUser.Username} ";
            } else if (NewMessage.EndsWith("\r") || NewMessage.EndsWith("\n"))
            {
                NewMessage = $"{NewMessage}\r> {message.Text}\r\r@{message.FromUser.Username} ";
            } else
            {
                NewMessage = $"{NewMessage}\r\r> {message.Text}\r\r@{message.FromUser.Username} ";
            }
            StateHasChanged();
        }
    }
}
