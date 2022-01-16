using OpenPGN;
using OpenPGN.Models;
using ReactiveUI;
using StockFischer.Engine;
using StockFischer.Models.BoardElements.Pieces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using UIColors = System.Windows.Media.Colors;

namespace StockFischer.Models;

/// <summary>
/// This class is used to store the current state of the board
/// Also exposes methods to make legal moves on the board
/// Also bound to <see cref="Board.LiveBoard"/> to get inputs from UI
/// </summary>
public class LiveBoard : ReactiveObject
{
    private LivePiece _selectedPiece;

    /// <summary>
    /// Stores the current state of the board, ie the location of all pieces
    /// Whether or not either side can castle, active player color, number of moves played etc
    /// <see>
    ///     <cref>BoardSetup.ToString</cref>
    /// </see>
    /// will give FEN of position stored (https://en.wikipedia.org/wiki/Forsyth–Edwards_Notation).
    /// </summary>
    public BoardSetup BoardSetup { get; private set; }

    /// <summary>
    /// All the moves played in the game
    /// </summary>
    public MoveCollection Moves { get; } = new();

    /// <summary>
    /// Option to Enable/Disable showing legal moves for selected piece in UI
    /// </summary>
    public bool ShowLegalMoves { get; set; } = true;

    /// <summary>
    /// Active Player Color
    /// </summary>
    public Color ActiveColor { get; private set; } = Color.White;

    /// <summary>
    /// Selected Piece, Updated when active player clicks one of his pieces
    /// Needed when used with UI
    /// </summary>
    private LivePiece SelectedPiece
    {
        get => _selectedPiece;
        set
        {
            if (_selectedPiece == value)
            {
                return;
            }

            _selectedPiece = value;

            if (_selectedPiece is null)
            {
                Clear<SquareHighlight>();
            }
            else
            {
                AddLegalMovesHint(_selectedPiece);
            }
        }
    }

    /// <summary>
    /// Default constructor
    /// Initializes <see cref="BoardSetup"/> with starting position
    /// </summary>
    private LiveBoard() : this(BoardSetup.NewGame())
    {
    }

    /// <summary>
    /// Constructor to initialize board with a custom position
    /// </summary>
    /// <param name="boardSetup">Custom position</param>
    private LiveBoard(BoardSetup boardSetup)
    {
        Load(boardSetup);
    }

    /// <summary>
    /// Function to quickly load a position on the current instance without creating a new object
    /// this is used when rewinding moves
    /// </summary>
    /// <param name="boardSetup">Position to load</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Load(BoardSetup boardSetup)
    {
        Elements.Clear();

        BoardSetup = boardSetup;

        foreach (var square in Square.AsEnumerable())
        {
            if (boardSetup[square] is { } p)
            {
                Elements.Add(LivePiece.GetPiece(p, square));
            }
        }

        ActiveColor = boardSetup.IsWhiteMove ? Color.White : Color.Black;

        UpdateCheckElements(ActiveColor);

        this.RaisePropertyChanged(nameof(BoardSetup));
    }

    /// <summary>
    /// Update UI if there are checks in the current position, only used when loading position from fen
    /// </summary>
    /// <param name="color"></param>
    private void UpdateCheckElements(Color color)
    {
        var king = GetKing(color);

        if (BoardSetup.GetGameState(ActiveColor) == GameState.Check)
            Elements.Add(new Check(king.Square));
        else if (BoardSetup.GetGameState(ActiveColor) == GameState.DoubleCheck)
            Elements.Add(new Check(king.Square));
        else if (BoardSetup.GetGameState(ActiveColor) == GameState.Checkmate)
            Elements.Add(new Checkmate(king.Square));
    }

    /// <summary>
    /// Models for UI Elements that is drawn on top of the chess board
    /// This can be
    /// 1. Chess Pieces <see cref="LivePiece"/>
    /// 2. Highlights to show legal moves <see cref="SquareHighlight"/> <see cref="SquareHighlight2"/>
    /// 3. Highlights to show game state <see cref="Check"/> <see cref="Checkmate"/>
    /// 4. Arrows to indicate moves
    /// 5. Move Annotations
    /// </summary>
    public ObservableCollection<ILiveBoardElement> Elements { get; } = new();

