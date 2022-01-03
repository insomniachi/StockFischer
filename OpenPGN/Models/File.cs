using System.ComponentModel;

namespace OpenPGN.Models;

public enum File
{

    A = 'a',
    B = 'b',
    C = 'c',
    D = 'd',
    E = 'e',
    F = 'f',
    G = 'g',
    H = 'h'
}

public static class FileExtensions
{
    public static int ToInt(this File f, bool useZeroIndexing = false)
    {
        var offset = 0;
        if (useZeroIndexing)
        {
            offset = -1;
        }

        return f switch
        {
            File.A => 1 + offset,
            File.B => 2 + offset,
            File.C => 3 + offset,
            File.D => 4 + offset,
            File.E => 5 + offset,
            File.F => 6 + offset,
            File.G => 7 + offset,
            File.H => 8 + offset,
            _ => throw new InvalidEnumArgumentException(),
        };
    }

    public static File FromInt(int file, bool useZeroIndexing = false)
    {
        return useZeroIndexing
            ? file switch
            {
                0 => File.A,
                1 => File.B,
                2 => File.C,
                3 => File.D,
                4 => File.E,
                5 => File.F,
                6 => File.G,
                7 => File.H,
                _ => throw new ArgumentException(null, nameof(file)),
            }
            : file switch
            {
                1 => File.A,
                2 => File.B,
                3 => File.C,
                4 => File.D,
                5 => File.E,
                6 => File.F,
                7 => File.G,
                8 => File.H,
                _ => throw new ArgumentException(null, nameof(file)),
            };
    }
}