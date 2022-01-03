using System.Text.RegularExpressions;

namespace OpenPGN.Models;

public class Square
{
    internal Square(File file, int rank)
    {
        File = file;
        Rank = rank;
    }

    public static Square New(File file, int rank) => new(file, rank);

    public File File { get; internal set; }

    public int Rank { get; internal set; }

    public void Destructor(out File file, out int rank)
    {
        file = File;
        rank = Rank;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Square other) return false;

        return
            File == other.File &&
            Rank == other.Rank;
    }

    public override int GetHashCode()
    {
        return ((int)File) * Rank;
    }

    public override string ToString()
    {
        return File.ToString().ToLower() + Rank;
    }

    public static bool operator ==(Square? a, Square? b)
    {
        if (ReferenceEquals(a, b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if ((a is null) || (b is null))
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(Square? a, Square? b)
    {
        return !(a == b);
    }

    public static Square? Parse(string square)
    {
        if(Regex.IsMatch(square, "[a-h][1-8]") == false)
        {
            return null;
        }
        else
        {
            return New((File)square[0], int.Parse(square[1].ToString()));
        }
    }

    #region Squares
    public static readonly Square Invalid = New(File.A, -1);

    public static readonly Square A1 = new(File.A, 1);
    public static readonly Square A2 = new(File.A, 2);
    public static readonly Square A3 = new(File.A, 3);
    public static readonly Square A4 = new(File.A, 4);
    public static readonly Square A5 = new(File.A, 5);
    public static readonly Square A6 = new(File.A, 6);
    public static readonly Square A7 = new(File.A, 7);
    public static readonly Square A8 = new(File.A, 8);
    public static readonly Square B1 = new(File.B, 1);
    public static readonly Square B2 = new(File.B, 2);
    public static readonly Square B3 = new(File.B, 3);
    public static readonly Square B4 = new(File.B, 4);
    public static readonly Square B5 = new(File.B, 5);
    public static readonly Square B6 = new(File.B, 6);
    public static readonly Square B7 = new(File.B, 7);
    public static readonly Square B8 = new(File.B, 8);
    public static readonly Square C1 = new(File.C, 1);
    public static readonly Square C2 = new(File.C, 2);
    public static readonly Square C3 = new(File.C, 3);
    public static readonly Square C4 = new(File.C, 4);
    public static readonly Square C5 = new(File.C, 5);
    public static readonly Square C6 = new(File.C, 6);
    public static readonly Square C7 = new(File.C, 7);
    public static readonly Square C8 = new(File.C, 8);
    public static readonly Square D1 = new(File.D, 1);
    public static readonly Square D2 = new(File.D, 2);
    public static readonly Square D3 = new(File.D, 3);
    public static readonly Square D4 = new(File.D, 4);
    public static readonly Square D5 = new(File.D, 5);
    public static readonly Square D6 = new(File.D, 6);
    public static readonly Square D7 = new(File.D, 7);
    public static readonly Square D8 = new(File.D, 8);
    public static readonly Square E1 = new(File.E, 1);
    public static readonly Square E2 = new(File.E, 2);
    public static readonly Square E3 = new(File.E, 3);
    public static readonly Square E4 = new(File.E, 4);
    public static readonly Square E5 = new(File.E, 5);
    public static readonly Square E6 = new(File.E, 6);
    public static readonly Square E7 = new(File.E, 7);
    public static readonly Square E8 = new(File.E, 8);
    public static readonly Square F1 = new(File.F, 1);
    public static readonly Square F2 = new(File.F, 2);
    public static readonly Square F3 = new(File.F, 3);
    public static readonly Square F4 = new(File.F, 4);
    public static readonly Square F5 = new(File.F, 5);
    public static readonly Square F6 = new(File.F, 6);
    public static readonly Square F7 = new(File.F, 7);
    public static readonly Square F8 = new(File.F, 8);
    public static readonly Square G1 = new(File.G, 1);
    public static readonly Square G2 = new(File.G, 2);
    public static readonly Square G3 = new(File.G, 3);
    public static readonly Square G4 = new(File.G, 4);
    public static readonly Square G5 = new(File.G, 5);
    public static readonly Square G6 = new(File.G, 6);
    public static readonly Square G7 = new(File.G, 7);
    public static readonly Square G8 = new(File.G, 8);
    public static readonly Square H1 = new(File.H, 1);
    public static readonly Square H2 = new(File.H, 2);
    public static readonly Square H3 = new(File.H, 3);
    public static readonly Square H4 = new(File.H, 4);
    public static readonly Square H5 = new(File.H, 5);
    public static readonly Square H6 = new(File.H, 6);
    public static readonly Square H7 = new(File.H, 7);
    public static readonly Square H8 = new(File.H, 8);

    public static IEnumerable<Square> AsEnumerable()
    {
        return new List<Square>
        {
            A1, A2, A3, A4, A5, A6, A7, A8,
            B1, B2, B3, B4, B5, B6, B7, B8,
            C1, C2, C3, C4, C5, C6, C7, C8,
            D1, D2, D3, D4, D5, D6, D7, D8,
            E1, E2, E3, E4, E5, E6, E7, E8,
            F1, F2, F3, F4, F5, F6, F7, F8,
            G1, G2, G3, G4, G5, G6, G7, G8,
            H1, H2, H3, H4, H5, H6, H7, H8,
        };
    }

    #endregion

}
