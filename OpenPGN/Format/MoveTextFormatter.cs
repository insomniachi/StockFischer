using OpenPGN.Models;

namespace OpenPGN.Format;

/// <summary>
/// A special formatter for move text in PGN notation
/// </summary>
class MoveTextFormatter
{
    private readonly string _separator;

    public MoveTextFormatter(string separator = " ")
    {
        _separator = separator;
    }

    private readonly MoveFormatter _moveFormatter = new();

    public void Format(MoveTextEntry entry, TextWriter writer)
    {
        switch (entry.Type)
        {
            case MoveTextEntryType.MovePair:
                FormatPair((MovePairEntry)entry, writer);
                break; ;
            case MoveTextEntryType.SingleMove:
                FormatHalfMove((HalfMoveEntry)entry, writer);
                break; ;
            case MoveTextEntryType.RecursiveAnnotationVariation:
                FormatRAVEntry((RAVEntry)entry, writer);
                break;
            case MoveTextEntryType.GameEnd:
            case MoveTextEntryType.NumericAnnotationGlyph:
                writer.Write(entry.ToString());
                break;
        }
    }

    public string Format(MoveTextEntry entry)
    {
        var writer = new StringWriter();
        Format(entry, writer);
        return writer.ToString();
    }


    public void Format(List<MoveTextEntry> moveText, TextWriter writer)
    {
        if (moveText.Count == 0)
            return;

        //no foreach here as last one is special case (no trailing space)
        for (int i = 0; i < moveText.Count - 1; ++i)
        {
            Format(moveText[i], writer);
            writer.Write(_separator);
        }

        Format(moveText[^1], writer);
    }


    public string Format(List<MoveTextEntry> moveText)
    {
        var writer = new StringWriter();
        Format(moveText, writer);
        return writer.ToString();
    }

    private void FormatPair(MovePairEntry movePair, TextWriter writer)
    {
        if (movePair.MoveNumber != null)
        {
            writer.Write(movePair.MoveNumber);
            writer.Write(". ");
        }
        _moveFormatter.Format(movePair.White, writer);
        writer.Write(" ");
        _moveFormatter.Format(movePair.Black, writer);
    }


    private void FormatHalfMove(HalfMoveEntry entry, TextWriter writer)
    {
        if (entry.MoveNumber != null)
        {
            writer.Write(entry.MoveNumber);
            writer.Write(entry.IsContinued ? "... " : ". ");
        }

        _moveFormatter.Format(entry.Move, writer);
    }


    private void FormatRAVEntry(RAVEntry entry, TextWriter writer)
    {
        writer.Write("(");

        if (entry.MoveText.Count == 0)
            return;

        //no foreach here as last one is special case (no trailing space)
        for (int i = 0; i < entry.MoveText.Count - 1; ++i)
        {
            Format(entry.MoveText[i], writer);
            writer.Write(" ");
        }

        Format(entry.MoveText[^1], writer);

        writer.Write(")");
    }

}
