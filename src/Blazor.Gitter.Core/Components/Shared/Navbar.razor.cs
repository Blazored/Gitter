using Microsoft.AspNetCore.Components;
using System;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class NavbarBase : ComponentBase, IDisposable
    { 
        [Inject] internal IAppState State { get; set; }

        protected override void OnInit()
        {
            State.GotChatUser += State_GotChatUser;
        }

        protected void ToggleMenu()
        {
            State.ToggleMenu();
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
