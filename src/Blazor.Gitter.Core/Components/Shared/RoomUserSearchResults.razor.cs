using Blazor.Gitter.Core.Browser;
using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace Blazor.Gitter.Core.Components.Shared
{
    public class RoomUserSearchResultsBase : ComponentBase
    {
        [Inject] IAppState State { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }

        protected IEnumerable<IChatUser> Results { get; set; } = Enumerable.Empty<IChatUser>();

        protected bool IsVisible { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            State.RoomUserSearchCancelled += SearchCancelled;
            State.RoomUserSearchPerformed += SearchPerformed;
        }

        private async void SearchCancelled(object sender, EventArgs e)
        {
            IsVisible = false;

            await InvokeAsync(StateHasChanged);
        }

        private async void SearchPerformed(object sender, IEnumerable<IChatUser> results)
        {
            Results = results;

            IsVisible = true;

            await InvokeAsync(StateHasChanged);

            await BrowserInterop.RepositionRoomSearchResults(JSRuntime);
        }
    }
}
