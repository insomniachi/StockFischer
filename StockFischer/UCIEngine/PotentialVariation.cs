using OpenPGN.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace StockFischer.Engine;

public class PotentialVariation
{
    private static readonly Regex _pvRegex = new(@"depth (?'Depth'\d+).*multipv (?'Id'\d+).*((cp (?'Eval'-?\d+))|(mate (?'Mate'-?\d+))).*nps (?'NPS'\d+).*pv (?'PV'(([a-h][1-8][qrbk]?){2}\s?)+)", RegexOptions.Compiled);

    /// <summary>
    /// MultiPV id from engine
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Depth in ply
    /// </summary>
    public int Depth { get; init; }

    /// <summary>
    /// Evaluation from engines perspective
    /// postive if engine is winning, negative otherwise
    /// </summary>
    public Evaluation Evaluation { get; set; }

    /// <summary>
    /// Kilo Nodes analysed per second
    /// </summary>
    public double KNps { get; init; }

    /// <summary>
    /// Variation calculated
    /// </summary>
    public IEnumerable<EngineMove> Moves { get; init; }

    public override string ToString()
    {
        StringBuilder sb = new();

        return sb.Append($"{Evaluation} ")
                 .Append($"depth {Depth} ")
                 .Append(string.Join(", ", Moves))
                 .ToString();
    }

    /// <summary>
    /// Parse from engine output
    /// </summary>
    /// <param name="engineOutput"></param>
    /// <returns></returns>
    public static PotentialVariation TryParseFromEngineOutput(string engineOutput)
    {
        if (_pvRegex.Match(engineOutput) is Match { Success: true } m)
        {
            var depth = int.Parse(m.Groups["Depth"].Value);
            var eval = 0.0;
            if (m.Groups["Eval"].Success)
            {
                eval = int.Parse(m.Groups["Eval"].Value) / 100.0;
            }


            var nps = int.Parse(m.Groups["NPS"].Value);
            var id = int.Parse(m.Groups["Id"].Value);
            var pv = m.Groups["PV"].Value;

            int mate = 0;
            if (m.Groups["Mate"].Success)
            {
                mate = int.Parse(m.Groups["Mate"].Value);
                eval = mate > 0 ? double.PositiveInfinity : double.NegativeInfinity;
            }

            List<EngineMove> moves = new();
            foreach (var move in pv.Split(" "))
            {
                if (move.Length < 4) continue;

                PieceType? promotedPiece = null;
                var origin = Square.New((File)move[0], int.Parse(move[1].ToString()));
                var target = Square.New((File)move[2], int.Parse(move[3].ToString()));

                if (move.Length == 5)
                {
                    promotedPiece = CharToPiece(move[4]);
                }

                moves.Add(new EngineMove
                {
                    From = origin,
                    To = target,
                    PromotedPiece = promotedPiece,
                });
            }


            return new PotentialVariation
            {
                Id = id,
                Depth = depth,
                Evaluation = new Evaluation { Score = eval, MateIn = mate },
                KNps = nps / 1000.0,
                Moves = moves,
            };
        }

        return null;
    }

    private static PieceType CharToPiece(char piece)
    {
        return char.ToLower(piece) switch
        {
            'p' => PieceType.Pawn,
            'n' => PieceType.Knight,
            'b' => PieceType.Bishop,
            'r' => PieceType.Rook,
            'q' => PieceType.Queen,
            'k' => PieceType.King,
            _ => throw new ArgumentException(null, nameof(piece))
        };
    }
}
