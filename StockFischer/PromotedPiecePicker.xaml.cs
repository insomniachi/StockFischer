using StockFischer.Models;
using OpenPGN.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace StockFischer
{
    /// <summary>
    /// Interaction logic for PromotedPiecePicker.xaml
    /// </summary>
    public partial class PromotedPiecePicker : UserControl
    {
        public PromotedPiecePicker()
        {
            InitializeComponent();
            ItemsControl.ItemsSource = new List<LivePiece>
            {
                LivePiece.GetPiece(new Piece(PieceType.Queen, Color), Square.Invalid),
                LivePiece.GetPiece(new Piece(PieceType.Rook, Color), Square.Invalid),
                LivePiece.GetPiece(new Piece(PieceType.Knight, Color), Square.Invalid),
                LivePiece.GetPiece(new Piece(PieceType.Bishop, Color), Square.Invalid)
            };
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(PromotedPiecePicker), new PropertyMetadata(Color.White, OnColorChanged));

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as PromotedPiecePicker;

            if(e.NewValue is Color color)
            {
                c.ItemsControl.ItemsSource = new List<LivePiece>
                {
                    LivePiece.GetPiece(new Piece(PieceType.Queen, color), Square.Invalid),
                    LivePiece.GetPiece(new Piece(PieceType.Rook, color), Square.Invalid),
                    LivePiece.GetPiece(new Piece(PieceType.Knight, color), Square.Invalid),
                    LivePiece.GetPiece(new Piece(PieceType.Bishop, color), Square.Invalid)
                };
            }
        }
    }
}
