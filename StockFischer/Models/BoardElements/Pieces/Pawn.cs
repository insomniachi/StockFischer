using OpenPGN;
using OpenPGN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFischer.Models.BoardElements.Pieces
{
    internal class Pawn : LivePiece
    {
        internal Pawn(Piece piece, Square square) : base(piece, square) { }

        public override IEnumerable<Square> GetPossibleMoves(BoardSetup boardSetup)
        {
            var candidateMoves = new List<Square>();
            
            // Move up 1 square
            var candidate = Square.Up(Color);
            if(candidate != Square.Invalid && boardSetup[candidate] is null)
            {
                candidateMoves.Add(candidate);
            }

            // Move up 2 squares if on starting position
            if(IsAtStartingPosition())
            {
                candidate = Square.Up(Color, 2);
                if(candidate != Square.Invalid && boardSetup[candidate] is null && boardSetup[candidate.Down(Color)] is null)
                {
                    candidateMoves.Add(candidate);
                }
            }

            // Captures
            var captures = new List<Square>()
            {
                Square.Move(Color, 1,1),
                Square.Move(Color, 1,-1)
            };

            foreach (var move in captures)
            {
                if (move != Square.Invalid && boardSetup[move] is Piece p && p.Color != Color)
                {
                    candidateMoves.Add(move);
                }
            }

            // En passant
            if(boardSetup.EnPassantSquare is Square ep)
            {
                if(captures.Any(x => x == ep))
                {
                    candidateMoves.Add(ep);
                }
            }

            return candidateMoves;
        }

        internal bool IsAtStartingPosition() => Color == Color.White
                ? Square.Rank == 2
                : Square.Rank == 7;
    }
}
