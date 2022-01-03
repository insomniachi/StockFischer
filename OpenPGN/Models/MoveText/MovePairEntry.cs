namespace OpenPGN.Models;

/// <summary>
/// Represents a full move (white and black). 
/// </summary>
public class MovePairEntry : MoveTextEntry
{

    public Move White { get; set; }

    public Move Black { get; set; }

    public MovePairEntry(Move white, Move black)
        : base(MoveTextEntryType.MovePair)
    {
        White = white;
        Black = black;
    }
}
