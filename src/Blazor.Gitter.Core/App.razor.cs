using Blazor.Gitter.Core.Components.Shared;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core
{
    public class AppModel : ComponentBase
    {

        internal List<System.Reflection.Assembly> AssemblyList;

        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();

            AssemblyList = new List<System.Reflection.Assembly>()
                {
                    typeof(ILocalStorageService).Assembly,
                    typeof(MainLayout).Assembly
                };
        }
    }
}
