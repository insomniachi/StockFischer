namespace OpenPGN.Models;

/// <summary>
/// Represents a RAV (recursive annotated variations) entry in the move text.
/// </summary>
public class RAVEntry : MoveTextEntry
{

    public MoveTextEntryList MoveText { get; set; }
    public bool IsContinued { get; set; }

    public RAVEntry(MoveTextEntryList moveText)
        : base(MoveTextEntryType.RecursiveAnnotationVariation)
    {
        MoveText = moveText;
    }
}
