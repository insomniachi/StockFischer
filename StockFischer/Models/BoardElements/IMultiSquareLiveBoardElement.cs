namespace StockFischer.Models
{
    public interface IMultiSquareLiveBoardElement : ILiveBoardElement
    {
        int Height { get; }
        int Width { get; }
    }
}
