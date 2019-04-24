namespace Blazor.Gitter.Library
{
    public interface IChatMessageOptions
    {
        string BeforeId { get; set; }
        string AfterId { get; set; }
        string Lang { get; set; }
        int Limit { get; set; }
        int Skip { get; set; }
        string Query { get; set; }
    }
}