using OpenPGN.Models;

namespace OpenPGN
{
    public static class SquareExtensions
    {
        public static Square Move(this Square square, Color playerPerspective, int up, int right)
        {
            Func<Square, Color, int, Square> upFunc = Math.Sign(up) == 1 ? Up : Down;
            Func<Square, Color, int, Square> rightFunc = Math.Sign(right) == 1 ? Right : Left;

            var step1 = upFunc(square, playerPerspective, Math.Abs(up));
            var step2 = rightFunc(step1, playerPerspective, Math.Abs(right));

            return step2;
        }

        public static Square Up(this Square square, Color playerPerspective, int count = 1)
        {
            if (square == Square.Invalid || count == 0) return square;

            if (playerPerspective == Color.White)
            {
                if (square.Rank + count > 8) return Square.Invalid;
            }
            else
            {
                if (square.Rank - count < 1) return Square.Invalid;
            }

            return playerPerspective == Color.White
                ? new Square(square.File, square.Rank + count)
                : new Square(square.File, square.Rank - count);
        }

        public static Square Down(this Square square, Color playerPerspective, int count = 1)
        {
            if (square == Square.Invalid || count == 0) return square;

            if (playerPerspective == Color.Black)
            {
                if (square.Rank + count > 8) return Square.Invalid;
            }
            else
            {
                if (square.Rank - count < 1) return Square.Invalid;
            }

            return playerPerspective == Color.Black
                ? new Square(square.File, square.Rank + count)
                : new Square(square.File, square.Rank - count);
        }

        public static Square Right(this Square square, Color playerPerspective, int count = 1)
        {
            if (square == Square.Invalid || count == 0) return square;

            int file = square.File.ToInt();
            if (playerPerspective == Color.White)
            {
                if (file + count > 8) return Square.Invalid;
            }
            else
            {
                if (file - count < 1) return Square.Invalid;
            }

            return playerPerspective == Color.White
                ? new Square(FileExtensions.FromInt(file + count), square.Rank)
                : new Square(FileExtensions.FromInt(file - count), square.Rank);
        }

        public static Square Left(this Square square, Color playerPerspective, int count = 1)
        {
            if (square == Square.Invalid || count == 0) return square;

            int file = square.File.ToInt();
            if (playerPerspective == Color.Black)
            {
                if (file + count > 8) return Square.Invalid;
            }
            else
            {
                if (file - count < 1) return Square.Invalid;
            }

            return playerPerspective == Color.Black
                ? new Square(FileExtensions.FromInt(file + count), square.Rank)
                : new Square(FileExtensions.FromInt(file - count), square.Rank);
        }

        public static IEnumerable<Square> SquaresInLine(Square a, Square b)
        {
            var all = Square.AsEnumerable();

            if(a.Rank == b.Rank)
            {
                return all.Where(x => x.Rank == a.Rank); 
            }

            if(a.File == b.File)
            {
                return all.Where(x => x.File == a.File);
            }

            if (Math.Abs(a.File - b.File) != Math.Abs(a.Rank - b.Rank)) 
                return Enumerable.Empty<Square>();
            
            var max = a.Rank > b.Rank ? a : b;
            var min = a.Rank > b.Rank ? b : a;

            var dir = min.File < max.File ? 1 : -1;

            var result = new List<Square> { min };
                
            var s = min.Move(Color.White, 1, dir);
            while(s != Square.Invalid)
            {
                result.Add(s);
                s = s.Move(Color.White, 1, dir);
            }

            s = min.Move(Color.White, -1, -dir);
            while(s != Square.Invalid)
            {
                result.Add(s);
                s = s.Move(Color.White, 1, -dir);
            }

            return result;

        }

        public static IEnumerable<Square> SquaresInBetween(Square a, Square b, bool includeLower = false, bool includeUpper = false)
        {
            List<Square> result = new();

            if(a.Rank == b.Rank)
            {
                int max = Math.Max(a.File.ToInt(), b.File.ToInt());
                int min = Math.Min(a.File.ToInt(), b.File.ToInt());

                for (int i = min + 1; i < max; i++)
                {
                    result.Add(Square.New(FileExtensions.FromInt(i), a.Rank));
                }
            }
            if(a.File == b.File)
            {
                int max = Math.Max(a.Rank, b.Rank);
                int min = Math.Min(a.Rank, b.Rank);

                for (int i = min + 1; i < max; i++)
                {
                    result.Add(Square.New(a.File, i));
                }
            }
            else if(Math.Abs(a.File - b.File) == Math.Abs(a.Rank - b.Rank))
            {
                var max = a.Rank > b.Rank ? a : b;
                var min = a.Rank > b.Rank ? b : a;

                int dir = min.File < max.File ? 1 : -1;
                int fileStart = min.File.ToInt();
                int j = 1;
                for (int i = min.Rank + 1; i < max.Rank; i++, j++)
                {
                    result.Add(Square.New(FileExtensions.FromInt(fileStart + j*dir), i));
                }
            }

            if (includeLower) result.Add(a);
            if (includeUpper) result.Add(b);

            return result;
        }
    }
}
