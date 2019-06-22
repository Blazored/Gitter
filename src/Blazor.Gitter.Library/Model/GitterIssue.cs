using System.Text.Json.Serialization;

namespace Blazor.Gitter.Library
{
    public class GitterIssue : IChatIssue
    {
        [JsonPropertyName("number")]
        public string Number { get; set; }
    }
}

