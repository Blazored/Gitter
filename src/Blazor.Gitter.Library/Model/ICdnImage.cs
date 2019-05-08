namespace Blazor.Gitter.Library
{
    public interface ICdnImage
    {
        string name { get; set; }
        long bytes { get; set; }
        int height { get; set; }
        string secure_url { get; set; }
        string transformation { get; set; }
        string url { get; set; }
        int width { get; set; }
        ICdnImage[] transformations { get; set; }
    }
}