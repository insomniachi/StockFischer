using OpenPGN.Models;
using OpenPGN.Utils;

namespace OpenPGN
{
    public class Attack
    {
        public Square Square { get; }
        public Piece Piece { get; }
        public bool IsPiecePinnedToKing { get; }

        public Attack(Square square, Piece piece, bool isPiecePinnedToKing)
        {
            Square = square;
            Piece = piece;
            IsPiecePinnedToKing = isPiecePinnedToKing;
        }

    }

    public enum GameState
    {
        None,
        Check,
        DoubleCheck,
        Checkmate,
        Stalemate
    }

    public static class BoardSetupExtensions
    {
        private static readonly List<(int up, int right)> KnightTemplate = new()
        {
            new ValueTuple<int, int>(1, 2),
            new ValueTuple<int, int>(1, -2),
            new ValueTuple<int, int>(-1, 2),
            new ValueTuple<int, int>(-1, -2),
            new ValueTuple<int, int>(2, 1),
            new ValueTuple<int, int>(2, -1),
            new ValueTuple<int, int>(-2, 1),
            new ValueTuple<int, int>(-2, -1)
        };

        static IEnumerable<Square> GetKnightMoves(Square s)
        {
            var moves = new List<Square>();

            foreach (var (up, right) in KnightTemplate)
            {
                if (s.Move(Color.White, up, right) is { } q && q != Square.Invalid)
                {
                    moves.Add(q);
                }
            }

            return moves;
        }

        public static AttackResult IsAttacked(this BoardSetup boardSetup, Square a, Color attacker)
        {
            return boardSetup.IsAttackByPawn(a, attacker) |
                   boardSetup.IsAttackedByKnight(a, attacker) |
                   boardSetup.IsAttackedAlongDiagonals(a, attacker) |
                   boardSetup.IsAttackedAlongFile(a, attacker) |
                   boardSetup.IsAttackedAlongRank(a, attacker);

        }

        // TODO : refactor IsAttackeBy/Along* functions
        private static AttackResult IsAttackByPawn(this BoardSetup boardSetup, Square target, Color attacker)
        {
            var result = new AttackResult();

            var moves = new List<Square>
            {
                target.Move(attacker.Invert(), 1, 1),
                target.Move(attacker.Invert(), 1, -1)
            };

            foreach (var move in moves.Where(x => x != Square.Invalid))
            {
                if (boardSetup[move] is not { PieceType: PieceType.Pawn } p || p.Color != attacker) continue;

                result.IsAttacked = true;
                result.Attacks.Add(new Attack(move, p, boardSetup.IsSquarePinned(move, boardSetup.GetKingPosition(attacker), attacker.Invert())));
            }

            return result;
        }

        private static AttackResult IsAttackedByKnight(this BoardSetup boardSetup, Square target, Color attacker)
        {
            var result = new AttackResult();

            foreach (var move in GetKnightMoves(target))
            {
                if (boardSetup[move] is not { PieceType: PieceType.Knight } p || p.Color != attacker) continue;

                result.IsAttacked = true;
                result.Attacks.Add(new Attack(move, p, boardSetup.IsSquarePinned(move, boardSetup.GetKingPosition(attacker), attacker.Invert())));
            }

            return result;
        }

        private static AttackResult IsAttackedAlongDiagonals(this BoardSetup boardSetup, Square target, Color attacker)
        {
            Color player = attacker.Invert();
            var result = new AttackResult();

            var start = target.Move(player, 1, 1);
            while (start != Square.Invalid)
            {
                if (boardSetup[start] is { } p)
                {
                    if (p.Color != attacker)
                    {
                        break;
                    }
                    if (p.PieceType is PieceType.Queen or PieceType.Bishop)
                    {
                        result.IsAttacked = true;
                        result.Attacks.Add(new Attack(start, p, boardSetup.IsSquarePinned(start, boardSetup.GetKingPosition(attacker), attacker.Invert())));
                    }
                    break;
                }

                start = start.Move(player, 1, 1);
            }

            start = target.Move(player, 1, -1);
            while (start != Square.Invalid)
            {
                if (boardSetup[start] is { } p)
                {
                    if (p.Color != attacker)
                    {
                        break;
                    }

                    if (p.PieceType is PieceType.Queen or PieceType.Bishop)
                    {
                        result.IsAttacked = true;
                        result.Attacks.Add(new Attack(start, p, boardSetup.IsSquarePinned(start, boardSetup.GetKingPosition(attacker), attacker.Invert())));
                    }
                    break;
                }

                start = start.Move(player, 1, -1);
            }

            start = target.Move(player, -1, 1);
            while (start != Square.Invalid)
            {
                if (boardSetup[start] is { } p)
                {
                    if (p.Color != attacker)
                    {
                        break;
                    }

                    if (p.PieceType is PieceType.Queen or PieceType.Bishop)
                    {
                        result.IsAttacked = true;
                        result.Attacks.Add(new Attack(start, p, boardSetup.IsSquarePinned(start, boardSetup.GetKingPosition(attacker), attacker.Invert())));
                    }
                    break;
                }

                start = start.Move(player, -1, 1);
            }

            start = target.Move(player, -1, -1);
            while (start != Square.Invalid)
            {
                if (boardSetup[start] is { } p)
                {
                    if (p.Color != attacker)
                    {
                        break;
                    }
                    else
                    {
                        if (p.PieceType is PieceType.Queen or PieceType.Bishop)
                        {
                            result.IsAttacked = true;
                            result.Attacks.Add(new Attack(start, p, boardSetup.IsSquarePinned(start, boardSetup.GetKingPosition(attacker), attacker.Invert())));
                        }
                        break;
                    }
                }

                start = start.Move(player, -1, -1);
            }

            return result;
        }

