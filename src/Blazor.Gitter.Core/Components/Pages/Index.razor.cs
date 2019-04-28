using Blazor.Gitter.Core.Components.Shared;
using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Pages
{
    public class IndexModel : ComponentBase
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

        protected override void OnInit()
        {
            base.OnInit();
            State.GotChatUser += () => StateHasChanged();
        }

        internal async Task SignIn(bool remember)
        {
            Console.WriteLine($"Index: SignIn {(remember ? "remember" : "")}");

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
    }
}
