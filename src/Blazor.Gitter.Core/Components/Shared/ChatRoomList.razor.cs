using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class ChatRoomListBase : ComponentBase
    {
        [Inject] internal IAppState State { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (State is object)
            {
                State.GotChatRooms = async () =>
                {
                    Console.WriteLine("Updating Menu");
                    await Invoke(StateHasChanged);
                    await Task.Delay(1);
                };
            }
        }
    }
}
