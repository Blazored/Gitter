using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components.Shared
{
    public class RoomSearchBase : ComponentBase
    {
        [Inject] IChatApi GitterApi { get; set; }

        [Parameter] internal IChatRoom ChatRoom { get; set; }
        [Parameter] internal string UserId { get; set; }
        [Parameter] internal string OuterClassList { get; set; }

        private bool OuterClassListIsEmpty => string.IsNullOrWhiteSpace(OuterClassList);
        internal string OuterClass => new BlazorComponentUtilities.CssBuilder()
            .AddClass("blg-center-right", OuterClassListIsEmpty)
            .AddClass("d-flex", OuterClassListIsEmpty)
            .AddClass("flex-column", OuterClassListIsEmpty)
            .AddClass("p-1", OuterClassListIsEmpty)
            .AddClass(OuterClassList, !OuterClassListIsEmpty)
            .Build();

        internal List<IChatMessage> SearchResult;
        internal string SearchText;

        internal async Task Search(UIEventArgs args)
        {
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
        }
    }
}
