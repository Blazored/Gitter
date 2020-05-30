using System.Text;

namespace Blazor.Gitter.Library
{
    public class GitterRoomUserOptions : IRoomUserOptions
    {
        public string Query { get; set; }
        public int Limit { get; set; } = 30;
        public int Skip { get; set; }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder("?");

            if (!string.IsNullOrWhiteSpace(Query))
            {
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
