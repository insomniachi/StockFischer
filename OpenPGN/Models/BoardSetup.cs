using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenPGN.Models;

/// <summary>
/// Represents a board setup. This is used in position practices or non-standard chess variants like Chess960
/// </summary>
public class BoardSetup
{
    public const string StartingPosition = @"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    private readonly Piece[,] _board = new Piece[8, 8];

    /// <summary>
    /// Gets or sets a <see cref="Piece"/> on the specified square (defined as integers 0..7).
    /// </summary>
    /// <value>
    /// The <see cref="Piece"/>. Use <c>null</c> to unset.
    /// </value>
    /// <param name="file">The file.</param>
    /// <param name="rank">The rank.</param>
    /// <returns>The piece at the specified square or <c>null</c> if the square is empty.</returns>
    private Piece? this[int file, int rank]
    {
        get => _board[file, rank];
        set => _board[file, rank] = value!;
    }

    /// <summary>
    /// Gets or sets a <see cref="Piece"/> on the specified square (via file and rank).
    /// </summary>
    /// <value>
    /// The <see cref="Piece"/>. Use <c>null</c> to unset.
    /// </value>
    /// <param name="file">The file.</param>
    /// <param name="rank">The rank.</param>
    /// <returns>The piece at the specified square or <c>null</c> if the square is empty.</returns>
    private Piece? this[File file, int rank]
    {
        get => this[file.ToInt(true), rank - 1];
        set => this[file.ToInt(true), rank - 1] = value;
    }

    /// <summary>
    /// Gets or sets a <see cref="Piece" /> on the specified square.
    /// </summary>
    /// <value>
    /// The <see cref="Piece" />. Use <c>null</c> to unset.
    /// </value>
    /// <param name="square">The square.</param>
    /// <returns>
    /// The piece at the specified square or <c>null</c> if the square is empty.
    /// </returns>
    public Piece? this[Square square]
    {
        get => this[square.File, square.Rank];
        set => this[square.File, square.Rank] = value!;
    }

