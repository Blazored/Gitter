using Blazor.Gitter.Components.BGButton;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading;
using static Blazor.Gitter.Components.BGDialog.BGDialogBase;

namespace Blazor.Gitter.Settings.Pages
{
    public class SettingsBase : ComponentBase
    {
        [Inject]
        public IUriHelper UriHelper { get; set; }

        private static Timer m_Timer;

        // An enum we use to control modal component visibility
        protected enum ModalComps
        {
            None, SettingsDialog
        };
        protected ModalComps CurModal = ModalComps.None;

        protected override void OnInit()
        {
            base.OnInit();
            var autoEvent = new AutoResetEvent(false);
            m_Timer = new Timer(new TimerCallback(TimerCallback), autoEvent, 100, -1);
        }

        private void TimerCallback(Object state)
        {
            if (CurModal == ModalComps.None)
            {
                CurModal = ModalComps.SettingsDialog;
                Invoke(() => { StateHasChanged(); });
            }
        }

        protected void BGSettingsDialogCompletedCallback(bool result)
        {
            CurModal = ModalComps.None;
            UriHelper.NavigateTo("/home");
        }

    }
}
