using System;
using System.Text.RegularExpressions;

namespace Blazor.Gitter.Library
{
    public class GitterMessage : IChatMessage
    {
        private string html;

        public string Id { get; set; }
        public string Text { get; set; }
        public string Html { get => HandleEmojis(html); set => html = value; }
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

        private string HandleEmojis(string html)
        {
            //<img class="emoji" src="//cdn01.gitter.im/_s/da1325d59/images/emoji/point_up.png" height="20" width="20" title=":point_up:" alt=":point_up:" align="absmiddle">
            var regx = new Regex(":([a-z_]+):");
            return regx.Replace(html, match => $"<img class=\"emoji\" src=\"//cdn01.gitter.im/_s/da1325d59/images/emoji/{match.Groups[1].Value}.png\" height=\"20\" width=\"20\" title=\"{match.Value}\" alt=\"{match.Value}\" align=\"absmiddle\">");
        }
    }
}

