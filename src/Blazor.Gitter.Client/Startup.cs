using Blazor.Gitter.Core.Components.Shared;
using Blazor.Gitter.Library;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Gitter.Client
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {                   
            services
                .AddSingleton<IChatApi, GitterApi>()
                .AddSingleton<ICdnApi, CloudinaryApi>()
                .AddSingleton<ILocalStorageService, LocalStorageService>()
                .AddSingleton<ILocalisationHelper, LocalisationHelper>()
                .AddSingleton<IAppState, AppState>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<Core.App<Core.Components.Shared.Navbar>>("app");
        }
    }
}
