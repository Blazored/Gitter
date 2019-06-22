using System.Text.Json.Serialization;

namespace Blazor.Gitter.Library
{
    public class GitterUser : IChatUser
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("avatarUrlSmall")]
        public string AvatarUrlSmall { get; set; }
        [JsonPropertyName("avatarUrlMedium")]
        public string AvatarUrlMedium { get; set; }
    }
}
