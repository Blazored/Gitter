using Blazor.Gitter.Core.Browser;
using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace Blazor.Gitter.Core.Components.Shared
{
    public class RoomSendBase : ComponentBase, IDisposable
    {
        [Inject] IChatApi GitterApi { get; set; }
        [Inject] IAppState State { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }

        [Inject] Library.Services.RoomUsersRepository RoomUsersRepository { get; set; }

        [Parameter] public IChatRoom ChatRoom { get; set; }
        [Parameter] public IChatUser User { get; set; }

        private string newMessage = "";
        internal string NewMessage
        {
            get => newMessage;
            set
            {
                newMessage = value; CalculateSize(value);
            }
        }

        private readonly DebounceHelper debouncer = new DebounceHelper();
        protected async Task OnInput(ChangeEventArgs e)
        {
            NewMessage = e.Value as string ?? "";

            if (_IsShowingUsernameAutocomplete)
            {
                await debouncer.DebounceAsync(async (cancellationToken) =>
                {
                    await QueryRoomUsersAsync();
                }, delayMilliseconds: 300);
            }
        }

        private async Task QueryRoomUsersAsync()
        {
            if (!NewMessage.Contains("@"))
            {
                State.CancelRoomUserSearch();
                _IsShowingUsernameAutocomplete = false;
                return;
            }

            string query = NewMessage.Substring(_UsernameAutocompleteStartIndex);

            Console.WriteLine($"Should pop up! Will query: {query}");

            if (string.IsNullOrWhiteSpace(query))
                return;

            var chatRoomUsers = await RoomUsersRepository.QueryAsync(this.ChatRoom, query);

            if (chatRoomUsers.Any())
            {
                State.ShowRoomUserSearchResults(chatRoomUsers);
            }
            else
            {
                State.CancelRoomUserSearch();
            }

            // TODO:
            // * allow arrow up/down selection

            foreach (var item in chatRoomUsers)
            {
                Console.WriteLine(item.DisplayName);
            }
        }

        private const string BaseClass = "chat-room__send-message";
        internal string NewMessageClass = BaseClass;
        internal string OkButtonId = "message-send-button";
        internal string MessageInputId = "message-send-input";
        internal int Rows = 2;
        internal bool FilterUnreadActive = false;

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

        internal void HandleSizing(ChangeEventArgs args)
        {
            State.RecordActivity();

            string value = args.Value.ToString();
            CalculateSize(value);
        }

        private void CalculateSize(string value)
        {
            Rows = Math.Max(value.Split('\n').Length, value.Split('\r').Length);
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

        private bool _IsShowingUsernameAutocomplete;
        private int _UsernameAutocompleteStartIndex;

        internal async Task Shortcuts(KeyboardEventArgs args)
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
            else
            {
                switch (args.Key)
                {
                    case "@":
                        var selectionStart = await BrowserInterop.GetSelectionStart(JSRuntime, MessageInputId);

                        Console.WriteLine($"Selection start: {selectionStart}");

                        // the input field only contains '@', or there's whitespace next to our caret
                        // note that NewMessage represents the state _before_ oninput
                        if (selectionStart < 0 ||
                            NewMessage.Length == 0 ||
                            (selectionStart == 0 && char.IsWhiteSpace(NewMessage[selectionStart + 1])) ||
                            (selectionStart == NewMessage.Length && char.IsWhiteSpace(NewMessage[selectionStart - 1])))
                        {
                            _IsShowingUsernameAutocomplete = true;
                            _UsernameAutocompleteStartIndex = selectionStart + 1;
                        }

                        break;

                    case "Escape":
                        _IsShowingUsernameAutocomplete = false;
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
