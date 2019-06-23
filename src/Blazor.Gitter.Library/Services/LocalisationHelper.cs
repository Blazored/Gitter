using Microsoft.JSInterop;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Blazor.Gitter.Library
{
    public class LocalisationHelper : ILocalisationHelper
    {
        private TimeZoneInfo localTimeZoneInfo;
        private CultureInfo localCultureInfo;
        private IJSRuntime JSRuntime;

        public TimeZoneInfo LocalTimeZoneInfo => localTimeZoneInfo ?? TimeZoneInfo.Local;
        public CultureInfo LocalCultureInfo => localCultureInfo ?? CultureInfo.CurrentCulture;

        public LocalisationHelper(IJSRuntime jSRuntime)
        {
            JSRuntime = jSRuntime;
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
                Console.WriteLine(ex);
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
                Console.WriteLine(ex);
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
                Console.WriteLine(ex);
                return string.Empty;
            }
        }
    }
}
