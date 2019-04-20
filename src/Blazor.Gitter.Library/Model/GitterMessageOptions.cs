using System.Text;

namespace Blazor.Gitter.Library
{
    public class GitterMessageOptions : IChatMessageOptions
    {
        public string BeforeId { get; set; }
        public string AfterId { get; set; }
        public string Lang { get; set; }
        public int Limit { get; set; } = 20;
        public override string ToString()
        {
            StringBuilder s = new StringBuilder("?");

            if (!string.IsNullOrEmpty(AfterId))
            {
                s.Append($"afterId={AfterId.Trim()}");
            }
            if (!string.IsNullOrEmpty(BeforeId))
            {
                if (s.Length > 1) s.Append("&");
                s.Append($"beforeId={BeforeId.Trim()}");
            }
            if (!string.IsNullOrEmpty(Lang))
            {
                if (s.Length > 1) s.Append("&");
                s.Append($"lang={Lang.Trim()}");
            }
            if (Limit > 0)
            {
                if (s.Length > 1) s.Append("&");
                s.Append($"limit={Limit}");
            }

            if (s.Length == 1) s.Clear();
            return s.ToString();
        }
    }
}
