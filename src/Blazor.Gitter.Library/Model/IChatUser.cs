namespace Blazor.Gitter.Library
{
    public interface IChatUser
    {
        string Id { get; set; }
        string Username { get; set; }
        string DisplayName { get; set; }
        string Url { get; set; }
        string AvatarUrlSmall { get; set; }
        string AvatarUrlMedium { get; set; }
    }
}
