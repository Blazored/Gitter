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

        internal string ApiKey
        {
            get => State.GetApiKey();
            set => State.SetApiKey(value);
        }

        internal async Task SignIn(bool remember)
        {
            Console.WriteLine($"Index: SignIn {(remember ? "remember" : "")}");

            if (!State.HasApiKey)
            {
                ErrorMessage = "Did you notice that box? It says Enter your own Gitter API Key!";
                return;
            }

            if (remember)
            {
                await State.SaveApiKey();
                await AttemptLogin();
            }

            ErrorMessage = "";
            StateHasChanged();

            if (!(GitterApi is object))
            {
                ErrorMessage = "You don't appear to have an instance if IChatApi configured in startup.";
                return;
            }
        }

        private async Task AttemptLogin()
        {
            var apiKey = State.GetApiKey();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Console.WriteLine($"No API key available, login required");
                return;
            }

            if (State.HasChatUser)
            {
                Console.WriteLine($"Already logged in");
                State.TriggerLoggedIn();
                return;
            }

            Console.WriteLine($"Signing in using {apiKey}");
            GitterApi.SetAccessToken(apiKey);
            State.SetMyUser(await GitterApi.GetCurrentUser());

            await FetchRooms();
        }

        private async Task FetchRooms()
        {
            Console.WriteLine("Getting Rooms...");
            try
            {
                State.SetMyRooms((await GitterApi.GetChatRooms(State.GetMyUser().Id)).ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine($"ROOMS:{State.GetMyRooms()?.Count()}");
        }
    }
}
