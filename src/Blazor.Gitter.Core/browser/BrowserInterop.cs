using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Browser
{
    static class BrowserInterop
    {
        public static Task<double> GetScrollTop(this IJSRuntime JSRuntime, string id)
        {
            return JSRuntime.InvokeAsync<double>("chat.getScrollTop",id);
        }
        public static Task<bool> IsScrolledToBottom(this IJSRuntime JSRuntime, string id)
        {
            try
            {
                return JSRuntime.InvokeAsync<bool>("chat.isScrolledToBottom", id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BrowserInterop.IsScrolledToBottom: {ex.GetBaseException().Message}");
            }
            return Task.FromResult(false);
        }
        public static Task<bool> ScrollIntoView(this IJSRuntime JSRuntime, string id)
        {
            return JSRuntime.InvokeAsync<bool>("chat.scrollIntoView", id);
        }
        [Obsolete("Please use SetFocusById now as there is a bug in the JSInterop",true)]
        public static Task<bool> SetFocus(this IJSRuntime JSRuntime, ElementRef elementRef)
        {
            return JSRuntime.InvokeAsync<bool>("chat.setFocus", elementRef);
        }
        public static Task<bool> SetFocusById(this IJSRuntime JSRuntime, string id)
        {
            return JSRuntime.InvokeAsync<bool>("chat.setFocusById", id);
        }
    }
}
