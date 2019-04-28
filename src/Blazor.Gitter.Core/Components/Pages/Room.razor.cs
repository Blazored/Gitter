using Blazor.Gitter.Core.Components.Shared;
using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Pages
{
    public class RoomModel : ComponentBase
    {
        [Inject] IUriHelper UriHelper { get; set; }
        [Inject] internal IAppState State { get; set; }

        [Parameter] protected string RoomId { get; set; }

        internal IChatRoom ThisRoom;
        internal bool FirstLoad = true;

        string LastRoom = string.Empty;
        internal bool KeepWatching = true;

        protected bool IsLoading = true ;

        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();
            if (!State.HasChatRooms)
            {
                State.GotChatRooms += State_GotChatRooms;
            }
            else
            {
                LoadRoom();
            }
        }

        private void State_GotChatRooms()
        {
            LoadRoom();
            State.GotChatRooms -= State_GotChatRooms;
        }

        protected override void OnAfterRender()
        {
            if (CheckStateForRedirect())
            {
                UriHelper.NavigateTo("/");
            }
        }

        protected override void OnParametersSet()
        {
            if (!LastRoom.Equals(RoomId))
            {
                FirstLoad = true;
                LastRoom = RoomId;
            }
        }

        private void LoadRoom()
        {
            Console.WriteLine($"Loading Room {IsLoading}");
            ThisRoom = State.GetRoom(RoomId);
            IsLoading = false;
            Console.WriteLine($"Loading Room Complete {IsLoading}");
            StateHasChanged();
        }

        bool CheckStateForRedirect()
        {
            if (string.IsNullOrWhiteSpace(RoomId))
            {
                return true;
            }
            if (!(State is object)) 
            {
                return true;
            }
            return false;
        }
    }
}
