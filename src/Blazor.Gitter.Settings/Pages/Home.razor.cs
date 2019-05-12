using System.Threading.Tasks;
using Blazor.Gitter.Components.ViewModels.Settings;
using Microsoft.AspNetCore.Components;

namespace Blazor.Gitter.Settings.Pages
{
    public class HomeBase : ComponentBase
    {
        [Inject] protected Settings_VM pSettingsVM { get; set; }

        private bool m_IsFirstRendering = true;

        protected override async Task OnAfterRenderAsync()
        {
            if (m_IsFirstRendering)
            {
                await pSettingsVM.Restore();
                StateHasChanged();
                m_IsFirstRendering = false;
            }
        }
    }
}
