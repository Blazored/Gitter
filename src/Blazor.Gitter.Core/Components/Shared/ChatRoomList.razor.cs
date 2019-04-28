using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class ChatRoomListBase : ComponentBase, IDisposable
    {
        [Inject] internal IAppState State { get; set; }
        [Inject] IChatApi GitterApi { get; set; }
        public Action GetRooms { get; private set; }

        private Timer ChatRoomTimer;
        private const int CHATROOMUPDATETIME = 10000;
        protected override void OnInit()
        {
            State.GotChatUser += async () => await FetchRooms();
            State.GotChatRooms += RefreshRooms;
            ChatRoomTimer = new Timer(CHATROOMUPDATETIME) { AutoReset = false };
            ChatRoomTimer.Elapsed += ChatRoomTimer_Elapsed;
        }

        private void ChatRoomTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Factory.StartNew(FetchRooms);
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

        private void RefreshRooms()
        {
            StateHasChanged();
            ChatRoomTimer.Start();
        }

        public void Dispose()
        {
            ChatRoomTimer?.Stop();
            ChatRoomTimer?.Dispose();
        }
    }
}
