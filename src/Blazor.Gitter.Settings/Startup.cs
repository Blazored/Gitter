using Blazor.Gitter.Components.ViewModels.Settings;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Gitter.Settings
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILocalStorageService, LocalStorageService>();
            services.AddSingleton<Settings_VM>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