    /// <summary>
    /// Helper function to create a new instance from FEN
    /// </summary>
    /// <param name="fen">FEN (https://en.wikipedia.org/wiki/Forsyth–Edwards_Notation).</param>
    /// <returns></returns>
    public static LiveBoard FromFen(string fen)
    {
        return new LiveBoard(BoardSetup.FromFen(fen));
    }

    /// <summary>
    /// Helper function to create a new instance from starting position
    /// </summary>
    /// <returns></returns>
    public static LiveBoard NewGame() => new();

    /// <summary>
    /// Convert moves in Long Algebraic notation to Algebraic notation.
    /// For this we need to create a brand new board, and play the moves.
    /// </summary>
    /// <param name="startpos"></param>
    /// <param name="moves"></param>
    /// <returns></returns>
    public static IEnumerable<MoveModel> ConvertMovesToAlgebraic(string startpos, IEnumerable<EngineMove> moves)
    {
        var board = FromFen(startpos);

        foreach (var move in moves)
        {
            board.TryMakeMove(move.From, move.To);
        }

        return board.GetMovesPlayed();
    }

    /// <summary>
    /// Get all the moves played as plys.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MoveModel> GetMovesPlayed()
    {
        foreach (var move in Moves)
        {
            if(move.White is { })
                yield return move.White;

            if (move.Black is { })
                yield return move.Black;
        }
    }

    /// <summary>
    /// Create an instance from a pgn file
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static LiveBoard FromPgnFile(string file) => FromGame(Game.FromPgnFile(file));

    public static LiveBoard FromGame(Game game)
    {
        var board = NewGame();
        foreach (var item in game.MoveText)
        {
            switch (item)
            {
                case MovePairEntry mpe:
                    board.PlayMove(mpe.White, Color.White);
                    board.PlayMove(mpe.Black, Color.Black);
                    break;
                case HalfMoveEntry hme:
                    board.PlayMove(hme.Move, hme.IsContinued ? Color.Black : Color.White);
                    break;
            }
        }
        board.GoToMove(board.Moves.Current);
        return board;
    }

    /// <summary>
    /// Set the board position before <paramref name="move"/> was played
    /// <paramref name="move"/> should be inside <see cref="Moves"/>
    /// </summary>
    /// <param name="move">move</param>
    public void GoToMove(MoveModel move)
    {
        Load(BoardSetup.FromFen(move.Fen));
    }

    /// <summary>
    /// Go to starting position of the game
    /// </summary>
    public void GoToStart()
    {
        if (!Moves.GoToStart()) return;
        GoToMove(Moves.Current);
    }

    /// <summary>
    /// Go to the final position of the game.
    /// </summary>
    public void GoToEnd()
    {
        if (!Moves.GoToEnd()) return;
        GoToMove(Moves.Current);
    }

