using System.Text.Json.Serialization;

namespace Blazor.Gitter.Library
{
    public class GitterMention : IChatMention
    {
        [JsonPropertyName("screenName")]
        public string ScreenName { get;set; }
        [JsonPropertyName("userId")]
        public string UserId { get;set; }
    }
}
