using System.Text.Json.Serialization;

namespace Lichess.Board;

public class Challenge
{
    public string Id { get; set; }
    public string Url { get; set; }
    public string Status { get; set; }
    public Compat Compat { get; set; }
    public User Challenger { get; set; }
    public Variant Variant { get; set; }
    public bool Rated { get; set; }
    public string Speed { get; set; }
    public TimeControl TimeControl { get; set; }
    public string Color { get; set; }
    public Perf Perf { get; set; }
    public string Direction { get; set; }
    public string InitialFen { get; set; }
    public string DeclineReason { get; set; }


    [JsonPropertyName("destUser")]
    public User DestinationUser { get; set; }
}
