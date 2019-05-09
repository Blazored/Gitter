using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
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
        [Inject] ILocalisationHelper Localisation { get; set; }
        [Parameter] internal IChatMessage MessageData { get; set; }
        [Parameter] protected string RoomId { get; set; }
        [Parameter] internal string UserId { get; set; }
        [Parameter] protected Action<IChatMessage> QuoteMessage { get; set; }

        SemaphoreSlim slim = new SemaphoreSlim(1, 1);

        internal string MessageClassList(IChatMessage message) =>
            new BlazorComponentUtilities.CssBuilder()
                .AddClass("chat-room-messages__message-container")
                .AddClass("chat-room-messages__message-container--unread", message.Unread)
                .AddClass("chat-room-messages__message-container--mention", message.Mentions.Any(m => m.UserId == UserId))
                .Build();
        internal string LocalTime(DateTime dateTime) =>
        TimeZoneInfo
            .ConvertTime(
                dateTime,
                Localisation.LocalTimeZoneInfo
            )
            .ToString(
                "G",
                Localisation.LocalCultureInfo
            );
        internal bool EditMode = false;

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
            State.RecordActivity();
            State.QuoteMessage(MessageData);
        }
        internal void ReplyThis()
        {
            State.RecordActivity();
            State.ReplyMessage(MessageData);
        }
        internal void FilterThisUser()
        {
            State.SetMessageFilter(new GitterMessageFilter() { FilterByUserId = MessageData.FromUser.Id });
        }

        internal async Task EditOnChange(UIChangeEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.Value.ToString()))
            {
                EditMode = false;
                var message = await GitterApi.EditChatMessage(RoomId, MessageData.Id, args.Value.ToString());
                State.UpdateMessage(message);
            }
        }
        internal void EditThis()
        {
            State.RecordActivity();
            EditMode = true;
        }
        internal Task EditKeyPress(UIKeyboardEventArgs args)
        {
            State.RecordActivity();
            if (args.Key.Equals("escape",StringComparison.OrdinalIgnoreCase))
            {
                EditMode = false;
            }
            return Task.CompletedTask;
        }
    }
}
