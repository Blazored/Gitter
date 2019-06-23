using Blazor.Gitter.Library;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class AppState : IAppState, IDisposable
    {
        private readonly ILocalStorageService LocalStorage;
        private readonly IComponentContext ComponentContext;
        private readonly ILocalisationHelper LocalisationHelper;
        private readonly IUriHelper UriHelper;
        private string apiKey;
        private IChatUser myUser;
        private List<IChatRoom> myRooms;
        private bool initialised;
        private DateTime TimeoutTime;
        private Timer TimeoutTimer;
        private Stopwatch LastTriggerTimeOutChanged;
        private const int TIMEOUT = 60;
        private const string LOGINPAGE = "/";

        /// <summary>
        /// Attach to this to be notified when there is an ApiKey available
        /// </summary>
        public event EventHandler GotApiKey;
        /// <summary>
        /// Attach to this to be notified when there is a ChatUser available
        /// </summary>
        public event EventHandler GotChatUser;
        /// <summary>
        /// Attach to this to be notified when there are ChatRooms available
        /// </summary>
        public event EventHandler GotChatRooms;
        /// <summary>
        /// Attach to this to be notified that some as yet undiscovered state has changed
        /// </summary>
        public event EventHandler OnChange;
        /// <summary>
        /// Attach to this to be notified that there is a message to quote 
        /// </summary>
        public event EventHandler<ChatMessageEventArgs> GotMessageToQuote;
        /// <summary>
        /// Attach to this to be notifies that the user has been timed out due to lack of activity
        /// </summary>
        public event EventHandler ActivityTimeout;
        /// <summary>
        /// Attach to this to be notified that the user has become active
        /// </summary>
        public event EventHandler ActivityResumed;
        /// <summary>
        /// Attach to this to be notified that the time of expected user timeout has changed
        /// </summary>
        public event EventHandler<DateTime> TimeoutChanged;
        /// <summary>
        /// Attach to this to be notified that a message has been edited
        /// </summary>
        public event EventHandler<IChatMessage> GotMessageUpdate;
        /// <summary>
        /// Attach to this to be notified that the menu has been toggled
        /// </summary>
        public event EventHandler MenuToggled;
        /// <summary>
        /// Attach to this to be notified that the search menu has been toggled
        /// </summary>
        public event EventHandler SearchMenuToggled;

        public AppState(
            ILocalStorageService localStorage,
            IComponentContext componentContext,
            ILocalisationHelper localisationHelper,
            IUriHelper uriHelper
            )
        {
            LocalStorage = localStorage;
            ComponentContext = componentContext;
            LocalisationHelper = localisationHelper;
            UriHelper = uriHelper;
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
                        await LocalisationHelper.BuildLocalCulture();
                        await LocalisationHelper.BuildLocalTimeZone();
                        if (!HasApiKey)
                        {
                            SetApiKey(await LocalStorage.GetItemAsync<string>("GitterKey"));
                        }
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (done == 1)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }

                await Task.Delay(1000);
            }
            if (HasApiKey)
            {
                initialised = true;
            }
            else
            {
                var currentUri = UriHelper.GetAbsoluteUri();
                var baseUri = UriHelper.GetBaseUri();
                var currentPage = UriHelper.ToBaseRelativePath(baseUri, currentUri);
                if (!currentPage.Equals(LOGINPAGE,StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(currentPage))
                {
                    UriHelper.NavigateTo(LOGINPAGE);
                }
            }
        }

        void RaiseGotChatUserEvent()
        {
            GotChatUser?.Invoke(this, null);
        }

        void RaiseGotApiKeyEvent()
        {
            GotApiKey?.Invoke(this, null);
        }

        void RaiseGotChatRoomsEvent()
        {
            GotChatRooms?.Invoke(this, null);
        }

        void RaiseGotMessageToQuoteEvent(IChatMessage message, ChatMessageQuoteType quoteType)
        {
            GotMessageToQuote?.Invoke(this, new ChatMessageEventArgs() { ChatMessage = message, QuoteType = quoteType });
        }

        void RaiseOnChangeEvent()
        {
            OnChange?.Invoke(this, null);
        }

        void RaiseActivityResumedEvent()
        {
            ActivityResumed?.Invoke(this, null);
        }

        void RaiseActivityTimeoutEvent()
        {
            ActivityTimeout?.Invoke(this, null);
        }

        void RaiseTimeoutChangedEvent(DateTime dateTime)
        {
            TimeoutChanged?.Invoke(this, dateTime);
        }

        private void RaiseGotMessageUpdateEvent(IChatMessage message)
        {
            GotMessageUpdate?.Invoke(this, message);
        }

        public void ToggleMenu()
        {
            MenuToggled?.Invoke(this, null);
        }
        public void ToggleSearchMenu()
        {
            SearchMenuToggled?.Invoke(this, null);
        }

        public bool HasApiKey => !string.IsNullOrWhiteSpace(apiKey);
        public string GetApiKey()
        {
            return apiKey;
        }
        public void SetApiKey(string value)
        {
            apiKey = value;
            if (HasApiKey)
            {
                initialised = true;
                RaiseGotApiKeyEvent();
            }
        }
        public async Task SaveApiKey()
        {
            if (HasApiKey)
            {
                await LocalStorage.SetItemAsync("GitterKey", apiKey);
            }
        }
        public bool HasChatUser => myUser is object;
        public IChatUser GetMyUser()
        {
            return myUser;
        }
        public void SetMyUser(IChatUser value)
        {
            myUser = value;
            if (HasChatUser)
            {
                RaiseGotChatUserEvent();
            }
        }

        public bool HasChatRooms => myRooms?.Count > 0;
        public List<IChatRoom> GetMyRooms()
        {
            return myRooms;
        }
        public void SetMyRooms(List<IChatRoom> value)
        {
            myRooms = value;
            if (HasChatRooms)
            {
                RaiseGotChatRoomsEvent();
            }
        }
        public IChatRoom GetRoom(string RoomId)
        {
            if (HasChatRooms)
            {
                return myRooms.Where(room => room.Id.Equals(RoomId)).FirstOrDefault();
            }
            else
            {
                return default;
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
                RaiseActivityResumedEvent();
            }
            TimeoutTime = DateTime.UtcNow.AddMinutes(TIMEOUT);
            TimeoutTimer.Start();
            if (!(LastTriggerTimeOutChanged is object))
            {
                LastTriggerTimeOutChanged = new Stopwatch();
            }
            if (!LastTriggerTimeOutChanged.IsRunning || LastTriggerTimeOutChanged.ElapsedMilliseconds >= 1000)
            {
                RaiseTimeoutChangedEvent(TimeoutTime);
                LastTriggerTimeOutChanged.Restart();
            }
        }

        private void TimeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RaiseActivityTimeoutEvent();
        }

        public void RecordActivity()
        {
            SetTimeoutTime();
        }

        public void NotifyStateChanged() => RaiseOnChangeEvent();

        public void QuoteMessage(IChatMessage message)
        {
            RaiseGotMessageToQuoteEvent(message, ChatMessageQuoteType.Quote);
        }

        public void ReplyMessage(IChatMessage message)
        {
            RaiseGotMessageToQuoteEvent(message, ChatMessageQuoteType.Reply);
        }

        public void UpdateMessage(IChatMessage message)
        {
            RaiseGotMessageUpdateEvent(message);
        }

        public void Dispose()
        {
            if (TimeoutTimer is object)
            {
                TimeoutTimer.Elapsed -= TimeoutTimer_Elapsed;
                TimeoutTimer.Dispose();
            }
        }
    }
}
