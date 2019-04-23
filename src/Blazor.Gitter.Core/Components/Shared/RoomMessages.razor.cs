using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
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

        [Parameter] protected IChatRoom ChatRoom { get; set; }
        [Parameter] internal string UserId { get; set; }
        [Parameter] internal string OuterClassList { get; set; }

        private bool OuterClassListIsEmpty => string.IsNullOrWhiteSpace(OuterClassList);
        internal string OuterClass => new BlazorComponentUtilities.CssBuilder()
            .AddClass("blg-center", OuterClassListIsEmpty)
            .AddClass("scrollable", OuterClassListIsEmpty)
            .AddClass(OuterClassList, !OuterClassListIsEmpty)
            .Build();

        internal List<IChatMessage> Messages;
        SemaphoreSlim ssScroll = new SemaphoreSlim(1, 1);
        SemaphoreSlim ssFetch = new SemaphoreSlim(1, 1);
        bool IsFetchingOlder = false;
        bool NoMoreOldMessages = false;
        bool FirstLoad = true;
        CancellationTokenSource tokenSource;
        System.Timers.Timer RoomWatcher;
        IChatRoom LastRoom;

        protected override async Task OnAfterRenderAsync()
        {
            if (FirstLoad && Messages?.Count > 0)
            {
                FirstLoad = false;
                await JSRuntime.InvokeAsync<object>("eval", $"document.getElementById('{GetLastMessageId()}').scrollIntoView()");
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            if (!ChatRoom.Equals(LastRoom))
            {
                LastRoom = ChatRoom;
                RoomWatcher?.Stop();
                NoMoreOldMessages = false;
                IsFetchingOlder = false;
                Console.WriteLine("Loading room...");
                Messages = new List<IChatMessage>();
                await Invoke(StateHasChanged);
                await Task.Delay(1);

                if (!(RoomWatcher is object))
                {
                    tokenSource = new CancellationTokenSource();
                    RoomWatcher = new System.Timers.Timer(10);
                    RoomWatcher.Elapsed += async (s, e) => await MonitorNewMessages();
                }
                RoomWatcher.Interval = 10;
                RoomWatcher.Start();
            }
        }

        internal async Task MessagesScrolled(UIEventArgs args)
        {
            if (!NoMoreOldMessages && !IsFetchingOlder && Messages.Any())
            {
                await ssScroll.WaitAsync();
                try
                {

                    var scroll = await JSRuntime.InvokeAsync<double>("eval", $"document.getElementById('blgmessagelist').scrollTop");
                    if (scroll < 100)
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

        async Task MonitorNewMessages()
        {
            RoomWatcher.Stop();
            if (RoomWatcher.Interval == 10)
            {
                RoomWatcher.Interval = 2000;
            }
            var options = GitterApi.GetNewOptions();
            options.Lang = Localisation.LocalCultureInfo.Name;
            options.AfterId = "";
            if (Messages?.Any() ?? false)
            {
                options.AfterId = GetLastMessageId();
            }
            await FetchNewMessages(options, tokenSource.Token);
            RoomWatcher.Start();
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
        public void Dispose()
        {
            RoomWatcher?.Stop();
            RoomWatcher?.Dispose();
            Messages = null;
        }
    }
}
