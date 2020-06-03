using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Browser
{
    static class BrowserInterop
    {
        public static ValueTask<int> GetSelectionStart(this IJSRuntime JSRuntime, string id)
        {
            return JSRuntime.InvokeAsync<int>("chat.getSelectionStart", id);
        }

        public static ValueTask<double> GetScrollTop(this IJSRuntime JSRuntime, string id)
        {
            return JSRuntime.InvokeAsync<double>("chat.getScrollTop", id);
        }
        public static ValueTask<bool> IsScrolledToBottom(this IJSRuntime JSRuntime, string id)
        {
            try
            {
                return JSRuntime.InvokeAsync<bool>("chat.isScrolledToBottom", id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BrowserInterop.IsScrolledToBottom: {ex.GetBaseException().Message}");
            }
            return new ValueTask<bool>(Task.FromResult(false));
        }
        public static ValueTask<bool> ScrollIntoView(this IJSRuntime JSRuntime, string id)
        {
            return JSRuntime.InvokeAsync<bool>("chat.scrollIntoView", id);
        }
        [Obsolete("Please use SetFocusById now as there is a bug in the JSInterop", true)]
        public static ValueTask<bool> SetFocus(this IJSRuntime JSRuntime, ElementReference elementRef)
        {
            return JSRuntime.InvokeAsync<bool>("chat.setFocus", elementRef);
        }
        public static ValueTask<bool> SetFocusById(this IJSRuntime JSRuntime, string id)
        {
            return JSRuntime.InvokeAsync<bool>("chat.setFocusById", id);
        }
    }
}
