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

            State.GotApiKey += State_GotApiKey;
            await State.Initialise();

            AssemblyList = new List<System.Reflection.Assembly>()
                {
                    typeof(ILocalStorageService).Assembly,
                    typeof(MainLayout).Assembly
                };
        }

        private void State_GotApiKey()
        {
            Task.Factory.StartNew(async () => await AttemptLogin());
        }

        private async Task AttemptLogin()
        {
            var apiKey = State.GetApiKey();

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Console.WriteLine($"ML:No API key available, login required");
                return;
            }

            if (State.HasChatUser)
            {
                Console.WriteLine($"ML:Already logged in");
                State.TriggerLoggedIn();
                return;
            }

            Console.WriteLine($"ML:Signing in using {apiKey}");
            GitterApi.SetAccessToken(apiKey);
            State.SetMyUser(await GitterApi.GetCurrentUser());

        }
    }
}