    /// <summary>
    /// Undo the current move.
    /// </summary>
    public void GoBack()
    {
        var model = Moves.Current;

        if (Moves.GoBack() == false) return;

        Clear<Check>();

        if(model.Move.PromotedPiece is PieceType)
        {
            var pawn = new Piece(PieceType.Pawn, model.Color);
            BoardSetup[model.OriginSquare] = pawn;
            Elements.Remove(GetElement<LivePiece>(x => x.Square == model.TargetSquare));
            Elements.Add(LivePiece.GetPiece(pawn, model.TargetSquare));
        }
        else
        {
            BoardSetup[model.OriginSquare] = model.LivePiece.Piece;
        }

        GetElement<LivePiece>(x => x.Square == model.TargetSquare).Move(model.OriginSquare);

        switch (model.Move.Type)
        {
            case MoveType.Simple:
            case MoveType.DoublePawnMove:
                BoardSetup[model.TargetSquare] = null;
                break;
            case MoveType.Capture:
                BoardSetup[model.TargetSquare] = model.CapturedPiece.Piece;
                Elements.Add(LivePiece.GetPiece(model.CapturedPiece.Piece, model.TargetSquare));
                break;
            case MoveType.CaptureEnPassant:
                var capturedPieceSquare = model.TargetSquare.Down(model.Color);
                BoardSetup[capturedPieceSquare] = model.CapturedPiece.Piece;
                Elements.Add(LivePiece.GetPiece(model.CapturedPiece.Piece, capturedPieceSquare));
                break;
            case MoveType.CastleKingSide:
                var kingRookSquare = model.Color == Color.White ? Square.F1 : Square.F8;
                var kingRookOrigin = model.Color == Color.White ? Square.H1 : Square.H8;
                BoardSetup[kingRookSquare] = null;
                GetElement<LivePiece>(x => x.Square == kingRookSquare).Move(kingRookOrigin);
                break;
            case MoveType.CastleQueenSide:
                var queenRookSquare = model.Color == Color.White ? Square.D1 : Square.D8;
                var queenRookOrigin = model.Color == Color.White ? Square.A1 : Square.A8;
                BoardSetup[queenRookSquare] = null;
                GetElement<LivePiece>(x => x.Square == queenRookSquare).Move(queenRookOrigin);
                break;
        }

        UpdateCheckElements(model.Color.Invert());
    }

    /// <summary>
    /// Redo the last undone move.
    /// </summary>
    /// <returns></returns>
    public bool GoForward()
    {
        if (Moves.GoForward() == false) return false;
        if (Moves.Current is null) return false;

        OnMovePlayed(Moves.Current, true);
        return true;
    }

    /// <summary>
    /// Highlight a square
    /// </summary>
    /// <param name="square"></param>
    public void OnHighlightSquareSelected(Square square)
    {
        if (GetElements<SquareHighlight2>().SingleOrDefault(x => x.Square == square) is { } highlight)
        {
            Elements.Remove(highlight);
        }
        else
        {
            Elements.Add(new SquareHighlight2(square) { Color = UIColors.DarkRed });
        }

        // TODO : implement arrows.
    }

    /// <summary>
    /// Called whenever there is a click on top of the board
    /// Will get called even if is no piece on that square
    /// </summary>
    /// <param name="square">Square on which user clicked</param>
    /// <returns>Returns whether this user interaction cased a move to happen</returns>
    public bool TryMakeMove(Square square)
    {
        MoveModel moveModel = null;

        if (SelectedPiece is null)
        {
            // if the there is a piece on the clicked square, which is the same color as active player
            // and we don't have a selected piece yet, then make this as selected piece and show legal moves
            if (BoardSetup[square] is { } piece && piece.Color == ActiveColor)
            {
                SelectedPiece = Elements.OfType<LivePiece>().Single(x => x.Square == square);
                return false;
            }

            Clear<SquareHighlight>();

            return false;
        }
        else
        {
            // If we have a selected piece, and the clicked square also contains a piece
            if (BoardSetup[square] is { } piece)
            {
                // if it is the same color as active player Update the selected piece and show legal moves
                if (piece.Color == ActiveColor)
                {
                    SelectedPiece = Elements.OfType<LivePiece>().Single(x => x.Square == square);
                    return false;
                }

                // make sure the the selected piece could actually move to that square
                if (!SelectedPiece.GetLegalMoves(BoardSetup).Contains(square))
                {
                    return false;
                }

                // If its a pawn move to a last rank, then we are promoting with a capture
                if (SelectedPiece.Type == PieceType.Pawn && square.Rank is 1 or 8)
                {
                    // TODO : need to give choice on promoted piece
                    moveModel = Promote(SelectedPiece, square, PieceType.Queen, true);
                }
                // Otherwise it's a simple capture.
                else
                {
                    moveModel = Capture(SelectedPiece, square);
                }
            }
            // Otherwise it's not a capture
            else
            {
                // make sure the the selected piece could actually move to that square
                if (!SelectedPiece.GetLegalMoves(BoardSetup).Contains(square))
                {
                    Clear<SquareHighlight>();
                    return false;
                }

                // Check if it's a castling move (https://en.wikipedia.org/wiki/Castling).
                if (SelectedPiece is King king &&
                    (king.CanCastleKingSide || king.CanCastleQueenSide) &&
                    (square == king.KingSideCastleSquare || square == king.QueenSideCastleSquare) &&
                    (king.Square == Square.E1 || king.Square == Square.E8))
                {
                    BoardSetup.EnPassantSquare = null;
                    moveModel = Castle(SelectedPiece, square);
                }
                // Check if it's an En passant (https://en.wikipedia.org/wiki/En_passant).
                else if (SelectedPiece.Type == PieceType.Pawn &&
                         BoardSetup.EnPassantSquare == square)
                {
                    moveModel = CaptureEnPassant(SelectedPiece, square);
                    BoardSetup.EnPassantSquare = null;
                }
                // Check if it's a promotion (https://en.wikipedia.org/wiki/Promotion_(chess)).
                else if (SelectedPiece.Type == PieceType.Pawn && square.Rank is 1 or 8)
                {
                    BoardSetup.EnPassantSquare = null;

                    // TODO : need to give choice on promoted piece
                    moveModel = Promote(SelectedPiece, square);
                }
                // Otherwise move the piece to the square
                else
                {
                    BoardSetup.EnPassantSquare = null;
                    moveModel = SimpleMove(SelectedPiece, square);
                }
            }

            OnMovePlayed(moveModel);

            return true;
        }
    }