    /// <summary>
    /// Gets or sets the <see cref="Piece"/> at the specified position. Counting starts at A1 rank-wise. 0=A1, 1=B1, ..., 7=H1, 8=A2, ..., 64=H8
    /// </summary>
    /// <value>
    /// The <see cref="Piece"/>. Use <c>null</c> to unset.
    /// </value>
    /// <param name="pos">The position.</param>
    /// <returns>The piece at the specified square or <c>null</c> if the square is empty.</returns>
    public Piece? this[int pos]
    {
        get => this[pos % 8, pos / 8];
        set => this[pos % 8, pos / 8] = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether it is whites move (true) or blacks (false).
    /// </summary>
    /// <value>
    /// <c>true</c> if white is to move next; otherwise, <c>false</c>.
    /// </value>
    public bool IsWhiteMove { get; set; } = true;

    /// <summary>
    /// Indicates whether white can castle king-side.
    /// </summary>
    /// <value>
    /// <c>true</c> if white can castle king-side; otherwise, <c>false</c>.
    /// </value>
    public bool CanWhiteCastleKingSide { get; set; }

    /// <summary>
    /// Indicates whether white can castle queen-side.
    /// </summary>
    /// <value>
    /// <c>true</c> if white can castle queen-side; otherwise, <c>false</c>.
    /// </value>
    public bool CanWhiteCastleQueenSide { get; set; }

    /// <summary>
    /// Indicates whether black can castle king-side.
    /// </summary>
    /// <value>
    /// <c>true</c> if black can castle king-side; otherwise, <c>false</c>.
    /// </value>
    public bool CanBlackCastleKingSide { get; set; }

    /// <summary>
    /// Indicates whether black can castle queen-side.
    /// </summary>
    /// <value>
    /// <c>true</c> if black can castle queen-side; otherwise, <c>false</c>.
    /// </value>
    public bool CanBlackCastleQueenSide { get; set; }

    /// <summary>
    /// The en passant target square (if present). 
    /// </summary>
    /// <value>
    /// The en passant square or <c>null</c> if not en passant move is possible.
    /// </value>
    public Square? EnPassantSquare { get; set; }

    /// <summary>
    /// Gets or sets the half move clock. 
    /// It is a nonnegative integer representing the half-move clock. This number is the count of half-moves (or ply) 
    /// since the last pawn advance or capturing move. This value is used for the fifty move draw rule.
    /// </summary>
    /// <value>
    /// The half move clock.
    /// </value>
    public int HalfMoveClock { get; set; }

    /// <summary>
    /// A positive integer that gives the full-move number. This will have the value "1" for the first move of a game for both White and Black. It is incremented by one immediately after each move by Black.
    /// </summary>
    /// <value>
    /// The full move count.
    /// </value>
    public int FullMoveCount { get; set; }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents the board setup.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents the board setup.
    /// </returns>
    public override string ToString()
    {
        StringBuilder sb = new();

        for (int i = 7; i >= 0; --i)
        {
            for (int j = 0; j < 8; ++j)
                sb.Append(PieceToString(this[j, i]));

            if (i != 0)
            {
                sb.Append('/');
            }
        }

        var pos = Regex.Replace(sb.ToString(), "x+", m => m.Value.Length.ToString());
        var activeColor = IsWhiteMove ? 'w' : 'b';
        var castle = CanWhiteCastleKingSide == false && CanBlackCastleKingSide == false && CanWhiteCastleQueenSide == false && CanBlackCastleQueenSide == false
            ? "-"
            : $"{(CanWhiteCastleKingSide ? "K" : string.Empty)}{(CanWhiteCastleQueenSide ? "Q" : string.Empty)}{(CanBlackCastleKingSide ? "k" : string.Empty)}{(CanBlackCastleQueenSide ? "q" : string.Empty)}";

        var ep = EnPassantSquare is { } square
            ? square.ToString()
            : "-";

        return $"{pos} {activeColor} {castle} {ep} {HalfMoveClock} {FullMoveCount}";
    }

    private string PieceToString(Piece? p)
    {
        if (p == null) return "x";

        var str = p.PieceType switch
        {
            PieceType.Pawn => "p",
            PieceType.Knight => "n",
            PieceType.Bishop => "b",
            PieceType.Rook => "r",
            PieceType.Queen => "q",
            PieceType.King => "k",
            _ => throw new InvalidEnumArgumentException()
        };

        return p.Color == Color.White ? str.ToUpper() : str;
    }

    private static Piece CharToPiece(char piece)
    {
        var color = char.IsUpper(piece) ? Color.White : Color.Black;

        var type = char.ToLower(piece) switch
        {
            'p' => PieceType.Pawn,
            'n' => PieceType.Knight,
            'b' => PieceType.Bishop,
            'r' => PieceType.Rook,
            'q' => PieceType.Queen,
            'k' => PieceType.King,
            _ => throw new ArgumentException(null, nameof(piece))
        };

        return new Piece(type, color);
    }

    public static BoardSetup NewGame() => FromFen(StartingPosition);

    public static BoardSetup FromFen(string fen)
    {
        var parts = fen.Split(' ');

        var processedFen = Regex.Replace(parts[0], @"\d", m => new string('x', int.Parse(m.Value)));

        var rows = processedFen.Split('/');

        if(rows.Length != 8)
        {
            throw new ArgumentException("Fen does not contains 8 rows", nameof(fen));
        }

        var boardSetup = new BoardSetup();

        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                var c = rows[i][j];

                if (c == 'x') continue;
                
                var square = Square.New(FileExtensions.FromInt(j, true), 8 - i);

                boardSetup[square] = CharToPiece(c);
            }
        }

        boardSetup.IsWhiteMove = parts[1] == "w";
        boardSetup.CanWhiteCastleKingSide = parts[2].Contains('K');
        boardSetup.CanWhiteCastleQueenSide = parts[2].Contains('Q');
        boardSetup.CanBlackCastleKingSide = parts[2].Contains('k');
        boardSetup.CanBlackCastleQueenSide = parts[2].Contains('q');
        boardSetup.EnPassantSquare = Square.Parse(parts[3]);
        boardSetup.HalfMoveClock = int.Parse(parts[4]);
        boardSetup.FullMoveCount = int.Parse(parts[5]);

        return boardSetup;

    }
}