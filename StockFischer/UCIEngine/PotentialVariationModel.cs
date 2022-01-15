using OpenPGN;
using OpenPGN.Models;
using StockFischer.Models;
using System.Collections.Generic;
using System.Text;

namespace StockFischer.Engine;

public class PotentialVariationModel
{
    private readonly PotentialVariation _pv;


    /// <summary>
    /// MultiPV id from engine
    /// </summary>
    public int Id => _pv.Id;

    /// <summary>
    /// Depth in ply
    /// </summary>
    public int Depth => _pv.Depth;

    /// <summary>
    /// Evaluation from engines perspective
    /// postive if engine is winning, negative otherwise
    /// </summary>
    public Evaluation Evaluation => _pv.Evaluation;

    /// <summary>
    /// Kilo Nodes analysed per second
    /// </summary>
    public double KNps => _pv.KNps;

    /// <summary>
    /// Variation calculated
    /// </summary>
    public IEnumerable<MoveModel> Moves { get;}

    public PotentialVariationModel(string startpos, PotentialVariation pv)
    {
        _pv = pv;

        if(BoardSetupExtensions.GetActiveColor(startpos) is Color.Black)
        {
            _pv.Evaluation.Score *= -1;
            _pv.Evaluation.MateIn *= -1;
        }

        Moves = LiveBoard.ConvertMovesToAlgebraic(startpos, _pv.Moves);
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        return sb.Append($"{Evaluation} ")
                 .Append($"depth {Depth} ")
                 .Append(string.Join(", ", Moves))
                 .ToString();
    }
}
