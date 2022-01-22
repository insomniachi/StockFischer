using System.Text.Json.Serialization;

namespace Lichess.Board;

public class GameState
{
    public string Type { get; set; }
    public string Moves { get; set; }
    public string Status { get; set; }
    public string Winner { get; set; }

    [JsonPropertyName("wtime")]
    public int WhiteTime { get; set; }

    [JsonPropertyName("btime")]
    public int BlackTime { get; set; }

    [JsonPropertyName("winc")]
    public int WhiteIncrement { get; set; }

    [JsonPropertyName("binc")]
    public int BlackIncrement { get; set; }

    [JsonPropertyName("wdraw")]
    public bool WhiteDrawOffer { get; set; }

    [JsonPropertyName("bdraw")]
    public bool BlackDrawOffer { get; set; }

    [JsonPropertyName("wtakeback")]
    public bool WhiteProposeTakeback { get; set; }

    [JsonPropertyName("btakeback")]
    public bool BlackProposeTakeback { get; set; }
}
