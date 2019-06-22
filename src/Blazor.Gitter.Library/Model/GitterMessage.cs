using System;
using System.Text.Json.Serialization;

namespace Blazor.Gitter.Library
{
    public class GitterMessage : IChatMessage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("html")]
        public string Html { get; set; }
        [JsonPropertyName("sent")]
        public DateTime Sent { get; set; }
        [JsonPropertyName("editedAt")]
        public DateTime? EditedAt { get; set; }
        [JsonPropertyName("fromUser")]
        public GitterUser FromUser { get; set; }
        [JsonPropertyName("unread")]
        public bool Unread { get; set; }
        [JsonPropertyName("readBy")]
        public int ReadBy { get; set; }
        [JsonPropertyName("urls")]
        public GitterUrl[] Urls { get; set; }
        [JsonPropertyName("mentions")]
        public GitterMention[] Mentions { get; set; }
        [JsonPropertyName("issues")]
        public GitterIssue[] Issues { get; set; }
        [JsonPropertyName("meta")]
        public object[] Meta { get; set; }
        [JsonPropertyName("v")]
        public int V { get; set; }
        IChatUser IChatMessage.FromUser { get => FromUser; }
        IChatMention[] IChatMessage.Mentions { get => Mentions; }
        IChatIssue[] IChatMessage.Issues { get => Issues; }
        IChatUrl[] IChatMessage.Urls { get => Urls; }
        public bool IsStatus => Text.StartsWith("/me");
    }
}

