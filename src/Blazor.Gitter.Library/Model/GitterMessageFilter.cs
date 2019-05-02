namespace Blazor.Gitter.Library
{
    public class GitterMessageFilter : IChatMessageFilter
    {
        public GitterMessageFilter()
        {
        }

        public bool FilterUnread { get; set; } = false;
    }
}