using OpenPGN;
using OpenPGN.Models;
using OpenPGN.Utils;
using System.Collections.Generic;
using System.Linq;

namespace StockFischer.Models.BoardElements.Pieces
{
    internal class King : RangedPiece
    {
        public bool HasMoved { get; private set; }
        public bool CanCastleKingSide { get; set; }
        public bool CanCastleQueenSide { get; set; }

        public Square KingSideCastleSquare { get; }
        public Square QueenSideCastleSquare { get; }
        public override string Glyph => Color == Color.White ? "♔" : "♚"; 

        public King(Piece piece, Square square) : base(piece, square)
        {
            KingSideCastleSquare = piece.Color == Color.White ? Square.G1 : Square.G8;
            QueenSideCastleSquare = piece.Color == Color.White ? Square.C1 : Square.C8;
        }
        public override MoveTemplate MoveTemplate { get; } = MoveTemplate.King;
        public override IEnumerable<Square> GetLegalMoves(BoardSetup boardSetup)
        {
            var candidates = MoveTemplate.GetMoves(Square)
                .Where(x => boardSetup[x] is null || boardSetup[x] is { } p && p.Color != Color).ToList();

            if ((Color == Color.White ? boardSetup.CanWhiteCastleKingSide : boardSetup.CanBlackCastleKingSide) && CanCastleKingSide)
            {
                candidates.Add(KingSideCastleSquare);
            }
            if ((Color == Color.White ? boardSetup.CanWhiteCastleQueenSide : boardSetup.CanBlackCastleQueenSide) && CanCastleQueenSide)
            {
                candidates.Add(QueenSideCastleSquare);
            }

            return candidates.Where(x => !boardSetup.IsAttacked(x, Color.Invert()));
        }

        public override void Move(Square square)
        {
            HasMoved = true;
            base.Move(square);
        }
    }

}
