namespace Lichess.Games;

public class ExportGameQueryParameters : Parameters
{
    public bool Moves { get; set; } = true;
    public bool PgnInJson { get; set; }
    public bool Tags { get; set; } = true;
    public bool Clocks { get; set; } = true;
    public bool Evals { get; set; } = true;
    public bool Opening { get; set; } = true;
    public bool Literate { get; set; } = true;
    public string Players { get; set; }
}
