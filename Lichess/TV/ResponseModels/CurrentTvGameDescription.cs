namespace Lichess.TV;

public class CurrentTvGameDescription
{
    public string Id { get; set; }
    public string Orientation { get; set; }
    public User[] Players { get; set; }
    public string Fen { get; set; }
}
