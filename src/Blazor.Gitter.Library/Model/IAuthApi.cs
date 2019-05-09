using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazor.Gitter.Library
{
    public interface IAuthApi
    {
        string GetLoginUrl();
        Task<string> GetAccessToken(string exchangeToken);
    }
}