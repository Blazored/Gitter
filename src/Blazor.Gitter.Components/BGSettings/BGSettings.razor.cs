using Blazor.Gitter.Components.ViewModels.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;

//
//  2019-05-11  Mark Stega
//              Created
//
namespace Blazor.Gitter.Components.BGSettings
{
    public class BGSettingsBase : ComponentBase
    {
        [Inject] protected Settings_VM pSettingsVM { get; set; }

        [Inject] IJSRuntime jsRuntime { get; set; }

        // We invoke this to report OK or cancel
        [Parameter] private Action<bool> OnCompletion { get; set; }

        public BGDialog.BGDialog pSettingsDialog;

        protected async void OnDialogComplete(string buttonId)
        {
            bool result = buttonId == BGButton.BGButton.kStdId_OK;
            if (result)
            {
                // Update the Model & request storage.
                await pSettingsVM.Save();
            }

            OnCompletion?.Invoke(result);
        }

    }
}
