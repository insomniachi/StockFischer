namespace Lichess;

public class GameFull
{
    public string Type { get; set; } = "gameFull";
    public string Id { get; set; }
    public Variant Variant { get; set; }
    public Clock Clock { get; set; }
    public string Speed { get; set; }
    public Perf Perf { get; set; }
    public bool Rated { get; set; }
    public int CreatedAt { get; set; }
    public User White { get; set; }
    public User Black { get; set; }
    public string InitialFen { get; set; }
    public GameState Status { get; set; }
    public string TournamentId { get; set; }
}
