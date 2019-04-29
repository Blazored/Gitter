using Blazor.Gitter.Library;
using Blazored.Localisation.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class MessageModel : ComponentBase
    {
        [Inject] IChatApi GitterApi { get; set; }
        [Inject] IAppState State { get; set; }
        [Inject] IBrowserDateTimeProvider BrowserDateTimeProvider { get; set; }
        [Parameter] internal IChatMessage MessageData { get; set; }
        [Parameter] protected string RoomId { get; set; }
        [Parameter] protected string UserId { get; set; }
        [Parameter] protected Action<IChatMessage> QuoteMessage { get; set; }

        protected IBrowserDateTime BrowserDateTime { get; set; }

        SemaphoreSlim slim = new SemaphoreSlim(1, 1);

        internal string MessageClassList(IChatMessage message) =>
            new BlazorComponentUtilities.CssBuilder()
                .AddClass("chat-room-messages__message-container")
                .AddClass("chat-room-messages__message-container--unread", message.Unread)
                .AddClass("chat-room-messages__message-container--mention", message.Mentions.Any(m => m.UserId == UserId))
                .Build();

        protected override async Task OnInitAsync()
        {
            BrowserDateTime = await BrowserDateTimeProvider.GetInstance();
        }

        internal async Task MarkRead()
        {
            if (await slim.WaitAsync(1))
            {
                try
                {
                    if (MessageData.Unread)
                    {
                        await Task.Delay(1000);
                        State.RecordActivity();
                        if (await GitterApi.MarkChatMessageAsRead(UserId, RoomId, MessageData.Id))
                        {
                            MessageData.Unread = false;
                        }
                    }
                }
                catch { }
                finally
                {
                    slim.Release();
                }
            }
        }

        internal void QuoteThis()
        {
            Console.WriteLine($"MSG:Start Quoting {MessageData.FromUser.DisplayName}");
            State.QuoteMessage(MessageData);
        }
    }
}
