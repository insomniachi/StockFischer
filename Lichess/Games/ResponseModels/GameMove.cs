using System.Text.Json.Serialization;

namespace Lichess.Games;

public class GameMove
{
    public string Fen { get; set; } = string.Empty;

    [JsonPropertyName("wc")]
    public int WhiteTimeRemaining { get; set; }

    [JsonPropertyName("bc")]
    public int BlackTimeRemaining { get; set; }

    [JsonPropertyName("lm")]
    public string LastMove { get; set; } = string.Empty;
}
