using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Blazor.Gitter.Library
{
    public class CloudinaryApi : ICdnApi, IDisposable
    {
        private const string BASEURI = "https://api.cloudinary.com";
        private const string PRESET = "gitter";
        private const string APIVERSION = "v1_1";
        private const string CLOUDNAME = "blazoredgitter";

        public CloudinaryApi()
        {
            HttpClient = new HttpClient
            {
                BaseAddress = new Uri(BASEURI)
            };
        }

        public HttpClient HttpClient { get; }

        public void Dispose()
        {
            HttpClient?.Dispose();
        }

        public async Task<ICdnImage> UploadAsync(string file)
        {
            try
            {
                string fileType = file.Substring(5, file.IndexOf("base64") - 6);
                var values = new FileUpload
                {
                    name = fileType.Replace('/', '.'),
                    file = file,
                    upload_preset = PRESET
                };

                var response = await HttpClient.PostJsonAsync<CloudinaryImage>($"{APIVERSION}/{CLOUDNAME}/auto/upload", values);
                response.name = values.name;
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine($"CDN ERROR:{e.GetBaseException().Message}");
            }
            return default;
        }
    }
    public class FileUpload
    {
        public string name;
        public string content_type = "image/png";
        public string file;
        public string upload_preset;
    }
}
