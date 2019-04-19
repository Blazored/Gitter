using Blazor.Gitter.Library;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class AppState : IAppState
    {
        private readonly IJSRuntime JSRuntime;
        private readonly ILocalStorageService LocalStorage;
        private readonly IComponentContext ComponentContext;
        private string apiKey;
        private IChatUser myUser;
        private List<IChatRoom> myRooms;
        private bool initialised;

        /// <summary>
        /// Attach to this to be notified when there is an ApiKey available
        /// </summary>
        public Action GotApiKey { get; set; }
        /// <summary>
        /// Attach to this to be notified when there is a ChatUser available
        /// </summary>
        public Action GotChatUser { get; set; }
        /// <summary>
        /// Attach to this to be notified when there are ChatRooms available
        /// </summary>
        public Action GotChatRooms { get; set; }

        public AppState(IJSRuntime jSRuntime, ILocalStorageService localStorage, IComponentContext componentContext)
        {
            JSRuntime = jSRuntime;
            LocalStorage = localStorage;
            ComponentContext = componentContext;
            Task.Factory.StartNew(Initialise);
        }

        public bool Initialised => initialised;
        public async Task Initialise()
        {
            int done = 10;
            while (done-- > 0)
            {
                if (ComponentContext.IsConnected)
                {
                    try
                    {
                        Console.WriteLine($"reading GitterKey from storage - {done} attempts left");
                        SetApiKey(await LocalStorage.GetItem<string>("GitterKey"));
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    Console.WriteLine("Waiting for connection...");
                }
                await Task.Delay(1000);
            }
            initialised = true;
        }

        public bool HasApiKey => !string.IsNullOrWhiteSpace(apiKey);
        public string GetApiKey()
        {
            return apiKey;
        }
        public void SetApiKey(string value)
        {
            Console.WriteLine($"Setting ApiKey = [{value}] - there is {(GotApiKey is object ? "" : "not")} a subscriber");
            apiKey = value;
            if (HasApiKey)
            {
                GotApiKey?.Invoke();
            }
        }
        public async Task SaveApiKey()
        {
            if (HasApiKey)
            {
                Console.WriteLine($"Storing ApiKey {apiKey}");
                await LocalStorage.SetItem("GitterKey", apiKey);
            }
        }
        public bool HasChatUser => myUser is object;
        public IChatUser GetMyUser()
        {
            return myUser;
        }
        public void SetMyUser(IChatUser value)
        {
            Console.WriteLine($"Setting ChatUser {value.DisplayName} - {value.Username} - there is {(GotChatUser is object ? "" : "not")} a subscriber");
            myUser = value;
            if (HasChatUser)
            {
                GotChatUser?.Invoke();
            }
        }

        public bool HasChatRooms => myRooms?.Count > 0;
        public List<IChatRoom> GetMyRooms()
        {
            return myRooms;
        }
        public void SetMyRooms(List<IChatRoom> value)
        {
            Console.WriteLine($"Setting Rooms {value?.Count} - there is {(GotChatRooms is object ? "" : "not")} a subscriber");
            myRooms = value;
            if (HasChatRooms)
            {
                GotChatRooms?.Invoke();
            }
        }
    }
}
