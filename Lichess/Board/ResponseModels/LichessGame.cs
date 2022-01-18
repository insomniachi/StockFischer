namespace Lichess;

public class LichessGame
{
    public string Id { get; set; }
    public string Source { get; set; }
    public Compat Compat { get; set; }

    internal LichessGame()
    {
        Id = string.Empty;
        Compat = new();
    }
}
