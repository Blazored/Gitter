using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Blazor.Gitter.Components
{
    public static class BGDOMHelpers
    {
        public static Task setDOMElementVal(this ElementRef elementRef, IJSRuntime jsRuntime, string toSet)
        {
            return jsRuntime.InvokeAsync<string>("customizationJsFunctions.setDOMElementVal", elementRef, toSet);
        }

        public static Task<string> getDOMElementVal(this ElementRef elementRef, IJSRuntime jsRuntime)
        {
            return jsRuntime.InvokeAsync<string>("customizationJsFunctions.getDOMElementVal", elementRef);
        }

        public static Task<string> setDOMElementBgnClr(this ElementRef elementRef, IJSRuntime jsRuntime, string toSet)
        {
            return jsRuntime.InvokeAsync<string>("customizationJsFunctions.setDOMElementBgnClr", elementRef, toSet);
        }

        public static Task<string> setModalState(IJSRuntime jsRuntime, bool toSet)
        {
            return jsRuntime.InvokeAsync<string>("customizationJsFunctions.setModalState", toSet);
        }
    }
}
