using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Gitter.Library
{
    public class GitterRoom : IChatRoom
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Topic { get; set; }
        public string AvatarUrl { get; set; }
        public string Uri { get; set; }
        public bool OneToOne { get; set; }
        public int UserCount { get; set; }
        public GitterUser User { get; set; }
        public int UnreadItems { get; set; }
        public int Mentions { get; set; }
        public DateTime LastAccessTime { get; set; }
        public int Favourite { get; set; }
        public bool Lurk { get; set; }
        public string Url { get; set; }
        public string GithubType { get; set; }
        public string Security { get; set; }
        public bool Noindex { get; set; }
        public string[] Tags { get; set; }
        public bool RoomMember { get; set; }
        public string GroupId { get; set; }
        public int V { get; set; }
    }
}

