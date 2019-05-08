using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class RoomSendBase : ComponentBase, IDisposable
    {
        [Inject] IChatApi GitterApi { get; set; }
        [Inject] ICdnApi CDNApi { get; set; }
        [Inject] IAppState State { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }

        [Parameter] internal IChatRoom ChatRoom { get; set; }
        [Parameter] internal IChatUser User { get; set; }

        private const string BaseClass = "chat-room__send-message";
        private const string BaseFormClass = "chat-room__send-message-form";
        internal string NewMessage;
        internal string NewMessageClass = BaseClass;
        internal string FormClass = BaseFormClass;

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
                    AppendToNewMessageWithNewLines($"> {message.Text}\r\r@{message.FromUser.Username} ");
                    break;
                case ChatMessageQuoteType.Reply:
                    AppendToNewMessage($"@{message.FromUser.Username}");
                    break;
                default:
                    break;
            }
            BuildClassString(NewMessage);
            Invoke(StateHasChanged);
            Task.Delay(1);
        }

        void AppendToNewMessage(string value)
        {
            NewMessage = $"{NewMessage} {value} ".TrimStart();
        }
        void AppendToNewMessageWithNewLines(string value)
        {
            if (string.IsNullOrWhiteSpace(NewMessage))
            {
                NewMessage = value;
            }
            else if (NewMessage.EndsWith("\r") || NewMessage.EndsWith("\n"))
            {
                NewMessage = $"{NewMessage}\r{value}";
            }
            else
            {
                NewMessage = $"{NewMessage}\r\r{value}";
            }
        }
        internal async Task OnPasteFile(UIChangeEventArgs args)
        {
            string file = args.Value.ToString();
            FormClass = $"{BaseFormClass}--uploading";
            StateHasChanged();
            await SendToCdn(file);
        }

        private async Task SendToCdn(string file)
        {
            var image = await CDNApi.UploadAsync(file);
            if (image is ICdnImage)
            {
                if (!string.IsNullOrWhiteSpace(image.url))
                {
                    ICdnImage sThumb = null;
                    if (image.transformations is object)
                    {
                        sThumb = image.transformations.Where(e => e.transformation == "t_media_lib_thumb").First();
                    }
                    AppendToNewMessageWithNewLines($"[![{image.name}]({sThumb?.secure_url ?? image.secure_url})]({image.secure_url})");
                }
            }
            FormClass = BaseFormClass;
        }

        [JSInvokable]
        public async Task SendImageData(string data)
        {
            await SendToCdn(data);
        }

        public void Dispose()
        {
            State.GotMessageToQuote -= QuoteMessage;
        }
    }
}
