using StockFischer.Models.BoardElements.Pieces;
using OpenPGN;
using OpenPGN.Models;
using OpenPGN.Utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace StockFischer.Models
{
    public abstract class LivePiece : ReactiveObject, IPiece
    {
        public Piece Piece { get; set; }
        public PieceType Type { get; set; }
        public Color Color { get; set; }
        public Square Square { get; private set; }
        public string Image { get; set; }
        protected LivePiece(Piece piece, Square square)
        {
            Piece = piece;
            Type = piece.PieceType;
            Color = piece.Color;
            Square = square;
            Image = $"pack://application:,,,/StockFischer;component/Assets/Themes/Default/{piece.Color.ToString().ToLower()}_{piece.PieceType.ToString().ToLower()}.png";
        }

        public static LivePiece GetPiece(Piece piece, Square square)
        {
            return piece.PieceType switch
            {
                PieceType.Pawn => piece.Color == Color.White ? WhitePawn(square) : BlackPawn(square),
                PieceType.Knight => piece.Color == Color.White ? WhiteKnight(square) : BlackKnight(square),
                PieceType.Bishop => piece.Color == Color.White ? WhiteBishop(square) : BlackBishop(square),
                PieceType.Rook => piece.Color == Color.White ? WhiteRook(square) : BlackRook(square),
                PieceType.Queen => piece.Color == Color.White ? WhiteQueen(square) : BlackQueen(square),
                PieceType.King => piece.Color == Color.White ? WhiteKing(square) : BlackKing(square),
                _ => throw new InvalidEnumArgumentException()
            };
        }

        public static LivePiece WhitePawn(Square square) => new Pawn(Piece.WhitePawn, square);
        public static LivePiece WhiteKnight(Square square) => new Knight(Piece.WhiteKnight, square);
        public static LivePiece WhiteBishop(Square square) => new Bishop(Piece.WhiteBishop, square);
        public static LivePiece WhiteRook(Square square) => new Rook(Piece.WhiteRook, square);
        public static LivePiece WhiteQueen(Square square) => new Queen(Piece.WhiteQueen, square);
        public static LivePiece WhiteKing(Square square) => new King(Piece.WhiteKing, square);

        public static LivePiece BlackPawn(Square square) => new Pawn(Piece.BlackPawn, square);
        public static LivePiece BlackKnight(Square square) => new Knight(Piece.BlackKnight, square);
        public static LivePiece BlackBishop(Square square) => new Bishop(Piece.BlackBishop, square);
        public static LivePiece BlackRook(Square square) => new Rook(Piece.BlackRook, square);
        public static LivePiece BlackQueen(Square square) => new Queen(Piece.BlackQueen, square);
        public static LivePiece BlackKing(Square square) => new King(Piece.BlackKing, square);

        public virtual IEnumerable<Square> GetPossibleMoves(BoardSetup boardSetup)
        {
            return Enumerable.Empty<Square>();
        }

        public virtual IEnumerable<Square> GetLegalMoves(BoardSetup boardSetup)
        {
            var candidates = GetPossibleMoves(boardSetup);

            Square king = boardSetup.GetKingPosition(Color);

            //Is pinned?
            if (boardSetup.IsSquarePinned(Square, king, Color.Invert()))
            {
                return SquareExtensions.SquaresInLine(Square, king).Intersect(candidates);
            }

            switch(boardSetup.GetGameState(Color))
            {
                case GameState.DoubleCheck:
                case GameState.CheckMate:
                    return Enumerable.Empty<Square>();
                case GameState.Check:
                    Attack attack = boardSetup.IsAttacked(king, Color.Invert()).Attacks.Single();
                    var blockableSquares = SquareExtensions.SquaresInBetween(king, attack.Square).ToList();
                    blockableSquares.Add(attack.Square);
                    return blockableSquares.Where(x => candidates.Contains(x)).ToList();
                default:
                    return candidates;

            }
        }

        public virtual void Move(Square square)
        {
            Square = square;
            this.RaisePropertyChanged(nameof(Square));
        }

    }
}
