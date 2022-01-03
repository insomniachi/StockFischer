using OpenPGN.Models;
using System;

namespace StockFischer.Models
{
    public delegate void MovePlayedHandler(MovePlayedEventArgs e);

    public class MovePlayedEventArgs : EventArgs
    {
        public MoveModel Move { get; set; }
        public Color Color { get; set; }
    }
}
