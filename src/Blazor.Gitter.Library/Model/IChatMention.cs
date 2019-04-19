namespace Blazor.Gitter.Library
{
    public interface IChatMention
    {
        string ScreenName { get; set; }
        string UserId { get; set; }
    }
}