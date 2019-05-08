namespace Blazor.Gitter.Library
{
    public class CloudinaryImage : ICdnImage
    {
        public CloudinaryImage()
        {

        }

        public string name { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public long bytes { get; set; }
        public string url { get; set; }
        public string secure_url { get; set; }
        public CloudinaryImage[] eager { get; set; }
        public string transformation { get; set; }
        public ICdnImage[] transformations { get => eager; set => eager = (CloudinaryImage[])value; }
    }
}
