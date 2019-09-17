using Blazor.Gitter.Components.ViewModels.Settings;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

//
//  2019-05-11  Mark Stega
//              Created
//
namespace Blazor.Gitter.Components
{
    public class BGSettingsBase : ComponentBase
    {
        [Inject] protected Settings_VM pSettingsVM { get; set; }
        [Inject] protected IModalService pModalService { get; set; }

        public async Task Save()
        {
            // Save the new settings
            await pSettingsVM.Save();
            pModalService.Close(ModalResult.Ok<bool>(true));
        }

        public async void Cancel()
        {
            // Restore the original settings
            await pSettingsVM.Restore();
            pModalService.Cancel();
        }


    }
}
