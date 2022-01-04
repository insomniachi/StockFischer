using OpenPGN.Models;
using OpenPGN.Utils;

namespace StockFischer.Models.BoardElements.Pieces
{
    internal class Bishop : RangedPiece
    {
        public override string Glyph => Color == Color.White ? "♗" : "♝"; 
        public Bishop(Piece piece, Square square) : base(piece, square) { }

        public override MoveTemplate MoveTemplate { get; } = MoveTemplate.Bishop;
    }
}
