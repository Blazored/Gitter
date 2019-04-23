using Blazor.Gitter.Core.Components.Shared;
using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Pages
{
    public class RoomModel : ComponentBase, IDisposable
    {
        [Inject] IUriHelper UriHelper { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] IChatApi GitterApi { get; set; }
        [Inject] ILocalisationHelper Localisation { get; set; }
        [Inject] internal IAppState State { get; set; }

        [Parameter] protected string RoomId { get; set; }

        internal List<IChatMessage> Messages;
        internal string NewMessage;
        internal IChatRoom ThisRoom;
        internal List<IChatMessage> SearchResult;
        internal string SearchText;

        CancellationTokenSource tokenSource;
        Task RoomWatcher;
        string LastRoom = string.Empty;
        static SemaphoreSlim ssFetch = new SemaphoreSlim(1, 1);
        static SemaphoreSlim ssScroll = new SemaphoreSlim(1, 1);
        bool KeepWatching = false;
        bool FirstLoad = true;
        bool IsFetchingOlder = false;
        bool NoMoreOldMessages = false;

        protected override async Task OnAfterRenderAsync()
        {
            if (await CheckStateForRedirect())
            {
                UriHelper.NavigateTo("/");
            }
            if (FirstLoad && Messages?.Count > 0)
            {
                FirstLoad = false;
                await JSRuntime.InvokeAsync<object>("eval", $"document.getElementById('{GetLastMessageId()}').scrollIntoView()");
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (!LastRoom.Equals(RoomId))
            {
                FirstLoad = true;
                LastRoom = RoomId;
                NoMoreOldMessages = false;
                IsFetchingOlder = false;
            }
            if (GitterApi is object)
            {
                ThisRoom = (State.GetMyRooms()).Where(r => r.Id == RoomId).FirstOrDefault();
                Console.WriteLine("Loading room...");
                Messages = new List<IChatMessage>();
                await Invoke(StateHasChanged);
                await Task.Delay(1);

                if (!(RoomWatcher is object))
                {
                    KeepWatching = true;
                    tokenSource = new CancellationTokenSource();
                    RoomWatcher = MonitorNewMessages(tokenSource.Token);
                }
            }
        }

        internal string LocalTime(DateTime dateTime) =>
                TimeZoneInfo
                    .ConvertTime(
                        dateTime,
                        Localisation.LocalTimeZoneInfo
                    )
                    .ToString(
                        "G",
                        Localisation.LocalCultureInfo
                    );

        async Task<bool> CheckStateForRedirect()
        {
            if (string.IsNullOrWhiteSpace(RoomId))
            {
                return true;
            }
            if (!(State is object)) // || !(GitterApi is object))
            {
                return true;
            }
            if (!State.HasChatRooms)
            {
                State.SetMyRooms((await GitterApi.GetChatRooms(State.GetMyUser().Id)).ToList());
            }
            if (!State.HasChatRooms)
            {
                return true;
            }
            return false;
        }

        internal void SendMessage(UIEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(NewMessage))
            {
                GitterApi.SendChatMessage(RoomId, NewMessage);
                NewMessage = "";
            }
            return;
        }

        async Task MonitorNewMessages(CancellationToken token)
        {
            var options = GitterApi.GetNewOptions();
            options.Lang = Localisation.LocalCultureInfo.Name;
            while (!token.IsCancellationRequested && KeepWatching)
            {
                options.AfterId = "";
                if (Messages?.Any() ?? false)
                {
                    options.AfterId = GetLastMessageId();
                }
                await FetchNewMessages(options, token);
                await Task.Delay(2000, token);
            }
            token.ThrowIfCancellationRequested();
        }

        async Task<int> FetchNewMessages(IChatMessageOptions options, CancellationToken token)
        {
            List<IChatMessage> messages = null;
            int count = 0;
            if (!token.IsCancellationRequested)
            {
                await ssFetch.WaitAsync(token);
                try
                {
                    messages = (await GitterApi.GetChatMessages(RoomId, options)).ToList();
                    if (messages is object)
                    {
                        count = messages.Count;
                        if (!string.IsNullOrWhiteSpace(options.BeforeId))
                        {
                            Messages.InsertRange(0, messages);
                        }
                        else
                        {
                            Messages.AddRange(messages);
                        }
                        await Invoke(StateHasChanged);
                        await Task.Delay(1);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    ssFetch.Release();
                }
            }
            return count;
        }

        async Task<int> FetchOldMessages(CancellationToken token)
        {
            var options = GitterApi.GetNewOptions();
            options.Lang = Localisation.LocalCultureInfo.Name;
            if (!token.IsCancellationRequested && IsFetchingOlder)
            {
                options.AfterId = "";
                if (Messages?.Any() ?? false)
                {
                    options.BeforeId = GetFirstMessageId();
                    var count = await FetchNewMessages(options, token);
                    await Invoke(StateHasChanged);
                    await Task.Delay(100);
                    await JSRuntime.InvokeAsync<object>("eval", $"document.getElementById('{options.BeforeId}').scrollIntoView()");
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

        internal async Task MessagesScrolled(UIEventArgs args)
        {
            if (!NoMoreOldMessages && !IsFetchingOlder && Messages.Any())
            {
                await ssScroll.WaitAsync();
                try
                {

                    var scroll = await JSRuntime.InvokeAsync<double>("eval", $"document.getElementById('blgmessagelist').scrollTop");
                    if (scroll < 200)
                    {
                        IsFetchingOlder = true;

                        var count = await FetchOldMessages(tokenSource.Token);
                        if (count == 0)
                        {
                            NoMoreOldMessages = true;
                        }
                        IsFetchingOlder = false;
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

        internal async Task Search(UIEventArgs args)
        {
            var options = GitterApi.GetNewOptions();
            options.Query = SearchText;
            options.Limit = 100;
            SearchResult = new List<IChatMessage>();
            var messages = (await GitterApi.SearchChatMessages(RoomId, options)).ToList();
            while (messages?.Any() ?? false)
            {
                SearchResult.AddRange(messages.OrderBy(m => m.Sent).Reverse());
                Console.WriteLine($"Got {messages.Count} results. First is {SearchResult.First().Id} Last is {SearchResult.Last().Id}");
                await Invoke(StateHasChanged);
                await Task.Delay(2000);
                options.Skip += messages.Count;
                messages = (await GitterApi.SearchChatMessages(RoomId, options)).ToList();
            }
        }

        public void Dispose()
        {
            if (tokenSource is object && tokenSource.Token.CanBeCanceled)
            {
                try
                {
                    tokenSource.Cancel();
                    if (RoomWatcher is object)
                    {
                        var _ = RoomWatcher?.Wait(2000);
                    }
                    tokenSource.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            NoMoreOldMessages = false;
            IsFetchingOlder = false;
        }

    }
}
