using Blazor.Gitter.Core.Browser;
using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class RoomSendBase : ComponentBase, IDisposable
    {
        [Inject] IChatApi GitterApi { get; set; }
        [Inject] IAppState State { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }

        [Parameter] internal IChatRoom ChatRoom { get; set; }
        [Parameter] internal IChatUser User { get; set; }
        internal string NewMessage
        {
            get => newMessage;
            set
            {
                newMessage = value; CalculateSize(value);
            }
        }

        private const string BaseClass = "chat-room__send-message";
        private string newMessage;
        internal string NewMessageClass = BaseClass;
        internal string OkButtonId = "message-send-button";
        internal string MessageInputId = "message-send-input";
        internal int Rows = 2;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            State.GotMessageToQuote += QuoteMessage;
        }

        internal async Task SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(NewMessage))
            {
                await GitterApi.SendChatMessage(ChatRoom.Id, NewMessage);
                NewMessage = "";
            }
        }

        internal void HandleSizing(UIChangeEventArgs args)
        {
            State.RecordActivity();

            string value = args.Value.ToString();
            CalculateSize(value);
        }

        private void CalculateSize(string value)
        {
            Rows = Math.Max(value.Split('\n').Length, value.Split('\r').Length);
            Rows = Math.Min(Rows, 10);
            Rows = Math.Max(Rows, 2);
        }

        void QuoteMessage(object sender, ChatMessageEventArgs e)
        {
            IChatMessage message = e.ChatMessage;
            var quotedMessage = "> " + message.Text.Replace(Environment.NewLine, $"{Environment.NewLine}> ");
            switch (e.QuoteType)
            {
                case ChatMessageQuoteType.Quote:
                    if (string.IsNullOrWhiteSpace(NewMessage))
                    {
                        NewMessage = $"{quotedMessage}\r\r@{message.FromUser.Username} ";
                    }
                    else if (NewMessage.EndsWith("\r") || NewMessage.EndsWith("\n"))
                    {
                        NewMessage = $"{NewMessage}\r{quotedMessage}\r\r@{message.FromUser.Username} ";
                    }
                    else
                    {
                        NewMessage = $"{NewMessage}\r\r{quotedMessage}\r\r@{message.FromUser.Username} ";
                    }
                    break;
                case ChatMessageQuoteType.Reply:
                    NewMessage = $"{NewMessage} @{message.FromUser.Username} ".TrimStart();
                    break;
                default:
                    break;
            }
            CalculateSize(NewMessage);
            InvokeAsync(StateHasChanged);
            Task.Delay(1);
        }

        internal async Task Shortcuts(UIKeyboardEventArgs args)
        {
            if (args.CtrlKey)
            {
                switch (args.Key)
                {
                    case "Enter":
                    case "Return":
                        await SendMessage();
                        break;
                    default:
                        break;
                }
            }
            return;
        }
        public void Dispose()
        {
            State.GotMessageToQuote -= QuoteMessage;
        }
    }
}
