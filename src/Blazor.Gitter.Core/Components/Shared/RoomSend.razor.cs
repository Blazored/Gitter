using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class RoomSendBase : ComponentBase, IDisposable
    {
        [Inject] IChatApi GitterApi { get; set; }
        [Inject] IAppState State { get; set; }

        [Parameter] internal IChatRoom ChatRoom { get; set; }
        [Parameter] internal IChatUser User { get; set; }

        private const string BaseClass = "chat-room__send-message";
        internal string NewMessage;
        internal string NewMessageClass = BaseClass;
        internal bool FilterUnreadActive = false;

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

        internal Task FilterUnread()
        {
            switch (FilterUnreadActive)
            {
                case false:
                    State.SetMessageFilter(new GitterMessageFilter() { FilterUnread = true });
                    FilterUnreadActive = true;
                    break;
                case true:
                    State.SetMessageFilter(new GitterMessageFilter() { FilterUnread = false });
                    FilterUnreadActive = false;
                    break;
            }
            return Task.CompletedTask;
        }

        internal void HandleSizing(UIChangeEventArgs args)
        {
            State.RecordActivity();

            string value = args.Value.ToString();
            BuildClassString(value);
        }

        private void BuildClassString(string value)
        {
            var lines = Math.Max(value.Split('\n').Length, value.Split('\r').Length);
            lines = Math.Min(lines, 10);
            NewMessageClass = new BlazorComponentUtilities.CssBuilder()
                .AddClass(BaseClass, lines <= 1)
                .AddClass($"{BaseClass}--sh-{lines}", lines > 1)
                .Build();
        }

        void QuoteMessage(object sender, ChatMessageEventArgs e)
        {
            IChatMessage message = e.ChatMessage;
            switch (e.QuoteType)
            {
                case ChatMessageQuoteType.Quote:
                    if (string.IsNullOrWhiteSpace(NewMessage))
                    {
                        NewMessage = $"> {message.Text}\r\r@{message.FromUser.Username} ";
                    }
                    else if (NewMessage.EndsWith("\r") || NewMessage.EndsWith("\n"))
                    {
                        NewMessage = $"{NewMessage}\r> {message.Text}\r\r@{message.FromUser.Username} ";
                    }
                    else
                    {
                        NewMessage = $"{NewMessage}\r\r> {message.Text}\r\r@{message.FromUser.Username} ";
                    }
                    break;
                case ChatMessageQuoteType.Reply:
                    NewMessage = $"{NewMessage} @{message.FromUser.Username} ".TrimStart();
                    break;
                default:
                    break;
            }
            BuildClassString(NewMessage);
            Invoke(StateHasChanged);
            Task.Delay(1);
        }

        public void Dispose()
        {
            State.GotMessageToQuote -= QuoteMessage;
        }
    }
}
