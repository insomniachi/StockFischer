namespace Lichess.Board;

public class WriteInChatParameters : Parameters
{
    public Room Room { get; set; }
    public string Text { get; set; }
}

public enum Room
{
    Player,
    Spectator,
}
