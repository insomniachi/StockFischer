namespace OpenPGN.Models;

public class Piece
{
    public static readonly Piece WhitePawn = new(PieceType.Pawn, Color.White);
    public static readonly Piece WhiteKnight = new(PieceType.Knight, Color.White);
    public static readonly Piece WhiteBishop = new(PieceType.Bishop, Color.White);
    public static readonly Piece WhiteRook = new(PieceType.Rook, Color.White);
    public static readonly Piece WhiteQueen = new(PieceType.Queen, Color.White);
    public static readonly Piece WhiteKing = new(PieceType.King, Color.White);

    public static readonly Piece BlackPawn = new(PieceType.Pawn, Color.Black);
    public static readonly Piece BlackKnight = new(PieceType.Knight, Color.Black);
    public static readonly Piece BlackBishop = new(PieceType.Bishop, Color.Black);
    public static readonly Piece BlackRook = new(PieceType.Rook, Color.Black);
    public static readonly Piece BlackQueen = new(PieceType.Queen, Color.Black);
    public static readonly Piece BlackKing = new(PieceType.King, Color.Black);


    public Piece(PieceType type, Color color)
    {
        PieceType = type;
        Color = color;
    }

    public PieceType PieceType { get; private set; }

    public Color Color { get; private set; }

    public override string ToString()
    {
        return Color.ToString() + " " + PieceType.ToString();
    }
}