        private static AttackResult IsAttackedAlongRank(this BoardSetup boardSetup, Square target, Color attacker)
        {
            var result = new AttackResult();

            var player = attacker.Invert();
            var start = target.Right(player);
            while (start != Square.Invalid)
            {
                if (boardSetup[start] is { } p)
                {
                    if (p.Color != attacker)
                    {
                        break;
                    }

                    if (p.PieceType is PieceType.Queen or PieceType.Rook)
                    {
                        result.IsAttacked = true;
                        result.Attacks.Add(new Attack(start, p, boardSetup.IsSquarePinned(start, boardSetup.GetKingPosition(attacker), attacker.Invert())));
                    }
                    break;
                }

                start = start.Right(player);
            }

            start = target.Left(player);
            while (start != Square.Invalid)
            {
                if (boardSetup[start] is { } p)
                {
                    if (p.Color != attacker)
                    {
                        break;
                    }

                    if (p.PieceType is PieceType.Queen or PieceType.Rook)
                    {
                        result.IsAttacked = true;
                        result.Attacks.Add(new Attack(start, p, boardSetup.IsSquarePinned(start, boardSetup.GetKingPosition(attacker), attacker.Invert())));
                    }
                    break;
                }
                start = start.Left(player);
            }

            return result;
        }

        private static AttackResult IsAttackedAlongFile(this BoardSetup boardSetup, Square target, Color attacker)
        {
            var result = new AttackResult();

            var player = attacker.Invert();
            Square start = target.Up(player);

            while (start != Square.Invalid)
            {
                if (boardSetup[start] is { } p)
                {
                    if (p.Color != attacker)
                    {
                        break;
                    }

                    if (p.PieceType is PieceType.Queen or PieceType.Rook)
                    {
                        result.IsAttacked = true;
                        result.Attacks.Add(new Attack(start, p, boardSetup.IsSquarePinned(start, boardSetup.GetKingPosition(attacker), attacker.Invert())));
                    }
                    break;
                }

                start = start.Up(player);
            }

            start = target.Down(player);
            while (start != Square.Invalid)
            {
                if (boardSetup[start] is { } p)
                {
                    if (p.Color != attacker)
                    {
                        break;
                    }

                    if (p.PieceType is PieceType.Queen or PieceType.Rook)
                    {
                        result.IsAttacked = true;
                        result.Attacks.Add(new Attack(start, p, boardSetup.IsSquarePinned(start, boardSetup.GetKingPosition(attacker), attacker.Invert())));
                    }
                    break;

                }
                start = start.Down(player);
            }

            return result;
        }

        /// <summary>
        /// Get the current game state (check/mate/stalemate)
        /// </summary>
        /// <param name="boardSetup"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static GameState GetGameState(this BoardSetup boardSetup, Color color)
        {
            var attacker = color.Invert();
            var kingSquare = boardSetup.GetKingPosition(color);

            if (kingSquare == Square.Invalid)
            {
                throw new Exception("King not found");
            }

            var result = boardSetup.IsAttacked(kingSquare, attacker);

            // King is not attacked
            if (!result)
            {
                // TODO : check stalemate
                return GameState.None;
            }

            // Check if there are any unoccupied squares, or enemy pieces which are undefended
            var canKingMove = MoveTemplate.King.GetMoves(kingSquare)
                                                .Where(x => boardSetup[x] is null || boardSetup[x] is { } p && p.Color == attacker)
                                                .Any(x => !boardSetup.IsAttacked(x, attacker));
            // King has a move
            if (canKingMove)
            {
                return result.Attacks.Count == 1 ? GameState.Check : GameState.DoubleCheck;
            }

            // Can't defend against 2 attacks
            // more than 2 attacks in legally not possible
            if (result.Attacks.Count == 2)
            {
                return GameState.Checkmate;
            }

            var attack = result.Attacks.Single();

            // Check if we can capture attacker
            result = boardSetup.IsAttacked(attack.Square, color);
            if (result)
            {
                // Can capture attacker
                if (result.Attacks.Any(x => boardSetup.IsSquarePinned(x.Square, kingSquare, attacker) == false))
                {
                    return GameState.Check;
                }
            }

            var squares = SquareExtensions.SquaresInBetween(kingSquare, attack.Square).ToList();

            // attack from point blank range or its a knight, can't block
            if (!squares.Any())
            {
                return GameState.Checkmate;
            }

            // Check if we can block the attack
            foreach (var square in squares)
            {
                result = boardSetup.IsAttacked(square, color);

                // Can block
                if (result.Attacks.Any(x => boardSetup.IsSquarePinned(x.Square, kingSquare, attacker) == false))
                {
                    return GameState.Check;
                }
            }

            return GameState.Checkmate;
        }

