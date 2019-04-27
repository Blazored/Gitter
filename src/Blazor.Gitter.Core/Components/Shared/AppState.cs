using Blazor.Gitter.Library;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class AppState : IAppState
    {
        private readonly IJSRuntime JSRuntime;
        private readonly ILocalStorageService LocalStorage;
        private readonly IComponentContext ComponentContext;
        private readonly ILocalisationHelper LocalisationHelper;
        private readonly IChatApi GitterApi;
        private string apiKey;
        private IChatUser myUser;
        private List<IChatRoom> myRooms;
        private bool initialised;
        private DateTime TimeoutTime;
        private Timer TimeoutTimer;
        private const int TIMEOUT = 30;

        /// <summary>
        /// Attach to this to be notified when there is an ApiKey available
        /// </summary>
        public Action GotApiKey { get; set; }
        /// <summary>
        /// Attach to this to be notified when there is a ChatUser available
        /// </summary>
        public event Action GotChatUser;
        /// <summary>
        /// Attach to this to be notified when there are ChatRooms available
        /// </summary>
        public event Action GotChatRooms;

        public event Action OnChange;

        public AppState(IJSRuntime jSRuntime,
            ILocalStorageService localStorage,
            IComponentContext componentContext,
            ILocalisationHelper localisationHelper,
            IChatApi gitterApi)
        {
            JSRuntime = jSRuntime;
            LocalStorage = localStorage;
            ComponentContext = componentContext;
            LocalisationHelper = localisationHelper;
            GitterApi = gitterApi;
            //Task.Factory.StartNew(Initialise);
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
                        await LocalisationHelper.BuildLocalCulture();
                        await LocalisationHelper.BuildLocalTimeZone();
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

        public void TriggerLoggedIn()
        {
            GotChatUser?.Invoke();
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
            Console.WriteLine($"Setting ChatUser {value.DisplayName} - {value.Username} - there is {(GotChatUser != null ? "" : "not")} a subscriber");
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

        public string GetLocalTime(DateTime dateTime, string format = "G")
        {
            return TimeZoneInfo
                .ConvertTime(
                    dateTime,
                    LocalisationHelper.LocalTimeZoneInfo
                )
                .ToString(
                    format,
                    LocalisationHelper.LocalCultureInfo
                );
        }
        public DateTime GetTimeoutTime() => TimeoutTime;
        public event EventHandler ActivityTimeout;
        public event EventHandler ActivityResumed;
        public event EventHandler<DateTime> TimeoutChanged;

        private void SetTimeoutTime()
        {
            if (!(TimeoutTimer is object))
            {
                TimeoutTimer = new Timer() { AutoReset = false, Interval = new TimeSpan(0, TIMEOUT, 0).TotalMilliseconds };
                TimeoutTimer.Elapsed += TimeoutTimer_Elapsed;
            }
            if (TimeoutTimer.Enabled)
            {
                TimeoutTimer.Stop();
            }
            else
            {
                ActivityResumed?.Invoke(this, null);
            }
            TimeoutTime = DateTime.UtcNow.AddMinutes(TIMEOUT);
            TimeoutTimer.Start();
            TimeoutChanged?.Invoke(this, TimeoutTime);
        }

        private void TimeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ActivityTimeout?.Invoke(this, null);
        }

        public void RecordActivity()
        {
            SetTimeoutTime();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
