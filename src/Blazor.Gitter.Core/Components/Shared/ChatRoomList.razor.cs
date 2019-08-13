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
        protected override void OnInitialized()
        {
            ChatRoomTimer = new Timer(CHATROOMUPDATETIME) { AutoReset = false };
            ChatRoomTimer.Elapsed += ChatRoomTimer_Elapsed;
            State.GotChatRooms += RefreshRooms;
            if (State.HasChatUser)
            {
                Task.Factory.StartNew(FetchRooms);
            }
            else
            {
                State.GotChatUser += State_GotChatUser;
            }
        }

        protected void ToggleMenu()
        {
            State.ToggleMenu();
        }

        private void State_GotChatUser(object sender, EventArgs e)
        {
            Invoke(StateHasChanged);
            State.GotChatUser -= State_GotChatUser;
            Task.Factory.StartNew(FetchRooms);
        }

        private void ChatRoomTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Factory.StartNew(FetchRooms);
        }

        async Task FetchRooms()
        {
            try
            {
                await Invoke(StateHasChanged);
                State.SetMyRooms((await GitterApi.GetChatRooms(State.GetMyUser().Id)).ToList());
            }
            catch 
            {
            }
        }

        private void RefreshRooms(object sender, EventArgs e)
        {
            Invoke(StateHasChanged);
            ChatRoomTimer.Start();
        }

        public void Dispose()
        {
            State.GotChatRooms -= RefreshRooms;
            ChatRoomTimer.Elapsed -= ChatRoomTimer_Elapsed;
            ChatRoomTimer?.Stop();
            ChatRoomTimer?.Dispose();
        }
    }
}
