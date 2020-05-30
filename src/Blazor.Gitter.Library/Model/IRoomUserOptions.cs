namespace Blazor.Gitter.Library
{
    public interface IRoomUserOptions
    {
        int Limit { get; set; }
        int Skip { get; set; }
        string Query { get; set; }
    }
}