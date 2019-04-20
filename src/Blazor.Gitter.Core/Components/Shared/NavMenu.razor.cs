using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class NavMenuModel : ComponentBase
    {
        [Inject] IJSRuntime jSRuntime { get; set; }
        [Inject] internal IAppState State { get; set; }

        internal string Title => jSRuntime is IJSInProcessRuntime ? "Blazor Gitter - WASM" : "Blazor Gitter - Server";

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (State is object)
            {
                State.GotChatRooms = async () =>
                {
                    Console.WriteLine("Updating Menu");
                    await Invoke(StateHasChanged);
                    await Task.Delay(1);
                };
            }
        }

    }
}
