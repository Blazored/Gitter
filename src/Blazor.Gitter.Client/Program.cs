using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.Extensions.Logging;
using Blazor.Gitter.Core;
using Blazor.Gitter.Core.Components;
using Blazor.Gitter.Core.Components.Shared;
using Blazor.Gitter.Library;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blazor.Gitter.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
                .AddSingleton<IChatApi, GitterApi>()
                .AddSingleton<ILocalStorageService, LocalStorageService>()
                .AddSingleton<ILocalisationHelper, LocalisationHelper>()
                .AddSingleton<IAppState, AppState>();

            builder.Services.AddLogging(builder => builder
                .AddBrowserConsole()
                .SetMinimumLevel(LogLevel.Trace)
                .AddFilter("Microsoft.AspNetCore.*", level => level >= LogLevel.Information)
            );

            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}
