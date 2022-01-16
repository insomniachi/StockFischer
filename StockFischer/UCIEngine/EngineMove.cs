using OpenPGN.Models;
using System.ComponentModel;

namespace StockFischer.Engine;

public class EngineMove
{
    public Square From { get; set; }
    public Square To { get; set; }
    public PieceType? PromotedPiece { get; set; }

    public override string ToString()
    {
        return $"{From}{To}{PromotedPiece}";
    }

    private static string PieceToString(PieceType? p)
    {
        if (p == null) return string.Empty;

        return p switch
        {
            PieceType.Pawn => "p",
            PieceType.Knight => "n",
            PieceType.Bishop => "b",
            PieceType.Rook => "r",
            PieceType.Queen => "q",
            PieceType.King => "k",
            _ => throw new InvalidEnumArgumentException()
        };
    }
}
