using OpenPGN.Format;

namespace OpenPGN.Models;

public abstract class MoveTextEntry
{
    public int? MoveNumber { get; set; }

    public MoveTextEntryType Type { get; set; }

    protected MoveTextEntry(MoveTextEntryType type)
    {
        Type = type;
    }

    public override string ToString()
    {
        return new MoveTextFormatter().Format(this);
    }
}