    /// <summary>
    /// Api to make a move without GUI
    /// </summary>
    /// <param name="from">origin square of piece to move.</param>
    /// <param name="to">target square of piece to move.</param>
    /// <returns>whether a move was made successfully.</returns>
    public bool TryMakeMove(Square from, Square to)
    {
        if (from == null || to == null) return false;

        if (BoardSetup[from] is null) return false;

        SelectedPiece = GetElements<LivePiece>().Single(x => x.Square == from);

        return TryMakeMove(to);
    }

    /// <summary>
    /// Make a simple move, ( Moves a piece from one square to another )
    /// </summary>
    /// <param name="piece">Piece to move</param>
    /// <param name="square">Target square</param>
    private MoveModel SimpleMove(LivePiece piece, Square square)
    {
        var move = new Move
        {
            Piece = piece.Type,
            TargetSquare = square,
            Type = MoveType.Simple,
        };

        if (piece.Type == PieceType.Pawn && Math.Abs(piece.Square.Rank - square.Rank) == 2)
        {
            move.Type = MoveType.DoublePawnMove;
        }

        return new MoveModel(move)
        {
            LivePiece = piece,
            TargetSquare = square,
            OriginSquare = piece.Square
        };
    }


    /// <summary>
    /// Performs a capture
    /// A piece moves from a square to another square which contains a piece of the opposite color.
    /// That piece is removed from the board.
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="square"></param>
    private MoveModel Capture(LivePiece piece, Square square)
    {
        var move = new Move
        {
            Piece = piece.Type,
            TargetSquare = square,
            Type = MoveType.Capture,
        };

        if (piece.Type == PieceType.Pawn)
        {
            move.OriginFile = piece.Square.File;
        }

        return new MoveModel(move)
        {
            LivePiece = piece,
            TargetSquare = square,
            OriginSquare = piece.Square,
            CapturedPiece = GetElement<LivePiece>(x => x.Square == square)
        };
    }

    /// <summary>
    /// En Passant Capture (https://en.wikipedia.org/wiki/En_passant).
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="square"></param>
    private MoveModel CaptureEnPassant(LivePiece piece, Square square)
    {
        var move = new Move
        {
            Piece = piece.Type,
            TargetSquare = square,
            Type = MoveType.CaptureEnPassant,
            OriginFile = piece.Square.File
        };

        return new MoveModel(move)
        {
            LivePiece = piece,
            TargetSquare = square,
            OriginSquare = piece.Square,
            CapturedPiece = GetElement<LivePiece>(x => x.Square == square.Down(piece.Color))
        };
    }

