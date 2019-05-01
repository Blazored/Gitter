using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class NavbarBase : ComponentBase, IDisposable
    {
        [Inject] IJSRuntime jSRuntime { get; set; }
        [Inject] internal IAppState State { get; set; }

        internal string Mode => jSRuntime is IJSInProcessRuntime ? "WASM" : "Server";

        protected override void OnInit()
        {
            State.GotChatUser += State_GotChatUser;
        }

        private void State_GotChatUser(object sender, System.EventArgs e)
        {
            Invoke(StateHasChanged);
        }

        public void Dispose()
        {
            State.GotChatUser -= State_GotChatUser;
        }
    }
}
