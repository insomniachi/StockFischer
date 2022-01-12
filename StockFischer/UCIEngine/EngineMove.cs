using OpenPGN.Models;
using System.ComponentModel;

namespace StockFischer.Engine
{
    public class EngineMove
    {
        public Square OriginSquare { get; set; }
        public Square TargetSquare { get; set; }
        public PieceType? PromotedPiece { get; set; }

        public override string ToString()
        {
            return $"{OriginSquare}{TargetSquare}{PromotedPiece}";
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
}
