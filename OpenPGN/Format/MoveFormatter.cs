using OpenPGN.Models;
using System.Globalization;

namespace OpenPGN.Format;

/// <summary>
/// A special formatter moves in PGN notation.
/// </summary>
public class MoveFormatter
{
    /// <summary>
    /// The default writer.
    /// </summary>
    public readonly static MoveFormatter Default = new MoveFormatter();

    /// <summary>
    /// Formats the specified move and writes it to the writer.
    /// </summary>
    /// <param name="move">The move.</param>
    /// <param name="writer">The writer.</param>
    /// <exception cref="System.ArgumentException">Thrown on unsupported move types.</exception>
    public void Format(Move move, TextWriter writer)
    {
        var handled =
            HandleCastle(move, writer) ||
            HandleSimpleMove(move, writer) ||
            HandleCapturingMove(move, writer);

        if (!handled) throw new ArgumentException(string.Format("Unsupported MoveType {0}", move.Type));

        if (move.PromotedPiece != null)
        {
            writer.Write("=");
            writer.Write(GetPiece(move.PromotedPiece));
        }

        writer.Write(GetCheckAndMate(move));
        writer.Write(GetAnnotation(move));

        if (string.IsNullOrEmpty(move.Comment) == false)
        {
            writer.Write($" {{{move.Comment}}} ");
        }
    }

    /// <summary>
    /// Formats the specified move.
    /// </summary>
    /// <param name="move">The move.</param>
    /// <returns>The PGN representation of the move as string.</returns>
    public string Format(Move move)
    {
        var writer = new StringWriter();
        Format(move, writer);
        return writer.ToString();
    }

    /// <summary>
    /// Checks whether the specified move is a capturing move and formats it if so.
    /// </summary>
    /// <param name="move">The move.</param>
    /// <param name="writer">The writer.</param>
    /// <returns><c>true</c> if the move is a capturing move; otherwise <c>false</c></returns>
    private bool HandleCapturingMove(Move move, TextWriter writer)
    {
        if (move.Type != MoveType.Capture && move.Type != MoveType.CaptureEnPassant) return false;

        var origin = GetMoveOrigin(move);
        var target = GetMoveTarget(move);

        if (origin == string.Empty)
        {
            writer.Write(target);
            writer.Write("x");
        }
        else
        {
            writer.Write(origin);
            writer.Write("x");
            writer.Write(target);
        }
        if (move.Type == MoveType.CaptureEnPassant)
            writer.Write("e.p.");

        return true;
    }

    /// <summary>
    /// Checks whether the specified move is a simple move and formats it if so.
    /// </summary>
    /// <param name="move">The move.</param>
    /// <param name="writer">The writer.</param>
    /// <returns><c>true</c> if the move is a simple move; otherwise <c>false.</c></returns>
    private bool HandleSimpleMove(Move move, TextWriter writer)
    {
        if (move.Type != MoveType.Simple && move.Type != MoveType.DoublePawnMove) return false;

        var origin = GetMoveOrigin(move);
        var target = GetMoveTarget(move);

        writer.Write(origin);
        writer.Write(target);

        return true;
    }

    /// <summary>
    /// Checks whether the move is a castling move and formats it if so.
    /// </summary>
    /// <param name="move">The move.</param>
    /// <param name="writer">The writer.</param>
    /// <returns></returns>
    private bool HandleCastle(Move move, TextWriter writer)
    {
        switch (move.Type)
        {
            case MoveType.CastleKingSide:
                writer.Write("O-O");
                return true;

            case MoveType.CastleQueenSide:
                writer.Write("O-O-O");
                return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the string representation of the target. e.g. "Ne5" in "QxNe5",  "e4" in "e4", or "N5" in "QxN5"
    /// </summary>
    /// <param name="move">The move.</param>
    /// <returns>The string representation of the target.</returns>
    private string GetMoveTarget(Move move)
    {
        var piece = "";

        //if (move.Type != MoveType.Simple) // do not render target piece on a simple move 
        //    piece = GetPiece(move.TargetPiece);

        var target = "";
        if (move.TargetSquare is not null)
            target = piece + move.TargetSquare;
        else if (move.TargetFile is not null)
            target = piece + move.TargetFile?.ToString().ToLower();

        return target;
    }

    /// <summary>
    /// Gets the move origin (piece + starting square info) if specified. E.g. "Q" in "QxNe5" or "Qg2" in "Qg2xNe5", or even "" in "e4"
    /// </summary>
    /// <param name="move">The move.</param>
    /// <returns>The origin (piece + starting square info) if specified; otherwise an empty string</returns>
    private string GetMoveOrigin(Move move)
    {
        var piece = GetPiece(move.Piece);

        if (move.OriginSquare is not null)
            return piece + move.OriginSquare;

        var origin = "";
        if (move.OriginFile is not null)
            origin = move.OriginFile?.ToString().ToLower();

        if (move.OriginRank != null)
            origin += move.OriginRank;

        return piece + origin;
    }

    /// <summary>
    /// Gets the string representation of the specified piece if specified or an empty string.
    /// </summary>
    /// <param name="pieceType">Type of the piece or <c>null</c>.</param>
    /// <returns>The string representation of the specified piece if specified or an empty string.</returns>
    private string GetPiece(PieceType? pieceType)
    {
        if (pieceType == null || pieceType == PieceType.Pawn)
            return string.Empty;

        return ((char)pieceType).ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Gets the check and mate annotation.
    /// </summary>
    /// <param name="move">The move.</param>
    /// <returns>The check/checkmate anntoation symbols or an empty string if no chec/checkmate move.</returns>
    private string GetCheckAndMate(Move move)
    {
        if (move.IsCheckMate == true) return "#";
        if (move.IsDoubleCheck == true) return "++";
        if (move.IsCheck == true) return "+";

        return "";
    }

    /// <summary>
    /// Gets the move annotation symbol.
    /// </summary>
    /// <param name="move">The move.</param>
    /// <returns>The move annotation symbol.</returns>
    private string GetAnnotation(Move move)
    {
        if (move.Annotation == null) return "";

        return move.Annotation.Value switch
        {
            MoveAnnotation.MindBlowing => "!!!",
            MoveAnnotation.Brilliant => "!!",
            MoveAnnotation.Good => "!",
            MoveAnnotation.Interesting => "!?",
            MoveAnnotation.Dubious => "?!",
            MoveAnnotation.Mistake => "?",
            MoveAnnotation.Blunder => "??",
            MoveAnnotation.Abysmal => "???",
            MoveAnnotation.FascinatingButUnsound => "!!?",
            MoveAnnotation.Unclear => "∞",
            MoveAnnotation.WithCompensation => "=/∞",
            MoveAnnotation.EvenPosition => "=",
            MoveAnnotation.SlightAdvantageWhite => "+/=",
            MoveAnnotation.SlightAdvantageBlack => "=/+",
            MoveAnnotation.AdvantageWhite => "+/−",
            MoveAnnotation.AdvantageBlack => "−/+",
            MoveAnnotation.DecisiveAdvantageWhite => "+−",
            MoveAnnotation.DecisiveAdvantageBlack => "-+",
            MoveAnnotation.Space => "○",
            MoveAnnotation.Initiative => "↑",
            MoveAnnotation.Development => "↑↑",
            MoveAnnotation.Counterplay => "⇄",
            MoveAnnotation.Countering => "∇",
            MoveAnnotation.Idea => "Δ",
            MoveAnnotation.TheoreticalNovelty => "N",
            _ => "",
        };
    }
}
