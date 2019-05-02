namespace Blazor.Gitter.Library
{
    public interface IChatMessageFilter
    {
        bool FilterUnread { get; set; }
        string FilterByUserId { get; set; }
    }
}