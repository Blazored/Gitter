using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Blazor.Gitter.Library
{
    public interface ILocalisationHelper
    {
        CultureInfo LocalCultureInfo { get; }
        TimeZoneInfo LocalTimeZoneInfo { get; }

        Task BuildLocalCulture();
        Task BuildLocalTimeZone();
        Task<string> GetKey(string key);
    }
}