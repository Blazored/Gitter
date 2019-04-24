using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class RoomSendBase : ComponentBase
    {
        [Inject] IChatApi GitterApi { get; set; }

        [Parameter] internal IChatRoom ChatRoom { get; set; }
        [Parameter] internal IChatUser User { get; set; }
        [Parameter] internal string OuterClassList { get; set; }

        private bool OuterClassListIsEmpty => string.IsNullOrWhiteSpace(OuterClassList);
        internal string OuterClass => new BlazorComponentUtilities.CssBuilder()
            .AddClass("blg-bottom-center", OuterClassListIsEmpty)
            .AddClass("d-flex", OuterClassListIsEmpty)
            .AddClass("align-items-center", OuterClassListIsEmpty)
            .AddClass(OuterClassList, !OuterClassListIsEmpty)
            .Build();

        internal string NewMessage;
        internal void SendMessage(UIEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(NewMessage))
            {
                GitterApi.SendChatMessage(ChatRoom.Id, NewMessage);
                NewMessage = "";
            }
            return;
        }

    }
}
