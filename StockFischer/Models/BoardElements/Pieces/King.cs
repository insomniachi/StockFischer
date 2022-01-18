using OpenPGN;
using OpenPGN.Models;
using OpenPGN.Utils;
using System.Collections.Generic;
using System.Linq;

namespace StockFischer.Models.BoardElements.Pieces;

internal class King : RangedPiece
{
    public bool HasMoved { get; private set; }
    public bool CanCastleKingSide { get; set; }
    public bool CanCastleQueenSide { get; set; }

    public IEnumerable<Square> KingSideCastleSquares { get; }
    public IEnumerable<Square> QueenSideCastleSquares { get; }
    public override string Glyph => Color == Color.White ? "♔" : "♚";

    public King(Piece piece, Square square) : base(piece, square)
    {
        KingSideCastleSquares = piece.Color == Color.White ? new[] { Square.G1, Square.H1 } : new[] { Square.G8, Square.H8 };
        QueenSideCastleSquares = piece.Color == Color.White ? new[] { Square.C1 , Square.A1} : new[] { Square.C8, Square.A8 };
    }
    public override MoveTemplate MoveTemplate { get; } = MoveTemplate.King;
    public override IEnumerable<Square> GetLegalMoves(BoardSetup boardSetup)
    {
        var candidates = MoveTemplate.GetMoves(Square)
            .Where(x => boardSetup[x] is null || boardSetup[x] is { } p && p.Color != Color).ToList();

        if ((Color == Color.White ? boardSetup.CanWhiteCastleKingSide : boardSetup.CanBlackCastleKingSide) && CanCastleKingSide)
        {
            candidates.AddRange(KingSideCastleSquares);
        }
        if ((Color == Color.White ? boardSetup.CanWhiteCastleQueenSide : boardSetup.CanBlackCastleQueenSide) && CanCastleQueenSide)
        {
            candidates.AddRange(QueenSideCastleSquares);
        }

        return candidates.Where(x => !boardSetup.IsAttacked(x, Color.Invert()));
    }

    public override void Move(Square square)
    {
        HasMoved = true;
        base.Move(square);
    }
}
