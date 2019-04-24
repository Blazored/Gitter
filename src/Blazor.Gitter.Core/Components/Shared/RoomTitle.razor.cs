using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class RoomTitleBase : ComponentBase
    {
        [Parameter] internal IChatRoom ChatRoom { get; set; }
        [Parameter] internal string OuterClassList { get; set; }

        private bool OuterClassListIsEmpty => string.IsNullOrWhiteSpace(OuterClassList);
        internal string OuterClass => new BlazorComponentUtilities.CssBuilder()
            .AddClass("blg-top-center",OuterClassListIsEmpty)
            .AddClass("d-flex", OuterClassListIsEmpty)
            .AddClass("justify-content-start", OuterClassListIsEmpty)
            .AddClass("align-items-center", OuterClassListIsEmpty)
            .AddClass("text-muted",OuterClassListIsEmpty)
            .AddClass(OuterClassList,!OuterClassListIsEmpty)
            .Build();
    }
}
