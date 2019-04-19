using System;

namespace Blazor.Gitter.Library
{
    public interface IChatMessage
    {

        string Id { get; set; }
        string Text { get; set; }
        string Html { get; set; }
        DateTime Sent { get; set; }
        DateTime? EditedAt { get; set; }
        IChatUser FromUser { get; }
        bool Unread { get; set; }
        int ReadBy { get; set; }
        IChatUrl[] Urls { get; }
        IChatMention[] Mentions { get; }
        IChatIssue[] Issues { get; }
        object Meta { get; set; } //unused
        int V { get; set; }
    }
}