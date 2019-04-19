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
            var tz = await JSRuntime.InvokeAsync<string>("eval", "Intl.DateTimeFormat().resolvedOptions().timeZone");
            int.TryParse(await JSRuntime.InvokeAsync<string>("eval", "new Date().getTimezoneOffset()"), out int tzo);
            try
            {
                localTimeZoneInfo = TimeZoneInfo.CreateCustomTimeZone(tz ?? "Unknown", new TimeSpan(0, 0 - tzo, 0), tz, tz);
            }
            catch
            {
                localTimeZoneInfo = TimeZoneInfo.Local;
            }
        }
        public async Task BuildLocalCulture()
        {
            var lan = await JSRuntime.InvokeAsync<string>("eval", "navigator.language");
            try
            {
                localCultureInfo = new CultureInfo(lan);
            }
            catch
            {
                localCultureInfo = CultureInfo.CurrentCulture;
            }
        }
    }
}
