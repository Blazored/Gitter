﻿using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class RoomSearchBase : ComponentBase
    {
        [Inject] IChatApi GitterApi { get; set; }
        [Inject] IAppState State { get; set; }

        [Parameter] public IChatRoom ChatRoom { get; set; }
        [Parameter] public string UserId { get; set; }

        internal List<IChatMessage> SearchResult;
        internal string SearchText;

        protected bool Searching;
        CancellationTokenSource tokenSource;

        internal async Task Search(EventArgs args)
        {
            tokenSource = new CancellationTokenSource();
            try
            {
                await PerformSearch(tokenSource.Token);
            }
            catch 
            {
            }
        }

        async Task PerformSearch(CancellationToken token)
        {
            Searching = true;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(1);
            var options = GitterApi.GetNewOptions();
            options.Query = SearchText;
            options.Limit = 100;
            SearchResult = new List<IChatMessage>();
            var messages = await GitterApi.SearchChatMessages(ChatRoom.Id, options);

            while ((messages?.Any() ?? false) && !token.IsCancellationRequested)
            {
                SearchResult?.AddRange(messages.OrderBy(m => m.Sent).Reverse());
                await InvokeAsync(StateHasChanged);
                await Task.Delay(1000);
                options.Skip += messages.Count();
                messages = await GitterApi.SearchChatMessages(ChatRoom.Id, options);
            }

            Searching = false;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(1);

        }

        internal Task ClearSearch(MouseEventArgs args)
        {
            Searching = false;
            SearchResult = null;
            SearchText = string.Empty;
            return Task.CompletedTask;
        }

        internal Task CancelSearch(MouseEventArgs args)
        {
            tokenSource.Cancel();
            Searching = false;
            return Task.CompletedTask;
        }

        internal void CloseSearchMenu()
        {
            State.ToggleSearchMenu();
        }
    }
}
