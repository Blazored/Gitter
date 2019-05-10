using Microsoft.AspNetCore.Components;
using Blazor.Gitter.Components.BGButton;
using System;
using System.Collections.Generic;

namespace Blazor.Gitter.Components.BGDialog
{
    public class BGDialogBase : ComponentBase
    {
        //
        //  2018-11-20  Mark Stega
        //              Created
        //


        // The standard button styles we support
        public enum ButtonStyles
        {
            None, OK, OKCancel, YesNo
        };

        // We invoke this to report OK or cancel
        [Parameter] private Action<string> OnDialogCompletion { get; set; }

        // We get this passed in for the title
        [Parameter] protected string DialogTitle { get; set; }

        // And this for button display
        [Parameter] protected ButtonStyles ButtonSelection { get; set; } = ButtonStyles.None;

        //
        //  They can tell us to set a specific percent height and width of modal container. Default 
        //  is zero, which means just wrap around the content.
        //
        [Parameter] protected int PercentHeight { get; set; } = 0;
        [Parameter] protected int PercentWidth { get; set; } = 0;

        //
        //  An optional list of key/value pairs for custom buttons. They key is the button id, and
        //  the value is the button text.
        //
        [Parameter] protected List<KeyValuePair<string, string>> CustButtons { get; set; } = null;


        [Parameter] protected RenderFragment ChildContent { get; set; }


        //
        //  Each dialog that uses this component will set its id on us, which we pass to
        //  our modal component.
        //
        [Parameter] protected string DialogPopupId { get; set; } = Guid.NewGuid().ToString();

        // A ref to our modal component that we need to interact with
        protected BGModal.BGModal BGModalRef;

        // We ask our modal comp to invoke modal mode at this point
        protected override void OnAfterRender()
        {
            base.OnAfterRender();

            // Tell the modal component to invoke modal mode
            BGModalRef.InvokeModal(DialogPopupId);
        }

        protected void OnClickComplete(BGButtonRes result)
        {
            // Dismiss the modal, passing our popup id
            BGModalRef.DismissModal(DialogPopupId);
            OnDialogCompletion?.Invoke(result.ButtonId);
        }


        // Called by the contaiing dialog
        public void DismissModal()
        {
            // Dismiss the modal, passing our popup id
            BGModalRef.DismissModal(DialogPopupId);
        }

        public string GetPopupId()
        {
            return DialogPopupId;
        }

        protected string GetSizeStyles()
        {
            string retStr = "";
            if (PercentHeight != 0)
                retStr += "height : " + PercentHeight.ToString() + "%;";
            if (PercentWidth != 0)
                retStr += "width : " + PercentWidth.ToString() + "%;";

            return retStr;
        }
    }
}
