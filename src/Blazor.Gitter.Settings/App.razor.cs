using Blazor.Gitter.Components.BGButton;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazor.Gitter.Settings
{
    public class AppModel : ComponentBase
    {

        internal List<System.Reflection.Assembly> AssemblyList;

        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();

            AssemblyList = new List<System.Reflection.Assembly>()
                {
                    typeof(BGButton).Assembly
                };
        }
    }
}
