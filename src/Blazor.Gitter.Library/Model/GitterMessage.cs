using System;

namespace Blazor.Gitter.Library
{
    public class GitterMessage : IChatMessage
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Html { get; set; }
        public DateTime Sent { get; set; }
        public DateTime? EditedAt { get; set; }
        public GitterUser FromUser { get; set; }
        public bool Unread { get; set; }
        public int ReadBy { get; set; }
        public GitterUrl[] Urls { get; set; }
        public GitterMention[] Mentions { get; set; }
        public GitterIssue[] Issues { get; set; }
        public object Meta { get; set; }
        public int V { get; set; }
        IChatUser IChatMessage.FromUser { get => FromUser; }
        IChatMention[] IChatMessage.Mentions { get => Mentions; }
        IChatIssue[] IChatMessage.Issues { get => Issues; }
        IChatUrl[] IChatMessage.Urls { get => Urls; }
        public bool IsStatus => Text.StartsWith("/me");
    }
}

