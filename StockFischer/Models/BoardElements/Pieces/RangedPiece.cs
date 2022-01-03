using OpenPGN;
using OpenPGN.Models;
using OpenPGN.Utils;
using System.Collections.Generic;
using System.Linq;

namespace StockFischer.Models.BoardElements.Pieces
{
    internal abstract class RangedPiece : LivePiece
    {
        public abstract MoveTemplate MoveTemplate { get; } 

        internal RangedPiece(Piece piece, Square square) : base(piece, square) { }

        private void AddLegalMoves(BoardSetup boardSetup, List<Square> squares, int limit, int dim1, int dim2)
        {
            bool positiveLimitReached = false;
            bool negativeLimitReached = false;

            for (int i = 1; i <= limit; i++)
            {
                if(positiveLimitReached == false)
                {
                    Square positive = Square.Move(Color, i * dim1, i * dim2);
                    if (positive == Square.Invalid) positiveLimitReached = true;

                    if(positiveLimitReached == false)
                    {
                        if (boardSetup[positive] is Piece p)
                        {
                            if (p.Color != Color)
                            {
                                squares.Add(positive);
                                positiveLimitReached = true;
                            }
                            else
                            {
                                positiveLimitReached = true;
                            }
                        }
                        else
                        {
                            squares.Add(positive);
                        }
                    }
                }

                if (negativeLimitReached == false)
                {
                    Square negative = Square.Move(Color, -i * dim1, -i * dim2);
                    if (negative == Square.Invalid) negativeLimitReached = true;

                    if (negativeLimitReached == false)
                    {
                        if (boardSetup[negative] is Piece p)
                        {
                            if (p.Color != Color)
                            {
                                squares.Add(negative);
                                negativeLimitReached = true;
                            }
                            else
                            {
                                negativeLimitReached = true;
                            }
                        }
                        else
                        {
                            squares.Add(negative);
                        }
                    }
                }

                if(positiveLimitReached && negativeLimitReached)
                {
                    break;
                }
            }
        }

        public override IEnumerable<Square> GetPossibleMoves(BoardSetup boardSetup)
        {
            var candidates = new List<Square>();

            if(MoveTemplate.UpDown > 0)
            {
                AddLegalMoves(boardSetup, candidates, MoveTemplate.UpDown, 1, 0);
            }

            if(MoveTemplate.LeftRight > 0)
            {
                AddLegalMoves(boardSetup, candidates, MoveTemplate.LeftRight, 0, 1);
            }

            if (MoveTemplate.DiagonalUpDown > 0)
            {
                AddLegalMoves(boardSetup, candidates, MoveTemplate.DiagonalUpDown, 1, 1);
                AddLegalMoves(boardSetup, candidates, MoveTemplate.DiagonalUpDown, 1, -1);
            }

            if(Piece.PieceType == PieceType.King)
            {
                var attackedSquares = candidates.Where(x => boardSetup.IsAttacked(x, Piece.Color.Invert())).ToList();
                attackedSquares.ForEach(x => candidates.Remove(x));
            }

            return candidates;
        }


    }
}
