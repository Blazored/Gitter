using Microsoft.AspNetCore.Components;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class ChatRoomListBase : ComponentBase
    {
        [Inject] internal IAppState State { get; set; }

        protected bool HasRooms;

        protected override void OnInit()
        {
            if (!State.HasChatRooms)
            {
                State.GotChatRooms += RefreshRooms;
            }
            else
            {
                HasRooms = true;
            }
        }

        private void RefreshRooms()
        {
            HasRooms = true;
            StateHasChanged();
        }
    }
}
