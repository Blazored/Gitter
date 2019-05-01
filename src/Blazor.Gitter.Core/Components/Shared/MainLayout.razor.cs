using Blazor.Gitter.Library;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class MainLayoutModel : LayoutComponentBase, IDisposable
    {
        [Inject] protected IAppState State { get; set; }
        [Inject] IChatApi GitterApi { get; set; }


        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();

            State.GotApiKey += State_GotApiKey;
            State.GotChatUser += State_GotChatUser;
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
