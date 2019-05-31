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
        protected bool SearchIsOpen;
        protected string SearchCss = "";

        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();
            State.SearchMenuToggled += State_ToggleSearchMenu;
            if (!State.HasChatRooms)
            {
                State.GotChatRooms += State_GotChatRooms;
            }
            else
            {
                LoadRoom();
            }
        }

        private void State_GotChatRooms(object sender, EventArgs e)
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
                if (State.HasChatRooms)
                {
                    LoadRoom();
                }
            }
        }

        protected void ToggleSearchMenu()
        {
            State.ToggleSearchMenu();
        }

        protected void State_ToggleSearchMenu(object sender, EventArgs e)
        {
            SearchIsOpen = !SearchIsOpen;
            SearchCss = SearchIsOpen ? "chat-room__search--active" : "";
            StateHasChanged();
            Console.WriteLine("Search Opened");
        }

        private void LoadRoom()
        {
            ThisRoom = State.GetRoom(RoomId);
            IsLoading = false;
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
