using OpenPGN.Models;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StockFischer.Models;

public class LiveBoardElement : ILiveBoardElement
{
    public Square Square { get; set; }
    public int ZIndex { get; set; } = 0;
    public bool IsHitTestVisible { get; set; } = false;

    public LiveBoardElement(Square square)
    {
        Square = square;
    }

}

public class SquareHighlight : LiveBoardElement
{
    public SquareHighlight(Square square) : base(square) { }

    public System.Windows.Media.Color Color { get; set; } = Colors.Black;

}

/// <summary>
/// TODO : find a better name
/// </summary>
public class SquareHighlight2 : SquareHighlight
{
    public SquareHighlight2(Square square) : base(square) { }
}

public class LastMoveHighlight : SquareHighlight
{
    public LastMoveHighlight(Square square) : base(square) { }
}


public class Check : LiveBoardElement
{
    public Check(Square square) : base(square) { }
}

public class Checkmate : Check
{
    public Checkmate(Square square) : base(square) { }
}

public class ShapeElement : ILiveBoardElement
{
    public Square Square { get; set; }

    public Geometry Shape { get; set; }
}
