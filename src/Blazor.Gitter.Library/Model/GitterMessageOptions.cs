using System.Text;

namespace Blazor.Gitter.Library
{
    public class GitterMessageOptions : IChatMessageOptions
    {
        public string BeforeId { get; set; }
        public string AfterId { get; set; }
        public string Lang { get; set; }
        public string Query { get; set; }
        public int Limit { get; set; } = 20;
        public int Skip { get; set; } 
        public override string ToString()
        {
            StringBuilder s = new StringBuilder("?");

            if (!string.IsNullOrWhiteSpace(AfterId))
            {
                s.Append($"afterId={AfterId.Trim()}");
            }
            if (!string.IsNullOrWhiteSpace(BeforeId))
            {
                if (s.Length > 1) s.Append("&");
                s.Append($"beforeId={BeforeId.Trim()}");
            }
            if (!string.IsNullOrWhiteSpace(Lang))
            {
                if (s.Length > 1) s.Append("&");
                s.Append($"lang={Lang.Trim()}");
            }
            if (!string.IsNullOrWhiteSpace(Query))
            {
                if (s.Length > 1) s.Append("&");
                s.Append($"q={System.Net.WebUtility.UrlEncode(Query.Trim())}");
            }
            if (Limit > 0)
            {
                if (s.Length > 1) s.Append("&");
                s.Append($"limit={Limit}");
            }
            if (Skip > 0)
            {
                if (s.Length > 1) s.Append("&");
                s.Append($"skip={Skip}");
            }

            if (s.Length == 1) s.Clear();
            return s.ToString();
        }
    }
}