        /// <summary>
        /// Check if square is pinned to another square.
        /// </summary>
        /// <param name="boardSetup"></param>
        /// <param name="pinned"></param>
        /// <param name="to"></param>
        /// <param name="attacker"></param>
        /// <returns></returns>
        public static bool IsSquarePinned(this BoardSetup boardSetup, Square pinned, Square to, Color attacker)
        {
            var squaresInBetween = SquareExtensions.SquaresInBetween(pinned, to).ToList();
            var squaresInLine = SquareExtensions.SquaresInLine(pinned, to).ToList();

            // squares are not in a straight line
            if (!squaresInLine.Any())
            {
                return false;
            }

            // there is something between the squares
            if (squaresInBetween.Any(x => boardSetup[x] is { }))
            {
                return false;
            }

            var squaresExcept = squaresInLine.Except(squaresInBetween).ToList();
            squaresExcept.Remove(pinned);
            squaresExcept.Remove(to);

            // squares are at edges of the boards
            if (!squaresExcept.Any())
            {
                return false;
            }

            var attackSquares = Enumerable.Empty<Square>();
            Func<Piece, bool> condition = _ => false;

            if (pinned.Rank == to.Rank)
            {
                // in horizontal/vertical line, we're pinned if the piece is queen/rook
                condition = p => p.PieceType is PieceType.Rook or PieceType.Queen;
                attackSquares = pinned.File < to.File
                    ? SquareExtensions.SquaresInBetween(squaresExcept.MinBy(x => x.File)!, pinned, true)
                        .OrderByDescending(x => x.File)
                    : SquareExtensions.SquaresInBetween(squaresExcept.MaxBy(x => x.File)!, pinned, true)
                        .OrderBy(x => x.File);

            }
            else if (pinned.File == to.File)
            {
                // in horizontal/vertical line, we're pinned if the piece is queen/rook
                condition = p => p.PieceType is PieceType.Rook or PieceType.Queen;
                attackSquares = pinned.Rank < to.Rank
                    ? SquareExtensions.SquaresInBetween(squaresExcept.MinBy(x => x.Rank)!, pinned, true).OrderByDescending(x => x.Rank)
                    : SquareExtensions.SquaresInBetween(squaresExcept.MaxBy(x => x.Rank)!, pinned, true).OrderBy(x => x.Rank);
            }
            else if (Math.Abs(pinned.File - to.File) == Math.Abs(pinned.Rank - to.Rank))
            {
                // in diagonal line, we're pinned if the piece is queen/bishop
                condition = p => p.PieceType is PieceType.Bishop or PieceType.Queen;
                attackSquares = pinned.Rank < to.Rank
                    ? SquareExtensions.SquaresInBetween(squaresExcept.MinBy(x => x.Rank)!, pinned, true).OrderByDescending(x => x.Rank)
                    : SquareExtensions.SquaresInBetween(squaresExcept.MaxBy(x => x.Rank)!, pinned, true).OrderBy(x => x.Rank);
            }


            // we've reordered the list to start from possibly pinned pieces to the edge of the board.
            // so we can just loop it till we find a piece.
            foreach (var square in attackSquares)
            {
                if (boardSetup[square] is not { } p) continue;

                // 1. There is another piece shielding from pins 
                // 2. if the squares are in horizontal/vertical line, we're pinned if the piece is queen/rook
                //    if the squares are in diagonal line, we're pinned if the piece is queen/bishop
                return p.Color == attacker && condition(p);
            }

            // there are no pieces on squares that can be pinned.
            return false;
        }

        /// <summary>
        /// Get kings square
        /// </summary>
        /// <param name="boardSetup"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Square GetKingPosition(this BoardSetup boardSetup, Color color)
        {
            var kingSquare = Square.Invalid;

            foreach (var square in Square.AsEnumerable())
            {
                if (boardSetup[square] is not { PieceType: PieceType.King } p || p.Color != color) continue;
                kingSquare = square;
                break;
            }

            if (kingSquare == Square.Invalid)
            {
                throw new Exception("King not found");
            }

            return kingSquare;
        }

        /// <summary>
        /// Invert color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color Invert(this Color color)
        {
            return color == Color.White ? Color.Black : Color.White;
        }

        /// <summary>
        /// Get Active color from fen
        /// </summary>
        /// <param name="fen"></param>
        /// <returns></returns>
        public static Color GetActiveColor(string fen)
        {
            var parts = fen.Split(' ');

            return parts[1] == "w" ? Color.White : Color.Black;
        }
    }
}

