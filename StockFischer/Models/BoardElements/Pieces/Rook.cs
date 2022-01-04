using OpenPGN.Models;
using OpenPGN.Utils;

namespace StockFischer.Models.BoardElements.Pieces
{

    internal class Rook : RangedPiece
    {
        public override string Glyph => Color == Color.White ? "♖" : "♜︎"; 
        public bool HasMoved { get; private set; }

        internal Rook(Piece piece, Square square) : base(piece, square) { }

        public override MoveTemplate MoveTemplate { get; } = MoveTemplate.Rook;

        public override void Move(Square square)
        {
            HasMoved = true;
            base.Move(square);
        }
    }
}
