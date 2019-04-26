using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class NavMenuModel : ComponentBase, IDisposable
    {
        [Inject] IJSRuntime jSRuntime { get; set; }
        [Inject] internal IAppState State { get; set; }
        [Inject] IChatApi GitterApi { get; set; }

        internal string Title => jSRuntime is IJSInProcessRuntime ? "Blazor Gitter - WASM" : "Blazor Gitter - Server";
        private Timer ChatRoomTimer;
        private const int CHATROOMUPDATETIME = 10000;

        protected override void OnInit()
        {
            base.OnInit();
            ChatRoomTimer = new Timer(CHATROOMUPDATETIME) { AutoReset = false };
            ChatRoomTimer.Elapsed += ChatRoomTimer_Elapsed;
        }

        private void ChatRoomTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Factory.StartNew(FetchRooms);
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (State is object)
            {
                State.GotChatRooms = async () =>
                {
                    await Invoke(StateHasChanged);
                    await Task.Delay(1);
                    ChatRoomTimer.Start();
                };
            }
        }

        async Task FetchRooms()
        {
            try
            {
                State.SetMyRooms((await GitterApi.GetChatRooms(State.GetMyUser().Id)).ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Dispose()
        {
            ChatRoomTimer?.Stop();
            ChatRoomTimer?.Dispose();
        }
    }
}
