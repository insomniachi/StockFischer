namespace OpenPGN.Models;

/// <summary>
/// List of MoveTextEntries which also provides some helpful
/// properties like MoveCount.
/// </summary>
public class MoveTextEntryList : List<MoveTextEntry>
{
    public MoveTextEntryList() { }

    public MoveTextEntryList(IEnumerable<MoveTextEntry> entries) : base(entries) { }

    public void AddEntry(MoveTextEntry entry)
    {
        bool isContinued = false;

        if (entry is RAVEntry re)
        {
            isContinued = re.IsContinued;
        }

        var list = this.ToList();

        if (Find(x => x.MoveNumber == entry.MoveNumber) is MovePairEntry mpe)
        {
            int index = IndexOf(mpe);
            Insert(index + 1, entry);
        }
        else if (this.OfType<HalfMoveEntry>().FirstOrDefault(x => x.MoveNumber == entry.MoveNumber && x.IsContinued == isContinued) is HalfMoveEntry hme)
        {
            int index = IndexOf(hme);
            Insert(index + 1, entry);
        }
        else if (Find(x => x.MoveNumber < entry.MoveNumber) is null)
        {
            if (Count == 0)
            {
                Add(entry);
            }
            else
            {
                Insert(0, entry);
            }
        }
        else
        {
            var max = this.Where(x => x.MoveNumber < entry.MoveNumber)
                .MaxBy(x => x.MoveNumber);

            int index = IndexOf(max!);
            Insert(index + 1, entry);
        }

    }

    /// <summary>
    /// Gets the number of half-moves (moves by black *or* white)
    /// </summary>
    public int MoveCount
    {
        get
        {
            int count = 0;
            foreach (var entry in this)
            {
                if (entry.Type == MoveTextEntryType.MovePair)
                    count += 2;
                else if (entry.Type == MoveTextEntryType.SingleMove)
                    count += 1;
            }

            return count;
        }
    }

    /// <summary>
    /// Gets the number of full moves
    /// </summary>
    public int FullMoveCount
    {
        get { return MoveCount / 2; }
    }

    /// <summary>
    /// Gets the moves from the move text.
    /// </summary>
    /// <returns>Enumerations of the moves.</returns>
    public IEnumerable<Move> GetMoves()
    {
        foreach (var entry in this)
        {
            if (entry.Type == MoveTextEntryType.MovePair)
            {
                yield return ((MovePairEntry)entry).White;
                yield return ((MovePairEntry)entry).Black;
            }
            else if (entry.Type == MoveTextEntryType.SingleMove)
            {
                yield return ((HalfMoveEntry)entry).Move;
            }
        }
    }
}
