using Blazor.Gitter.Core.Components.Shared;
using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Pages
{
    public class IndexModel : ComponentBase, IDisposable
    {
        [Inject] IChatApi GitterApi { get; set; }
        [Inject] internal IAppState State { get; set; }

        internal string ErrorMessage;
        string apiKey;

        internal string ApiKey
        {
            get => State.GetApiKey();
            set => apiKey = value;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            State.GotChatUser += State_GotChatUser;
        }

        private void State_GotChatUser(object sender, EventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

        internal async Task SignIn(bool remember)
        {
            if (!State.HasApiKey && string.IsNullOrWhiteSpace(apiKey))
            {
                ErrorMessage = "Please Enter your own Gitter API Key!";
                return;
            }

            State.SetApiKey(apiKey);

            if (remember)
            {
                await State.SaveApiKey();
            }

            ErrorMessage = "";
            StateHasChanged();
        }

        public void Dispose()
        {
            State.GotChatUser -= State_GotChatUser;
        }
    }
}
