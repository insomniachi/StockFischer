using OpenPGN.Models;
using System.Collections.Generic;

namespace StockFischer.Models
{
    public interface IPiece : ILiveBoardElement
    {
        PieceType Type { get; }
        Color Color { get; }
        IEnumerable<Square> GetLegalMoves(BoardSetup boardSetup);
    }
}
