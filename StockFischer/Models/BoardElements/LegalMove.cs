using OpenPGN.Models;

namespace StockFischer.Models
{
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

    public class LegalMove : LiveBoardElement
    {
        public LegalMove(Square square) : base(square) { }

    }

    public class LegalCapture : LegalMove
    {
        public LegalCapture(Square square) : base(square) { }
    }

    public class CheckElement : LiveBoardElement
    {
        public CheckElement(Square square) : base(square) { }
    }

    public class CheckMateElement : CheckElement
    {
        public CheckMateElement(Square square) : base(square) { }
    }

    public class FileLegend : LiveBoardElement
    {
        public FileLegend(Square square) : base(square)
        {
        }
    }

    public class RankLegend : LiveBoardElement
    {
        public RankLegend(Square square) : base(square)
        {
        }
    }
}
