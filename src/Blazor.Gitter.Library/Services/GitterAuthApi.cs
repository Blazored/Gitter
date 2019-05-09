using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Blazor.Gitter.Library
{
    public class GitterAuthApi : IAuthApi
    {
        private const string APIBASE = "http://gitter.im/login/oauth/";

        private const string OAUTHKEY = "";
        private const string OAUTHSECRET = "";
        private const string REDIRECTURI = "http://localhost:57226/";

        private string Token { get; set; }
        private HttpClient HttpClient { get; set; }
        public GitterAuthApi(HttpClient httpClient = null)
        {
            HttpClient = httpClient ?? throw new Exception("Make sure you have added an HttpClient to your DI Container");
        }

        public async Task<string> GetAccessToken(string exchangeToken)
        {
            PrepareHttpClient();

            var result = await HttpClient.PostJsonAsync<GitterAccessToken>(
                String.Format("{0}token",
               APIBASE 
               ), new
                {
                    code = exchangeToken,
                    client_id = OAUTHKEY,
                    client_secret = OAUTHSECRET,
                    redirect_uri = REDIRECTURI,
                    grant_type = "authorization_code"
                });

            return null;
        }

        private void PrepareHttpClient()
        {
            if (!(HttpClient.BaseAddress is object))
            {
                HttpClient.BaseAddress = new Uri(APIBASE);
                HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        public string GetLoginUrl()
        {
            return String.Format("{0}authorize?redirect_uri={1}&response_type={2}&client_id={3}",
               APIBASE,
               "http://localhost:57226/",  //redirect_uri
               "code",                     //response_type
               OAUTHKEY                    //client_id
               );
        }
    }
}
