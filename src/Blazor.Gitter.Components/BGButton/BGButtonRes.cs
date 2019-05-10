using Microsoft.AspNetCore.Components;

namespace Blazor.Gitter.Components.BGButton
{
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
