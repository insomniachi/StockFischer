using System.Text.Json.Serialization;

namespace Lichess;

public class Error
{
    [JsonPropertyName("error")]
    public string ErrorMessage { get; set; }
}