    /// <summary>
    /// Pawn Promotion (https://en.wikipedia.org/wiki/Promotion_(chess)).
    /// </summary>
    /// <param name="piece">piece</param>
    /// <param name="square">target square</param>
    /// <param name="type">promoted piece type</param>
    /// <param name="isCapture">If there was a capture to reach the promotion square</param>
    private MoveModel Promote(LivePiece piece, Square square, PieceType type = PieceType.Queen, bool isCapture = false)
    {
        var move = new Move
        {
            Piece = piece.Type,
            TargetSquare = square,
            PromotedPiece = type,
            Type = isCapture ? MoveType.Capture : MoveType.Simple,
            OriginFile = piece.Square.File,
        };

        return new MoveModel(move)
        {
            LivePiece = piece,
            TargetSquare = square,
            OriginSquare = piece.Square,
            CapturedPiece = move.Type == MoveType.Capture ? GetElement<LivePiece>(x => x.Square == square) : null
        };
    }

    /// <summary>
    /// Castle // Check if it's a castling move (https://en.wikipedia.org/wiki/Castling).
    /// </summary>
    /// <param name="piece">King</param>
    /// <param name="square">G1/C1 if white, G8/C8 if black</param>
    private MoveModel Castle(LivePiece piece, Square square)
    {
        var type = (square == Square.G1 || square == Square.G8)
            ? MoveType.CastleKingSide
            : MoveType.CastleQueenSide;

        var move = new Move
        {
            TargetSquare = square,
            Type = type
        };

        return new MoveModel(move)
        {
            LivePiece = piece,
            TargetSquare = square,
            OriginSquare = piece.Square
        };
    }

    /// <summary>
    /// Show legal moves in chess board.
    /// </summary>
    /// <param name="piece"></param>
    private void AddLegalMovesHint(IPiece piece)
    {
        if (ShowLegalMoves == false)
        {
            return;
        }

        Clear<SquareHighlight>();

        var candidates = piece.GetLegalMoves(BoardSetup);

        foreach (var candidate in candidates)
        {
            Elements.Add(BoardSetup[candidate] is not null
                ? new SquareHighlight2(candidate)
                : new SquareHighlight(candidate));
        }
    }

