namespace Lichess;

public class ChatLine
{
    public string Type { get; set; } = "chatLine";
    public string Room { get; set; }
    public string Username { get; set; }
    public string Text { get; set; }
}