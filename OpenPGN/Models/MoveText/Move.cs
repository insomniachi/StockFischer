using OpenPGN.Format;

namespace OpenPGN.Models;

/// <summary>
/// A move (actually Half-Move or Ply). Holds all information about the move and does not check for inconsistency, completeness, contradictions...
/// NOTE: The same move may be expressed in different ways: Qd5xe6 or Qd5xKe6 or QxKe6 etc...
/// NOTE: The move class does not check for correctness of moves. Illegal moves may be constructed here.
/// </summary>
public class Move
{

    public MoveType Type { get; set; }

    public PieceType? TargetPiece { get; set; }

    public Square? TargetSquare { get; set; }

    public File? TargetFile { get; set; }

    public PieceType Piece { get; set; }

    public Square? OriginSquare { get; set; }

    public File? OriginFile { get; set; }

    public int? OriginRank { get; set; }

    public PieceType? PromotedPiece { get; set; }

    public bool? IsCheck { get; set; }

    public bool? IsDoubleCheck { get; set; }

    public bool? IsCheckMate { get; set; }

    public MoveAnnotation? Annotation { get; set; }

    public string? Comment { get; set; }

    public override bool Equals(object? obj)
    {
        var other = obj as Move;
        if (other == null) return false;
        if (this == obj) return true;

        return
            Type == other.Type &&
            TargetPiece == other.TargetPiece &&
            TargetSquare == other.TargetSquare &&
            Piece == other.Piece &&
            OriginSquare == other.OriginSquare &&
            OriginFile == other.OriginFile &&
            OriginRank == other.OriginRank &&
            PromotedPiece == other.PromotedPiece &&
            IsCheck == other.IsCheck &&
            IsCheckMate == other.IsCheckMate &&
            Annotation == other.Annotation;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + GetNullableHashCode(TargetSquare);
            hash = hash * 23 + GetNullableHashCode(Piece);
            hash = hash * 23 + GetNullableHashCode(OriginSquare);
            hash = hash * 23 + GetNullableHashCode(OriginFile);
            hash = hash * 23 + GetNullableHashCode(OriginRank);
            hash = hash * 23 + GetNullableHashCode(PromotedPiece);
            hash = hash * 23 + GetNullableHashCode(IsCheck);
            hash = hash * 23 + GetNullableHashCode(IsCheckMate);
            hash = hash * 23 + GetNullableHashCode(Annotation);
            return hash;
        }
    }

    private int GetNullableHashCode(object? obj)
    {
        return obj?.GetHashCode() ?? 1;
    }

    public override string ToString()
    {
        return MoveFormatter.Default.Format(this);
    }
}
