using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class RoomSearchBase : ComponentBase
    {
        [Inject] IChatApi GitterApi { get; set; }

        [Parameter] internal IChatRoom ChatRoom { get; set; }
        [Parameter] internal string UserId { get; set; }

        internal List<IChatMessage> SearchResult;
        internal string SearchText;

        protected bool Searching;

        internal async Task Search(UIEventArgs args)
        {
            Searching = true;

            var options = GitterApi.GetNewOptions();
            options.Query = SearchText;
            options.Limit = 100;
            SearchResult = new List<IChatMessage>();
            var messages = await GitterApi.SearchChatMessages(ChatRoom.Id, options);

            while (messages?.Any() ?? false)
            {
                SearchResult.AddRange(messages.OrderBy(m => m.Sent).Reverse());
                Console.WriteLine($"Got {messages.Count()} results. First is {SearchResult.First().Id} Last is {SearchResult.Last().Id}");
                await Invoke(StateHasChanged);
                await Task.Delay(2000);
                options.Skip += messages.Count();
                messages = await GitterApi.SearchChatMessages(ChatRoom.Id, options);
            }

            Searching = false;
        }

        internal Task ClearSearch(UIMouseEventArgs args)
        {
            SearchResult = null;
            SearchText = string.Empty;
            return Task.CompletedTask;
        }
    }
}
