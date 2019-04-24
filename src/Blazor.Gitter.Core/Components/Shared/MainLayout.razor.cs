using Blazor.Gitter.Library;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Layouts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class MainLayoutModel : LayoutComponentBase
    {
        [Inject] IChatApi GitterApi { get; set; }
        [Inject] ILocalStorageService LocalStorage { get; set; }
        [Inject] IAppState SharedAppState { get; set; }

        internal List<System.Reflection.Assembly> AssemblyList;

        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();
            SharedAppState.GotApiKey = async () => await SignIn();
            AssemblyList = new List<System.Reflection.Assembly>()
                {
                    typeof(ILocalStorageService).Assembly
                };
        }

        async Task SignIn()
        {
            if (!(GitterApi is object))
            {
                Console.WriteLine("MainLayout: No Gitter Api");
                return;
            }
            if (!SharedAppState.HasChatUser)
            {
                Console.WriteLine($"MainLayout signing in using {SharedAppState.GetApiKey()}");
                GitterApi.SetAccessToken(SharedAppState.GetApiKey());
                SharedAppState.SetMyUser(await GitterApi.GetCurrentUser());
                await Invoke(StateHasChanged);
                await Task.Delay(1);
            }
        }

    }
}
