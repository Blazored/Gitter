using System.Threading.Tasks;

namespace Blazor.Gitter.Library
{
    public interface ICdnApi
    {
        Task<ICdnImage> UploadAsync(string url);
    }
}
