using Blazor.Gitter.Library;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core
{
    public class AppModel : ComponentBase
    {
        [Inject] ILocalisationHelper LocalisationHelper { get; set; }
        
        protected override async Task OnInitAsync()
        {
            await base.OnInitAsync();
            await LocalisationHelper.BuildLocalCulture();
            await LocalisationHelper.BuildLocalTimeZone();
        }
    }
}
