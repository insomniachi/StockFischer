namespace OpenPGN.Models;

/// <summary>
/// Represents a half-move (or ply). E.g.  "17. Ne5" (white moves knight e5) or "17... Qxe5" (black queen takes e5)
/// </summary>
public class HalfMoveEntry : MoveTextEntry
{

    public bool IsContinued { get; set; }

    public Move Move { get; set; }

    public HalfMoveEntry(Move move)
        : base(MoveTextEntryType.SingleMove)
    {
        Move = move;
    }
}
