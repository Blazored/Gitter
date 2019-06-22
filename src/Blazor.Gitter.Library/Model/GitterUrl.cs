using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blazor.Gitter.Library
{
    public class GitterUrl : IChatUrl
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
