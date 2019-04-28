using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class MessageModel : ComponentBase
    {
        [Inject] ILocalisationHelper Localisation { get; set; }
        [Parameter] internal IChatMessage MessageData { get; set; }
        [Parameter] protected string UserId { get; set; }

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

    }
}
