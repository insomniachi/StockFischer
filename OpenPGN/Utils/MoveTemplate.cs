using OpenPGN.Models;

namespace OpenPGN.Utils
{
    public class MoveTemplate
    {
        public int UpDown { get; private init; }
        public int LeftRight { get; private init; }
        public int DiagonalUpDown { get; private init; }

        public static readonly MoveTemplate King = new() { UpDown = 1, LeftRight = 1, DiagonalUpDown = 1 };
        public static readonly MoveTemplate Queen = new() { UpDown = 8, LeftRight = 8, DiagonalUpDown = 8 };
        public static readonly MoveTemplate Rook = new() { UpDown = 8, LeftRight = 8 };
        public static readonly MoveTemplate Bishop = new() { DiagonalUpDown = 8 };

        public IEnumerable<Square> GetMoves(Square s)
        {
            var moves = new List<Square>();

            if (UpDown > 0)
            {
                FillMoves(s, moves, UpDown, 1, 0);
            }
            if (LeftRight > 0)
            {
                FillMoves(s, moves, LeftRight, 0, 1);
            }
            if (DiagonalUpDown > 0)
            {
                FillMoves(s, moves, DiagonalUpDown, 1, 1);
                FillMoves(s, moves, DiagonalUpDown, 1, -1);
            }

            return moves;
        }

        private static void FillMoves(Square square, List<Square> moves, int limit, int dim1, int dim2)
        {
            _ = moves ?? throw new ArgumentNullException(nameof(moves));
            
            var positiveLimitReached = false;
            var negativeLimitReached = false;

            for (int i = 1; i <= limit; i++)
            {
                if (positiveLimitReached == false)
                {
                    Square positive = square.Move(Color.White, i * dim1, i * dim2);
                    if (positive == Square.Invalid) positiveLimitReached = true;

                    if (positiveLimitReached == false)
                    {
                        moves.Add(positive);

                    }
                }

                if (negativeLimitReached == false)
                {
                    Square negative = square.Move(Color.White, -i * dim1, -i * dim2);
                    if (negative == Square.Invalid) negativeLimitReached = true;

                    if (negativeLimitReached == false)
                    {
                        moves.Add(negative);
                    }
                }

                if (positiveLimitReached && negativeLimitReached)
                {
                    break;
                }
            }
        }
    }
}
