namespace Lichess.Board;

public class TimeControl
{
    public string Type { get; set; }
    public int Limit { get; set; }
    public int Increment { get; set; }
    public string Show { get; set; }
    public int DaysPerTurn { get; set; } // for correspondence
}
