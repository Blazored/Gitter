using Blazor.Gitter.Core.Components.Shared;
using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;

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

        protected bool IsLoading;

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
                if (State is object)
                {
                    IsLoading = true;
                    Console.WriteLine($"Loading Room {IsLoading}");
                    ThisRoom = State.GetMyRooms().Where(r => r.Id == RoomId).FirstOrDefault();
                    IsLoading = false;
                    Console.WriteLine($"Loading Room Complete {IsLoading}");
                }
            }
        }

        bool CheckStateForRedirect()
        {
            if (string.IsNullOrWhiteSpace(RoomId))
            {
                return true;
            }
            if (!(State is object)) // || !(GitterApi is object))
            {
                return true;
            }
            if (!State.HasChatRooms)
            {
                return true;
            }
            return false;
        }
    }
}
