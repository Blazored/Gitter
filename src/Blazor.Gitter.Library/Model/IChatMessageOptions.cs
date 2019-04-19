namespace Blazor.Gitter.Library
{
    public interface IChatMessageOptions
    {
        string AfterId { get; set; }
        string Lang { get; set; }
        int Limit { get; set; }
    }
}