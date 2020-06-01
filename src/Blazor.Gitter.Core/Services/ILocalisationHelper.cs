using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Blazor.Gitter.Core
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