    /// <summary>
    /// Update <see cref="Elements"/> and <see cref="BoardSetup"/> for the give move
    /// </summary>
    /// <param name="moveModel">Move played</param>
    private void UpdateMoveOnBoard(MoveModel moveModel)
    {
        var piece = GetElement<LivePiece>(x => x.Square == moveModel.OriginSquare);
        var color = piece.Color;
        Piece promotedPiece = null;

        // Update board setup
        BoardSetup[moveModel.TargetSquare] = piece.Piece;
        BoardSetup[piece.Square] = null;

        // Reset half-move clock if it's a pawn move or capture
        BoardSetup.HalfMoveClock = (moveModel.Move.Type == MoveType.Capture || piece.Type == PieceType.Pawn)
            ? 0
            : BoardSetup.HalfMoveClock + 1;

        if (color == Color.Black)
        {
            BoardSetup.FullMoveCount++;
        }

        if (moveModel.Move.PromotedPiece is { } type)
        {
            BoardSetup[moveModel.TargetSquare] = promotedPiece = new Piece(type, color);
        }

        switch (moveModel.Move.Type)
        {
            case MoveType.CastleKingSide when color == Color.White:
                BoardSetup[Square.H1] = null;
                BoardSetup[Square.F1] = Piece.WhiteRook;
                BoardSetup.CanWhiteCastleKingSide = BoardSetup.CanWhiteCastleQueenSide = false;
                break;
            case MoveType.CastleKingSide:
                BoardSetup[Square.H8] = null;
                BoardSetup[Square.F8] = Piece.BlackRook;
                BoardSetup.CanBlackCastleKingSide = BoardSetup.CanBlackCastleQueenSide = false;
                break;
            case MoveType.CastleQueenSide when color == Color.White:
                BoardSetup[Square.A1] = null;
                BoardSetup[Square.D1] = Piece.WhiteRook;
                BoardSetup.CanWhiteCastleKingSide = BoardSetup.CanWhiteCastleQueenSide = false;
                break;
            case MoveType.CastleQueenSide:
                BoardSetup[Square.A8] = null;
                BoardSetup[Square.D8] = Piece.BlackRook;
                BoardSetup.CanBlackCastleKingSide = BoardSetup.CanBlackCastleQueenSide = false;
                break;
            case MoveType.CaptureEnPassant:
                {
                    var captureSquare = moveModel.TargetSquare.Down(piece.Color);
                    BoardSetup[captureSquare] = null;
                    break;
                }
        }


        // Update UI
        switch (moveModel.Move.Type)
        {
            case MoveType.Capture:
                Elements.Remove(GetElements<LivePiece>().Single(x => x.Square == moveModel.Move.TargetSquare));
                break;
            case MoveType.CaptureEnPassant:
                var captureSquare = moveModel.TargetSquare.Down(piece.Color);
                var capturedPiece = Elements.OfType<LivePiece>().Single(x => x.Square == captureSquare);
                Elements.Remove(capturedPiece);
                break;
            case MoveType.CastleKingSide when color == Color.White:
                GetElement<Rook>(x => x.Square == Square.H1).Move(Square.F1);
                break;
            case MoveType.CastleKingSide:
                GetElement<Rook>(x => x.Square == Square.H8).Move(Square.F8);
                break;
            case MoveType.CastleQueenSide when color == Color.White:
                GetElement<Rook>(x => x.Square == Square.A1).Move(Square.D1);
                break;
            case MoveType.CastleQueenSide:
                GetElement<Rook>(x => x.Square == Square.A8).Move(Square.D8);
                break;
        }

        if (promotedPiece is { } p)
        {
            Elements.Remove(piece);
            Elements.Add(LivePiece.GetPiece(p, moveModel.Move.TargetSquare));
        }
        else
        {
            piece.Move(moveModel.Move.TargetSquare);
        }

        SelectedPiece = null;
    }

    /// <summary>
    /// Helper function to get King 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    private King GetKing(Color color) => GetElements<King>().Single(x => x.Color == color);

    /// <summary>
    /// Helper function to get UI models for specific type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private IEnumerable<T> GetElements<T>() => Elements.OfType<T>();

    /// <summary>
    /// Helper function to get a single UI element that matches the predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private T GetElement<T>(Func<T, bool> predicate) => Elements.OfType<T>().Single(predicate);

    /// <summary>
    /// Remove all elements of given type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void Clear<T>()
        where T : ILiveBoardElement
    {
        var prevSuggestions = Elements.OfType<T>().ToList();
        prevSuggestions.ForEach(x => Elements.Remove(x));
    }
    
    /// <summary>
    /// Remove all the lements of given type statifying the predicate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="condition"></param>
    private void Clear<T>(Func<T, bool> condition)
        where T : ILiveBoardElement
    {
        var prevSuggestions = Elements.OfType<T>().Where(x => condition(x)).ToList();
        prevSuggestions.ForEach(x => Elements.Remove(x));
    }

    /// <summary>
    /// Called after a move is played
    /// This will update <see cref="BoardSetup"/>
    /// Move pieces on UI
    /// Validate game state (check, checkmate etc)
    /// </summary>
    /// <param name="move"></param>
    public void OnMovePlayed(MoveModel move, bool isRewinding = false)
    {
        Clear<Check>();

        if (isRewinding == false)
        {
            ResolveAmbiguousMove(move); 
        }

        UpdateMoveOnBoard(move);

        UpdateGameState(move);

        if (isRewinding == false)
        {
            AddMoveToHistory(move);
        }
    }

