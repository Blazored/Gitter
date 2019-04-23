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

        protected override void OnInit()
        {
            base.OnInit();
            State.GotChatUser = async () => await FetchRooms();
        }

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
                await Invoke(StateHasChanged);
                await Task.Delay(1);
                return;
            }
            if (remember)
            {
                await State.SaveApiKey();
            }
            ErrorMessage = "";
            await Invoke(StateHasChanged);
            await Task.Delay(1);
            if (!(GitterApi is object))
            {
                ErrorMessage = "You don't appear to have an instance if IChatApi configured in startup.";
                await Invoke(StateHasChanged);
                await Task.Delay(1);
                return;
            }
        }

        async Task FetchRooms()
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
            await Invoke(StateHasChanged);
            await Task.Delay(1);
        }

        internal string RoomButtonClass(IChatRoom room)
        {
            return new BlazorComponentUtilities.CssBuilder()
                        .AddClass("btn")
                        .AddClass("btn-secondary", room.UnreadItems == 0 && room.Mentions == 0)
                        .AddClass("btn-success", room.UnreadItems > 0 && room.Mentions == 0)
                        .AddClass("btn-warning", room.Mentions > 0)
                        .AddClass("ml-1")
                        .AddClass("mb-1")
                        .Build();
        }

    }
}
