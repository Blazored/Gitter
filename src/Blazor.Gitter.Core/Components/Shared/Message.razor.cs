using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class MessageModel : ComponentBase
    {
        [Inject] ILocalisationHelper Localisation { get; set; }
        [Parameter] internal IChatMessage MessageData { get; set; }
        [Parameter] protected string UserId { get; set; }

        internal string MessageClassList(IChatMessage message) =>
            new BlazorComponentUtilities.CssBuilder()
                .AddClass("list-group-item")
                .AddClass("flex-column")
                .AddClass("align-items-start")
                .AddClass("bg-inherit")
                .AddClass("text-inherit")
                .AddClass("list-group-item-success", message.Unread)
                .AddClass("list-group-item-warning", message.Mentions.Any(m => m.UserId == UserId))
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
