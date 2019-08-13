using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Layouts;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class MainLayoutModel : LayoutComponentBase, IDisposable
    {
        [Inject] protected IAppState State { get; set; }
        [Inject] IChatApi GitterApi { get; set; }
        [Inject] IJSRuntime jSRuntime { get; set; }

        internal string Mode => jSRuntime is IJSInProcessRuntime ? "WASM" : "Server";
        internal string Build => this.GetType().Assembly.GetName().Version.ToString();

        protected bool MenuIsOpen;
        protected string MenuCss { get; set; }


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            State.GotApiKey += State_GotApiKey;
            State.GotChatUser += State_GotChatUser;
            State.MenuToggled += State_MenuToggled;
        }

        private void State_MenuToggled(object sender, EventArgs e)
        {
            MenuIsOpen = !MenuIsOpen;
            MenuCss = MenuIsOpen ? "wrapper__left-menu--active" : "";
            StateHasChanged();
        }

        private void State_GotChatUser(object sender, EventArgs e)
        {
            Invoke(StateHasChanged);
        }

        private void State_GotApiKey(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () => await AttemptLogin());
        }

        private async Task AttemptLogin()
        {
            var apiKey = State.GetApiKey();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return;
            }

            if (State.HasChatUser)
            {
                return;
            }

            GitterApi.SetAccessToken(apiKey);
            State.SetMyUser(await GitterApi.GetCurrentUser());

        }

        public void Dispose()
        {
            State.GotApiKey -= State_GotApiKey;
            State.GotChatUser -= State_GotChatUser;
        }
    }
}
