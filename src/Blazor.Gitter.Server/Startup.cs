using Blazor.Gitter.Core.Components.Shared;
using Blazor.Gitter.Library;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

namespace Blazor.Gitter.Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<HttpClient>((s) => new HttpClient())
                .AddScoped<IChatApi, GitterApi>()
                .AddScoped<ILocalStorageService, LocalStorageService>()
                .AddScoped<ILocalisationHelper, LocalisationHelper>()
                .AddScoped<IAppState, AppState>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(routes =>
            {
                routes.MapBlazorHub<Core.Components.App>("app");
                routes.MapFallbackToPage("/_Host");
            });
        }
    }
}
