using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Blazor.Gitter.Library
{
    public class GitterApi : IChatApi
    {
        private const string APIBASE = "https://api.gitter.im/v1/";
        private const string APIUSERPATH = "user";
        private const string APIROOMS = "rooms";

        private string Token { get; set; }
        private HttpClient HttpClient { get; set; }
        public GitterApi(HttpClient httpClient = null)
        {
            HttpClient = httpClient ?? throw new Exception("Make sure you have added an HttpClient to your DI Container");
        }

        public void SetAccessToken(string token)
        {
            Token = token;
            PrepareHttpClient();
        }

        private void PrepareHttpClient()
        {
            if (HttpClient.BaseAddress == null || HttpClient.BaseAddress.ToString() != APIBASE)
            {
                HttpClient.BaseAddress = new Uri(APIBASE);
                HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token.Replace("\"",String.Empty)}");
            }
        }

        public async Task<IChatUser> GetCurrentUser()
        {
            try
            {
                Console.WriteLine("Fetching gitter user.");
                return (await HttpClient.GetFromJsonAsync<GitterUser[]>(APIUSERPATH)).First();
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public Task<IChatUser> GetChatUser(string UserId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IChatUser>> GetChatRoomUsers(string RoomId, IRoomUserOptions Options)
        {
            return await HttpClient.GetFromJsonAsync<GitterUser[]>($"{APIROOMS}/{RoomId}/users{Options}");
        }

        public async Task<IChatRoom> GetChatRoom(string UserId, string RoomId)
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"id", RoomId}
            });

            var response = await HttpClient.PostAsJsonAsync($"{APIUSERPATH}/{UserId}/{APIROOMS}", content);

            return (await response.Content.ReadFromJsonAsync<GitterRoom[]>()).First();
        }

        public async Task<IEnumerable<IChatRoom>> GetChatRooms(string UserId)
        {
            return await HttpClient.GetFromJsonAsync<GitterRoom[]>($"{APIUSERPATH}/{UserId}/{APIROOMS}");
        }

        public Task<IChatMessage> GetChatMessage(string RoomId, string MessageId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IChatMessage>> GetChatMessages(string RoomId, IChatMessageOptions Options)
        {
            return await HttpClient.GetFromJsonAsync<GitterMessage[]>($"{APIROOMS}/{RoomId}/chatMessages{Options}");
        }

        public async Task<IEnumerable<IChatMessage>> SearchChatMessages(string RoomId, IChatMessageOptions Options)
        {
            if (string.IsNullOrWhiteSpace(Options.Query))
            {
                return default;
            }
            return await GetChatMessages(RoomId, Options);
        }

        public async Task<IChatMessage> SendChatMessage(string RoomId, string Message)
        {
            var content = new NewMessage() { text = Message };

            var response = await HttpClient.PostAsJsonAsync($"{APIROOMS}/{RoomId}/chatMessages", content);

            var result = await response.Content.ReadFromJsonAsync<GitterMessage>();

            return result;
        }

        public async Task<IChatMessage> EditChatMessage(string RoomId, string MessageId, string Message)
        {
            var content = new NewMessage() { text = Message };

            var response = await HttpClient.PutAsJsonAsync($"{APIROOMS}/{RoomId}/chatMessages/{MessageId}", content);

            return await response.Content.ReadFromJsonAsync<GitterMessage>();
        }

        public async Task<bool> MarkChatMessageAsRead(string UserId, string RoomId, string MessageId)
        {
            var content = new MarkUnread { chat = new string[] { MessageId } };
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{APIUSERPATH}/{UserId}/{APIROOMS}/{RoomId}/unreadItems", content);

                var result = await response.Content.ReadFromJsonAsync<SimpleSuccess>();
                return result.success;
            }
            catch { }
            return false;
        }

        public IChatMessageOptions GetNewOptions()
        {
            return new GitterMessageOptions();
        }
    }
    public class NewMessage
    {
        public string text { get; set; }
    }
    public class MarkUnread
    {
        public string[] chat { get; set; }
    }

    public class SimpleSuccess
    {
        public bool success { get; set; }
    }
}
