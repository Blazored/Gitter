using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core.Components
{
    public class AppModel : ComponentBase
    {
        [Inject] IComponentContext ComponentContext { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }

        internal List<System.Reflection.Assembly> AssemblyList;
        private bool HasLoaded;
        internal bool EmbedRequired => HasLoaded && !( JSRuntime is IJSInProcessRuntime );

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            AssemblyList = new List<System.Reflection.Assembly>()
                {
                    typeof(ILocalStorageService).Assembly,
                    GetType().Assembly,
                };
        }
        protected override void OnAfterRender()
        {
            base.OnAfterRender();
            if (ComponentContext.IsConnected && !HasLoaded)
            {
                HasLoaded = true;
                StateHasChanged();
            }
        }
    }
}
