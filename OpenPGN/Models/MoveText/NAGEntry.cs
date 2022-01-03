namespace OpenPGN.Models;

/// <summary>
/// Represents a NAG (numeric annotation glyph) entry in the move text.
/// </summary>
public class NAGEntry : MoveTextEntry
{
    public int Code { get; set; }

    public NAGEntry(int code)
        : base(MoveTextEntryType.NumericAnnotationGlyph)
    {
        Code = code;
    }

    public override string ToString()
    {
        return "$" + Code;
    }
}
