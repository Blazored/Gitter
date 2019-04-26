using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class NavbarBase : ComponentBase
    {
        [Inject] IJSRuntime jSRuntime { get; set; }
        [Inject] internal IAppState State { get; set; }

        internal string Mode => jSRuntime is IJSInProcessRuntime ? "WASM" : "Server";

    }
}
