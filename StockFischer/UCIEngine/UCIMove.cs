using OpenPGN.Models;
using System;
using System.ComponentModel;

namespace StockFischer.Engine;

public class UCIMove
{
    public Square From { get; set; }
    public Square To { get; set; }
    public PieceType? PromotedPiece { get; set; }

    public override string ToString() => $"{From}{To}{PromotedPiece}";

    public static UCIMove Parse(string uciMove)
    {
        if (uciMove.Length < 4)
        {
            return null;
        }

        PieceType? promotedPiece = null;
        var origin = Square.New((File)uciMove[0], int.Parse(uciMove[1].ToString()));
        var target = Square.New((File)uciMove[2], int.Parse(uciMove[3].ToString()));

        if (uciMove.Length == 5)
        {
            promotedPiece = CharToPiece(uciMove[4]);
        }

        return new UCIMove
        {
            From = origin,
            To = target,
            PromotedPiece = promotedPiece,
        };
    }

    private static PieceType CharToPiece(char piece) => char.ToLower(piece) switch
    {
        'p' => PieceType.Pawn,
        'n' => PieceType.Knight,
        'b' => PieceType.Bishop,
        'r' => PieceType.Rook,
        'q' => PieceType.Queen,
        'k' => PieceType.King,
        _ => throw new ArgumentException(null, nameof(piece))
    };
}
