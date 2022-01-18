namespace Lichess;

public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public int Rating { get; set; }
    public bool Patron { get; set; }
    public bool Online { get; set; }
    public int Lag { get; set; }
    public bool Provisional { get; set; }
    public int AiLevel { get; set; }
}
