using OpenPGN.Models;
using OpenPGN.Utils;

namespace StockFischer.Models.BoardElements.Pieces;

internal class Queen : RangedPiece
{
    public override string Glyph => Color == Color.White ? "♕" : "♛︎";

    public Queen(Piece piece, Square square) : base(piece, square) { }

    public override MoveTemplate MoveTemplate { get; } = MoveTemplate.Queen;
}