    /// <summary>
    /// Save a move made, so we can rewind later
    /// </summary>
    /// <param name="move"></param>
    private void AddMoveToHistory(MoveModel move)
    {
        move.Fen = BoardSetup.ToString();
        Moves.AddMove(move);
    }

    /// <summary>
    /// Update
    /// 1. Whether or not either side can castle
    /// 2. Whether or not either side is in Check/Checkmate
    /// 3. En passant Square
    /// 4. Toggles active color
    /// </summary>
    /// <param name="moveModel"></param>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    private void UpdateGameState(MoveModel moveModel)
    {
        var color = moveModel.Color;
        var otherPlayer = color.Invert();

        UpdateCanCastle(color);
        UpdateCanCastle(otherPlayer);

        var king = GetKing(otherPlayer);
        switch (BoardSetup.GetGameState(otherPlayer))
        {
            case GameState.Check:
                moveModel.Move.IsCheck = true;
                Elements.Add(new Check(king.Square));
                break;
            case GameState.DoubleCheck:
                moveModel.Move.IsDoubleCheck = true;
                Elements.Add(new Check(king.Square));
                break;
            case GameState.Checkmate:
                moveModel.Move.IsCheckMate = true;
                Elements.Add(new Checkmate(king.Square));
                break;
            case GameState.None:
                break;
            default:
                throw new InvalidEnumArgumentException();
        }

        // If it's a double pawn move, update En passant square
        BoardSetup.EnPassantSquare =
            moveModel.Move.Type == MoveType.DoublePawnMove ? moveModel.TargetSquare.Down(color) : null;

        ToggleActiveColor();

        this.RaisePropertyChanged(nameof(BoardSetup));
    }

    /// <summary>
    /// Called after every moves, checks whether the king can castle
    /// </summary>
    /// <param name="color"></param>
    private void UpdateCanCastle(Color color)
    {
        Rook GetKingRook()
        {
            var square = color == Color.White ? Square.H1 : Square.H8;
            return GetElements<Rook>().Where(x => x.Color == color).SingleOrDefault(x => x.Square == square);
        }
        Rook GetQueenRook()
        {
            var square = color == Color.White ? Square.A1 : Square.A8;
            return GetElements<Rook>().Where(x => x.Color == color).SingleOrDefault(x => x.Square == square);
        }
        bool CanCastleKingSide(King king, Rook kingRook)
        {
            if (king.HasMoved || kingRook is null || kingRook.HasMoved)
            {
                if (king.Color == Color.White)
                {
                    BoardSetup.CanWhiteCastleKingSide = false;
                }
                else
                {
                    BoardSetup.CanBlackCastleKingSide = false;
                }

                return false;
            }

            Square s1, s2;
            if (king.Color == Color.White)
            {
                s1 = Square.F1;
                s2 = Square.G1;
            }
            else
            {
                s1 = Square.F8;
                s2 = Square.G8;
            }

            return BoardSetup[s1] is null && BoardSetup[s2] is null &&
                   !BoardSetup.IsAttacked(s1, king.Color.Invert()) &&
                   !BoardSetup.IsAttacked(s2, king.Color.Invert());
        }
        bool CanCastleQueenSide(King king, Rook queenRook)
        {
            if (king.HasMoved || queenRook is null || queenRook.HasMoved)
            {
                if (king.Color == Color.White)
                {
                    BoardSetup.CanWhiteCastleQueenSide = false;
                }
                else
                {
                    BoardSetup.CanBlackCastleQueenSide = false;
                }

                return false;
            }

            Square s1, s2, s3;
            if (king.Color == Color.White)
            {
                s1 = Square.B1;
                s2 = Square.C1;
                s3 = Square.D1;
            }
            else
            {
                s1 = Square.B8;
                s2 = Square.C8;
                s3 = Square.D8;
            }

            return BoardSetup[s1] is null && BoardSetup[s2] is null && BoardSetup[s3] is null &&
                   !BoardSetup.IsAttacked(s2, king.Color.Invert()) &&
                   !BoardSetup.IsAttacked(s3, king.Color.Invert());
        }

        var king = GetKing(color);
        var kingRook = GetKingRook();
        var queenRook = GetQueenRook();

        king.CanCastleKingSide = CanCastleKingSide(king, kingRook);
        king.CanCastleQueenSide = CanCastleQueenSide(king, queenRook);
    }

