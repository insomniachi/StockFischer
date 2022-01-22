using Lichess.Games;
using System.Text.Json.Serialization;

namespace Lichess.TV;

public class CurrentTvGame
{
    [JsonPropertyName("t")]
    public string Type { get; set; }

    [JsonPropertyName("d")]
    public CurrentTvGameDescription Details { get; set; }
}

public class CurrentTvGameMove
{
    [JsonPropertyName("t")]
    public string Type { get; set; }

    [JsonPropertyName("d")]
    public GameMove Details { get; set; }
}
