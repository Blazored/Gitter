using System;

namespace Blazor.Gitter.Library
{
    public interface IChatRoom
    {
        string Id { get; set; }
        string Name { get; set; }
        string Topic { get; set; }
        string AvatarUrl { get; set; }
        string Uri { get; set; }
        bool OneToOne { get; set; }
        int UserCount { get; set; }
        int UnreadItems { get; set; }
        int Mentions { get; set; }
        DateTime LastAccessTime { get; set; }
        int Favourite { get; set; }
        bool Lurk { get; set; }
        string Url { get; set; }
        string GithubType { get; set; }
        string Security { get; set; }
        bool Noindex { get; set; }
        string[] Tags { get; set; }
        bool RoomMember { get; set; }
        string GroupId { get; set; }
        int V { get; set; }
    }
}