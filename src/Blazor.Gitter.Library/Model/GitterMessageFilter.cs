namespace Blazor.Gitter.Library
{
    public class GitterMessageFilter : IChatMessageFilter
    {
        public GitterMessageFilter()
        {
        }

        public bool FilterUnread { get; set; } = false;
        public string FilterByUserId { get; set; } = string.Empty;

        public override string ToString()
            => $"Unread: {FilterUnread} UserId: {FilterByUserId}";
    }
}