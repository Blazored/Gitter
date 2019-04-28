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
    public class MainLayoutModel : LayoutComponentBase
    {
        [Inject] protected IAppState State { get; set; }
        [Inject] IChatApi GitterApi { get; set; }

        internal List<System.Reflection.Assembly> AssemblyList;

        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();

            await State.Initialise();
            await AttemptLogin();

            AssemblyList = new List<System.Reflection.Assembly>()
                {
                    typeof(ILocalStorageService).Assembly,
                    typeof(MainLayout).Assembly
                };
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
