using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazor.Gitter.Core;
using Blazor.Gitter.Core.Components;
using Blazor.Gitter.Core.Components.Shared;
using Blazor.Gitter.Library;
using Blazor.Gitter.Library.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

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
                .AddSingleton<RoomUsersRepository>()
                .AddSingleton<IAppState, AppState>();
            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}
