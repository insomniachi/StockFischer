namespace Lichess;

public class LiveGameStatus
{
    public string Id { get; set; } = string.Empty;
    public Variant Variant { get; set; } = new();
    public string Perf { get; set; } = string.Empty;
    public bool Rated { get; set; }
    public string InitialFen { get; set; } = string.Empty;
    public string Fen { get; set; } = string.Empty;
    public string Player { get; set; } = string.Empty;
    public int Turns { get; set; }
    public int StartedAtTurn { get; set; }
    public string Source { get; set; } = string.Empty;
    public GameStatus Status { get; set; } = new();
    public long CreatedAt { get; set; }
    public string LastMove { get; set; } = string.Empty;
    public string Winner { get; set; } = string.Empty;
}
