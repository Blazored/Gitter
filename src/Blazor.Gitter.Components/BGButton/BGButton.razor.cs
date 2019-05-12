using Microsoft.AspNetCore.Components;
using System;

namespace Blazor.Gitter.Components.BGButton
{
    public class BGButtonBase : ComponentBase
    {
        //
        //  2019-03-05  Mark Stega
        //              Created
        //

        public const string kStdId_Cancel = "CANCEL";
        public const string kStdId_No = "NO";
        public const string kStdId_OK = "OK";
        public const string kStdId_Yes = "YES";

        protected void IntClick(UIMouseEventArgs e)
        {
            OnClick?.Invoke(new BGButtonRes(ButtonId, UserData));
        }

        [Parameter] protected string ButtonId { get; set; }
        [Parameter] protected object UserData { get; set; } = null;
        [Parameter] protected string ButtonText { get; set; }
        [Parameter] protected Action<BGButtonRes> OnClick { get; set; }
    }

    public class BGButtonRes : UIEventArgs
    {
        public BGButtonRes(string id, object data)
        {
            ButtonId = id;
            UserData = data;
        }

        public string ButtonId { get; set; }
        public object UserData { get; set; } = null;
    };
}
