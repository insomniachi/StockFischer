using OpenPGN;
using OpenPGN.Models;
using System.Collections.Generic;

namespace StockFischer.Models.BoardElements.Pieces;

internal class Knight : LivePiece
{
    public override string Glyph => Color == Color.White ? "♘" : "♞";

    static List<(int Up, int Right)> moveTemplate = new()
    {
        new(1, 2),
        new(1, -2),
        new(-1, 2),
        new(-1, -2),
        new(2, 1),
        new(2, -1),
        new(-2, 1),
        new(-2, -1)
    };

    internal Knight(Piece piece, Square square) : base(piece, square) { }

    public static IEnumerable<Square> GetMovesFrom(Square s)
    {
        var moves = new List<Square>();

        foreach (var (Up, Right) in moveTemplate)
        {
            if (s.Move(Color.White, Up, Right) is Square q && q != Square.Invalid)
            {
                moves.Add(q);
            }
        }

        return moves;
    }

    public override IEnumerable<Square> GetPossibleMoves(BoardSetup boardSetup)
    {
        var candidates = new List<Square>();

        foreach (var (Up, Right) in moveTemplate)
        {
            if (Square.Move(Color, Up, Right) is Square q && q != Square.Invalid)
            {
                if (boardSetup[q] is Piece p)
                {
                    if (p.Color != Color)
                    {
                        candidates.Add(q);
                    }
                }
                else
                {
                    candidates.Add(q);
                }
            }
        }

        return candidates;
    }
}
