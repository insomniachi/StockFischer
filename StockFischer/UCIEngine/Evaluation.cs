using System;

namespace StockFischer.Engine;

public class Evaluation
{
    public double Score { get; set; }
    public int MateIn { get; set; }

    public override string ToString()
    {
        if (MateIn != 0)
        {
            return $"{(Math.Sign(MateIn) > 0 ? "+" : "-")}M{Math.Abs(MateIn)}";
        }
        else if (Score == 0)
        {
            return "0";
        }
        else
        {
            return $"{(Math.Sign(Score) > 0 ? "+" : "-")}{Math.Abs(Score)}";
        }
    }
}