    /// <summary>
    /// Toggle active color
    /// </summary>
    private void ToggleActiveColor()
    {
        ActiveColor = ActiveColor.Invert();
        BoardSetup.IsWhiteMove = !BoardSetup.IsWhiteMove;
    }

    /// <summary>
    /// The <see cref="Move"/> created initially would not contain <see cref="Move.OriginSquare"/> or
    /// <see cref="Move.OriginRank"/> or <see cref="Move.OriginFile"/> as they are are need for most of the moves,
    /// When two or more pieces of the same type can move to the same square, these are required to identify
    /// which piece should be moved.
    /// </summary>
    /// <param name="moveModel"></param>
    private void ResolveAmbiguousMove(MoveModel moveModel)
    {
        if (moveModel.PieceType is PieceType.Pawn or PieceType.King)
        {
            return;
        }

        var piece = moveModel.LivePiece;

        var otherPieces = Elements.OfType<LivePiece>().Where(x =>
            x.Type == piece.Type && x.Color == piece.Color && x.Square != piece.Square);
        foreach (var otherPiece in otherPieces)
        {
            if (!otherPiece.GetLegalMoves(BoardSetup).Contains(moveModel.Move.TargetSquare)) continue;

            if (piece.Square.Rank == otherPiece.Square.Rank)
            {
                moveModel.Move.OriginFile = piece.Square.File;
            }
            else if (piece.Square.File == otherPiece.Square.File)
            {
                moveModel.Move.OriginRank = piece.Square.Rank;
            }
            else
            {
                moveModel.Move.OriginFile = piece.Square.File;
            }
        }

        if (moveModel.Move.OriginRank is not null && moveModel.Move.OriginFile is not null)
        {
            moveModel.Move.OriginSquare =
                Square.New(moveModel.Move.OriginFile.Value, moveModel.Move.OriginRank.Value);
        }
    }

    /// <summary>
    /// Plays a move on the board, does not do any validations on the moves, assumes that every thing is correct
    /// and move is playable.
    /// </summary>
    /// <param name="move"></param>
    /// <param name="color"></param>
    private void PlayMove(Move move, Color color)
    {
        var model = new MoveModel(move)
        {
            TargetSquare = move.TargetSquare,
        };

        var pieces = GetElements<LivePiece>()
            .Where(x => x.Color == color)
            .Where(x => x.Type == move.Piece)
            .Where(x => x.GetLegalMoves(BoardSetup).Contains(move.TargetSquare))
            .ToList();


        LivePiece piece = null;

        if (move.Type is MoveType.CastleKingSide or MoveType.CastleQueenSide)
        {
            var king = GetKing(color);
            piece = king;
            model.TargetSquare = move.TargetSquare = move.Type == MoveType.CastleKingSide ? king.KingSideCastleSquare : king.QueenSideCastleSquare;
        }
        else if (pieces.Count > 1)
        {
            if (move.OriginSquare is not null)
            {
                piece = pieces.Single(x => x.Square == move.OriginSquare);
            }
            else if (move.OriginRank is not null)
            {
                piece = pieces.Single(x => x.Square.Rank == move.OriginRank);
            }
            else if (move.OriginFile is not null)
            {
                piece = pieces.Single(x => x.Square.File == move.OriginFile);
            }
        }
        else
        {
            piece = pieces.Single();
        }

        model.LivePiece = piece;
        model.OriginSquare = piece!.Square;
        model.CapturedPiece = move.Type switch
        {
            MoveType.Capture => GetElement<LivePiece>(x => x.Square == move.TargetSquare),
            MoveType.CaptureEnPassant => GetElement<LivePiece>(x => x.Square == move.TargetSquare!.Down(model.Color)),
            _ => model.CapturedPiece
        };

        OnMovePlayed(model);
    }
}