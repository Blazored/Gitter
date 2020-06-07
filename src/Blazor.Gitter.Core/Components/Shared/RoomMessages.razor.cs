using Blazor.Gitter.Core.Browser;
using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class RoomMessagesBase : ComponentBase, IDisposable
    {
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] IChatApi GitterApi { get; set; }
        [Inject] ILocalisationHelper Localisation { get; set; }
        [Inject] IAppState State { get; set; }
        [Inject] ILogger<RoomMessagesBase> Log { get; set; }

        [Parameter] public IChatRoom ChatRoom { get; set; }
        [Parameter] public string UserId { get; set; }

        internal bool LoadingMessages;
        internal IChatMessageFilter MessageFilter = new GitterMessageFilter();

        internal List<IChatMessage> Messages;
        SemaphoreSlim ssScroll = new SemaphoreSlim(1, 1);
        SemaphoreSlim ssFetch = new SemaphoreSlim(1, 1);
        bool IsFetchingOlder = false;
        bool NoMoreOldMessages = false;
        bool FirstLoad = true;
        internal bool Paused = false;
        CancellationTokenSource tokenSource;
        System.Timers.Timer RoomWatcher;
        IChatRoom LastRoom;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            State.ActivityTimeout += ActivityTimeout;
            State.ActivityResumed += ActivityResumed;
            State.GotMessageUpdate += GotMessageUpdate;
            State.GotMessageFilter += GotMessageFilter;
        }

        private void GotMessageFilter(object sender, IChatMessageFilter filter)
        {
            MessageFilter = filter;
        }

        private void GotMessageUpdate(object sender, IChatMessage message)
        {
            if (ssFetch.Wait(-1,tokenSource.Token))
            {
                try
                {
                    var oldmessage = Messages.Find(m => m.Id == message.Id);
                    if (oldmessage is IChatMessage)
                    {
                        Messages.Remove(oldmessage);
                        Messages.Add(message);
                    }
                }
                finally
                {
                    ssFetch.Release();   
                }
            }
        }

        private void ActivityResumed(object sender, EventArgs e)
        {
            StartRoomWatcher();
        }

        private void ActivityTimeout(object sender, EventArgs e)
        {
            try
            {
                RoomWatcher?.Stop();
                Paused = true;
                InvokeAsync(StateHasChanged);
            }
            catch 
            {
            }
        }

        protected override async Task OnAfterRenderAsync(bool FirstRender)
        {
            await base.OnAfterRenderAsync(FirstRender);
            if (FirstLoad && Messages?.Count > 0)
            {
                FirstLoad = false;
                State.RecordActivity();
                await JSRuntime.ScrollIntoView(GetLastMessageId());
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (!ChatRoom.Equals(LastRoom))
            {
                LoadingMessages = true;

                LastRoom = ChatRoom;
                RoomWatcher?.Stop();
                NoMoreOldMessages = false;
                IsFetchingOlder = false;
                Messages = new List<IChatMessage>();
                StartRoomWatcher();

                LoadingMessages = false;
                FirstLoad = true;
            }
        }

        private void StartRoomWatcher()
        {
            if (!(RoomWatcher is object))
            {
                tokenSource = new CancellationTokenSource();
                RoomWatcher = new System.Timers.Timer(250);
                RoomWatcher.Elapsed += async (s, e) => await MonitorNewMessages();
            }
            RoomWatcher.Interval = 250;
            RoomWatcher.Start();
            Paused = false;
            InvokeAsync(StateHasChanged);
            Task.Delay(1);
        }

        internal async Task MessagesScrolled(EventArgs args)
        {
            if (!NoMoreOldMessages && Messages.Any())
            {
                if (await ssScroll.WaitAsync(0))
                {
                    try
                    {
                        State.RecordActivity();
                        var scroll = await JSRuntime.GetScrollTop("blgmessagelist");
                        if (scroll < 100)
                        {

                            var count = await FetchOldMessages(tokenSource.Token);
                            if (count == 0)
                            {
                                NoMoreOldMessages = true;
                            }
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        ssScroll.Release();
                    }
                }
            }
        }

        async Task MonitorNewMessages()
        {
            RoomWatcher.Stop();
            if (RoomWatcher.Interval == 250)
            {
                RoomWatcher.Interval = 2000;
            }
            var options = GitterApi.GetNewOptions();
            options.Lang = Localisation.LocalCultureInfo.Name;
            options.AfterId = "";

            //bool bottom = false;
            //try
            //{
            //    bottom = await JSRuntime.IsScrolledToBottom("blgmessagelist");
            //}
            //catch 
            //{
            //}

            if (Messages?.Any() ?? false)
            {
                options.AfterId = GetLastMessageId();
            }
            await FetchNewMessages(options, tokenSource.Token);

            //if (Messages?.Any() ?? false)
            //{
            //    if (bottom)
            //    {
            //        _ = await JSRuntime.ScrollIntoView(GetLastMessageId());
            //    }
            //}
            RoomWatcher?.Start();
        }

        async Task<int> FetchNewMessages(IChatMessageOptions options, CancellationToken token)
        {
            IEnumerable<IChatMessage> messages = null;
            int count = 0;
            if (!token.IsCancellationRequested)
            {
                await ssFetch.WaitAsync(token);
                try
                {
                    messages = await GitterApi.GetChatMessages(ChatRoom.Id, options);
                    if (messages is object)
                    {
                        count = messages.Count();
                        if (!string.IsNullOrWhiteSpace(options.BeforeId))
                        {
                            Messages.InsertRange(0, RemoveDuplicates(Messages, messages));
                        }
                        else
                        {
                            Messages.AddRange(RemoveDuplicates(Messages, messages));
                        }
                        
                        await InvokeAsync(StateHasChanged);
                        await Task.Delay(1);
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError(ex, "Failed to fetch new room messages");
                }
                finally
                {
                    ssFetch.Release();
                }
            }
            return count;

            IEnumerable<IChatMessage> RemoveDuplicates(IEnumerable<IChatMessage> Existing, IEnumerable<IChatMessage> Merging)
            {
                IEnumerable<string> ExistingIds = Existing.Select(m => m.Id);
                return Merging.Where(m => !ExistingIds.Contains(m.Id));
            }
        }

        async Task<int> FetchOldMessages(CancellationToken token)
        {
            var options = GitterApi.GetNewOptions();
            options.Lang = Localisation.LocalCultureInfo.Name;
            options.AfterId = "";
            if (Messages?.Any() ?? false)
            {
                options.BeforeId = GetFirstMessageId();
                if (!token.IsCancellationRequested)
                {
                    var count = await FetchNewMessages(options, token);
                    await InvokeAsync(StateHasChanged);
                    //await Task.Delay(100);
                    //_ = await JSRuntime.ScrollIntoView(options.BeforeId);
                    return count;
                }
            }
            token.ThrowIfCancellationRequested();
            return 0;
        }

        private string GetFirstMessageId()
        {
            return Messages.OrderBy(m => m.Sent).First().Id;
        }

        private string GetLastMessageId()
        {
            return Messages?.OrderBy(m => m.Sent).Last()?.Id ?? "";
        }
        public void Dispose()
        {
            State.ActivityTimeout -= ActivityTimeout;
            State.ActivityResumed -= ActivityResumed;
            RoomWatcher?.Stop();
            RoomWatcher?.Dispose();
            Messages = null;
            RoomWatcher = null;
        }
    }
}
