using Blazor.Gitter.Core.Components.Shared;
using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Pages
{
    public class RoomModel : ComponentBase
    {
        [Inject] IUriHelper UriHelper { get; set; }
        [Inject] IChatApi GitterApi { get; set; }
        [Inject] internal IAppState State { get; set; }

        [Parameter] protected string RoomId { get; set; }

        internal IChatRoom ThisRoom;
        internal bool FirstLoad = true;

        string LastRoom = string.Empty;
        internal bool KeepWatching = true;

        protected override void OnAfterRender()
        {
            if (CheckStateForRedirect())
            {
                UriHelper.NavigateTo("/");
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (!LastRoom.Equals(RoomId))
            {
                FirstLoad = true;
                LastRoom = RoomId;
                if (GitterApi is object)
                {
                    ThisRoom = (State.GetMyRooms()).Where(r => r.Id == RoomId).FirstOrDefault();
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
