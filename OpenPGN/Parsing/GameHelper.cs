using OpenPGN.Models;
using System.Text.RegularExpressions;

namespace OpenPGN.Parsing;

internal class GameHelpers
{
    static string[] SevenTags = new string[] { "Event", "Site", "Date", "Round", "White", "Black", "Result" };

    static void FillSevenTagInfo(Game game, string name, string value)
    {
        if (name == "Event")
        {
            game.Event = value;
        }
        else if (name == "Round")
        {
            game.Round = value;
        }
        else if (name == "Black")
        {
            game.BlackPlayer = value;
        }
        else if (name == "White")
        {
            game.WhitePlayer = value;
        }
        else if (name == "Result")
        {
            game.Result = value switch
            {
                "1-0" => GameResult.White,
                "0-1" => GameResult.Black,
                "1/2-1/2" => GameResult.Draw,
                _ => GameResult.Open
            };
        }
        else if (name == "Site")
        {
            game.Site = value;
        }
        else if (name == "Date")
        {
            var sections = value.Split('.');

            if (int.TryParse(sections[0], out int year))
            {
                game.Year = year;
            }
            if (int.TryParse(sections[1], out int month))
            {
                game.Month = month;
            }
            if (int.TryParse(sections[2], out int day))
            {
                game.Day = day;
            }
        }
    }

    internal static Game Create(string pgn)
    {
        var game = new Game();

        // Tag Pairs
        foreach (Match item in PgnRegex.GameInfoRegex.Matches(pgn))
        {
            string name = item.Groups["Name"].Value;
            string value = item.Groups["Value"].Value;

            if (SevenTags.Contains(name))
            {
                FillSevenTagInfo(game, name, value);
            }
            else
            {
                game.AdditionalInfo.Add(new GameInfo { Name = name, Value = value });
            }
        }

        // Parse and clear the variations for now, current regex can't handle this
        var variations = GetVariations(pgn);
        pgn = ClearVariations(pgn, variations);

        // Parse moves made
        game.MoveText = GetMoves(pgn);

        // Insert previously parsed variations
        GetRAVEntries(variations).ForEach(x => game.MoveText.AddEntry(x));

        // Add game end entry
        game.MoveText.Add(new GameEndEntry(game.Result));

        return game;
    }

    static string ClearVariations(string pgn, List<Variation> variations)
    {
        foreach (var item in variations)
        {
            pgn = pgn.Replace(@$"({item.Text})", string.Empty);
        }
        return pgn;
    }

    static MoveTextEntryList GetMoves(string pgn)
    {
        var entries = new MoveTextEntryList();

        var test = PgnRegex.MoveText;

        foreach (Match match in PgnRegex.MoveTextRegex.Matches(pgn))
        {
            if (match.GetMoveText() is MoveTextEntry mte)
            {
                entries.Add(mte);
            }
        }

        return entries;
    }

    static List<Variation> GetVariations(string pgn)
    {
        var entries = new List<Variation>();

        foreach (Match match in PgnRegex.RAV.Matches(pgn))
        {
            Variation v = new() { Text = match.Value[1..^1] };

            var subVariations = PgnRegex.RAV.Matches(v.Text);

            foreach (Match subVar in subVariations.Skip(1))
            {
                v.SubVariations = GetVariations(subVar.Value);
                v.SubVariations.ForEach(x => x.Parent = v);
            }

            if (PgnRegex.MoveNumberRegex.Match(v.Text) is Match { Success: true } m)
            {
                v.Move = int.Parse(m.Groups["MoveNumber"].Value);
                v.IsContinued = m.Groups["Continued"].Success;
                entries.Add(v);
            }
        }

        return entries;
    }

    static MoveTextEntryList GetRAVEntries(List<Variation> variations)
    {
        var entries = new MoveTextEntryList();
        GetRAVEntries(variations, entries);
        return entries;
    }

    static void GetRAVEntries(List<Variation> variations, MoveTextEntryList entries, RAVEntry? entry = null)
    {
        foreach (var item in variations)
        {
            if (item.HasSubVariations)
            {
                var newEntry = new RAVEntry(new MoveTextEntryList())
                {
                    MoveNumber = item.Move,
                    IsContinued = item.IsContinued
                };

                GetRAVEntries(item.SubVariations, newEntry.MoveText, newEntry);

                entries.AddEntry(newEntry);

                foreach (var e in GetMoves(item.Text!))
                {
                    newEntry.MoveText.AddEntry(e);
                }

            }
            else
            {
                var moves = new RAVEntry(GetMoves(item.Text!))
                {
                    MoveNumber = item.Move,
                    IsContinued = item.IsContinued
                };

                if (entry is not null)
                {
                    entry.MoveText.AddEntry(moves);
                }
                else
                {
                    entries.AddEntry(moves);
                }

                if (item.Parent is not null)
                {
                    item.Parent.Text = item.Parent.Text?.Replace($"({item.Text})", string.Empty);
                }
            }
        }
    }
}

internal class Variation
{
    public Variation? Parent { get; set; }
    public string? Text { get; set; }
    public int Move { get; set; }
    public bool IsContinued { get; set; }

    public List<Variation> SubVariations { get; set; } = new();
    public bool HasSubVariations => SubVariations.Count > 0;

    public override string ToString()
    {
        return Text ?? string.Empty;
    }
}
