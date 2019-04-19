namespace Blazor.Gitter.Library
{
    public class GitterUser : IChatUser
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Url { get; set; }
        public string AvatarUrlSmall { get; set; }
        public string AvatarUrlMedium { get; set; }
    }
}
