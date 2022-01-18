namespace Lichess;

public class GameStartEndEvent
{
    public string Type { get; set; }
    public LichessGame Game { get; set; }

    internal GameStartEndEvent()
    {
        Type = string.Empty;
        Game = new();
    }
}
