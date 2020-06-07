using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core
{
    public class LocalisationHelper : ILocalisationHelper
    {
        private TimeZoneInfo localTimeZoneInfo;
        private CultureInfo localCultureInfo;
        private IJSRuntime JSRuntime;
        private readonly ILogger<ILocalisationHelper> Log;

        public TimeZoneInfo LocalTimeZoneInfo => localTimeZoneInfo ?? TimeZoneInfo.Local;
        public CultureInfo LocalCultureInfo => localCultureInfo ?? CultureInfo.CurrentCulture;

        public LocalisationHelper(IJSRuntime jSRuntime, ILogger<ILocalisationHelper> log)
        {
            JSRuntime = jSRuntime;
            Log = log;
        }
        public async Task BuildLocalTimeZone()
        {
            try
            {
                var tz = await JSRuntime.InvokeAsync<string>("eval", "Intl.DateTimeFormat().resolvedOptions().timeZone");
                int tzo = await JSRuntime.InvokeAsync<int>("eval", "new Date().getTimezoneOffset()");
                localTimeZoneInfo = TimeZoneInfo.CreateCustomTimeZone(tz ?? "Unknown", new TimeSpan(0, 0 - tzo, 0), tz, tz);
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Failed to build local time zone");
                localTimeZoneInfo = TimeZoneInfo.Local;
            }
        }
        public async Task BuildLocalCulture()
        {
            try
            {
                var lan = await JSRuntime.InvokeAsync<string>("eval", "navigator.language");
                localCultureInfo = new CultureInfo(lan);
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Failed to build local culture info");
                localCultureInfo = CultureInfo.CurrentCulture;
            }
        }
        public async Task<string> GetKey(string key)
        {
            try
            {
                string result = await JSRuntime.InvokeAsync<string>("localStorage.getItem", key);
                return result;
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Failed to get key");
                return string.Empty;
            }
        }
    }
}
